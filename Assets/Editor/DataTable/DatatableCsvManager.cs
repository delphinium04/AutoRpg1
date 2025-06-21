using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor.DataTable
{
    public static class DatatableCsvManager
    {
        private class ParsedCsv
        {
            public string SheetName;
            public string[] CommentNames; // korean etc
            public string[] VarNames;
            public string[] VarTypes;
            public string[] Data; // csv after line 2
        }

        private const string BaseScriptPath = "Assets/Scripts/Datatable/";
        private const string BaseDataPath = "Assets/Resources/Datatable/";
        private const string DatatableNamespace = "DataTable";

        public static void DeleteAllGeneratedFiles()
        {
            Delete(BaseScriptPath);
            Delete(BaseDataPath);
            return;

            void Delete(string path)
            {
                if (!Directory.Exists(path))
                {
                    Debug.LogWarning($"path is invalid: {path}");
                    return;
                }

                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException e)
                    {
                        Debug.LogError($"fail: {file}\n{e}");
                    }
                }
            }
        }

        public static void Parse(string sheetName, string csv)
        {
            string[] lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // 실제 데이터 영역 확인
            int validRowCount = 0;
            int validColumnCount = 0;

            string[] columns = lines[0].Split(',');
            validColumnCount = columns.Length;
            for (int i = 0; i < columns.Length; i++)
            {
                if (!string.IsNullOrEmpty(columns[i]) && !string.IsNullOrWhiteSpace(columns[i])) continue;
                validColumnCount = i;
                break;
            }

            validRowCount = lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                if (!string.IsNullOrEmpty(values[0]) && !string.IsNullOrWhiteSpace(values[0])) continue;
                validRowCount = i + 1;
                break;
            }

            if (validRowCount < 3 || validColumnCount == 0)
            {
                Debug.LogWarning($"Invalid {sheetName} CSV format, Cancel generating");
                return;
            }

            Debug.Log($"Read {sheetName} from A:1 to {GetGoogleSheetColumnLetter(validColumnCount)}:{validRowCount}");


            // 데이터 필터링 및 저장 (# 무시)
            bool[] ignoreColumns = new bool[validColumnCount];
            for (int i = 0; i < validColumnCount; i++)
                ignoreColumns[i] = columns[i].StartsWith("#");

            ParsedCsv parsedCsv = new ParsedCsv();
            parsedCsv.SheetName = sheetName;
            parsedCsv.CommentNames = FilterColumns(lines[0], ignoreColumns).Split(',');
            parsedCsv.VarNames = FilterColumns(lines[1], ignoreColumns).Split(',');
            parsedCsv.VarTypes = FilterColumns(lines[2], ignoreColumns).Split(',');
            parsedCsv.Data = FilterColumnsMultiline(csv, ignoreColumns)[3..];


            // 코드 생성
            GenerateDataClass(parsedCsv);
            GenerateTableClass(parsedCsv);
            GenerateProviderClass(parsedCsv);
            AssetDatabase.Refresh();
            Debug.Log($"Generated Script of {sheetName} Successfully, try to generate scriptable object...");

            // 데이터 생성
            CreateScriptableObject(parsedCsv);
            Debug.Log($"{sheetName}: Done");
        }

        private static void GenerateDataClass(ParsedCsv parsedCsv)
        {
            // add variable lines
            string variables = "";
            for (int i = 0; i < parsedCsv.CommentNames.Length; i++)
            {
                variables +=
                    $"\t\tpublic {parsedCsv.VarTypes[i]} {parsedCsv.VarNames[i]}; // {parsedCsv.CommentNames[i]}\n";
            }

            string script = string.Format(DatatableClassFormat.Data, DatatableNamespace, parsedCsv.SheetName,
                variables);
            File.WriteAllText(BaseScriptPath + $"{parsedCsv.SheetName}Data.cs", script, Encoding.UTF8);
        }

        private static void GenerateTableClass(ParsedCsv parsedCsv)
        {
            string script = string.Format(DatatableClassFormat.Table, DatatableNamespace, parsedCsv.SheetName);
            File.WriteAllText(BaseScriptPath + $"{parsedCsv.SheetName}Table.cs", script, Encoding.UTF8);
        }

        private static void GenerateProviderClass(ParsedCsv csv)
        {
            string idVariableName = csv.VarNames[0];
            string script = string.Format(DatatableClassFormat.Provider, DatatableNamespace, csv.SheetName,
                csv.VarNames[0]);
            File.WriteAllText(BaseScriptPath + $"{csv.SheetName}DataProvider.cs", script, Encoding.UTF8);
        }

        private static void CreateScriptableObject(ParsedCsv csv)
        {
            try
            {
                // ScriptableObject 생성
                var tableType = GetTypeFromName($"{DatatableNamespace}.{csv.SheetName}Table");
                var dataType = GetTypeFromName($"{DatatableNamespace}.{csv.SheetName}Data");

                if (tableType == null || dataType == null)
                {
                    Debug.LogError(
                        $"Types not found for {csv.SheetName}. Make sure to compile the generated scripts first.");
                    return;
                }

                // SO 인스턴스 생성
                var table = ScriptableObject.CreateInstance(tableType);
                var listType = typeof(List<>).MakeGenericType(dataType);
                var list = (IList)Activator.CreateInstance(listType);

                // 데이터 행 처리
                foreach (var dataLine in csv.Data)
                {
                    string[] data = dataLine.Split(',');
                    var dataInstance = CreateDataInstance(dataType, csv, data);
                    if (dataInstance != null)
                    {
                        list.Add(dataInstance);
                    }
                }

                // list 프로퍼티 설정
                var listField = tableType.GetField("list");
                listField.SetValue(table, list);

                // 파일로 저장
                SaveScriptableObject(table, csv.SheetName);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating ScriptableObject for {csv.SheetName}: {e.Message}\n{e.StackTrace}");
            }
        }

        private static Type GetTypeFromName(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static object CreateDataInstance(Type dataType, ParsedCsv csv, string[] values)
        {
            var instance = Activator.CreateInstance(dataType);

            for (int i = 0; i < csv.VarNames.Length; i++)
            {
                try
                {
                    var field = dataType.GetField(csv.VarNames[i]);
                    if (field != null)
                    {
                        object convertedValue = ConvertValue(values[i], csv.VarTypes[i]);
                        field.SetValue(instance, convertedValue);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error setting field {csv.VarNames[i]}: {e.Message}");
                }
            }

            return instance;
        }

        private static object ConvertValue(string value, string type)
        {
            if (string.IsNullOrEmpty(value))
                return GetDefaultValue(type);

            switch (type.ToLower())
            {
                case "short":
                    return short.Parse(value);
                case "int":
                    return int.Parse(value);
                case "float":
                    return float.Parse(value);
                case "bool":
                    return bool.Parse(value);
                case "string":
                    return value;
                default:
                    if (Type.GetType($"System.{type}") is { } enumType && enumType.IsEnum)
                        return Enum.Parse(enumType, value);
                    throw new ArgumentException($"Unsupported type: {type}");
            }
        }

        private static object GetDefaultValue(string type)
        {
            return type.ToLower() switch
            {
                "short" => 0,
                "int" => 0,
                "float" => 0f,
                "bool" => false,
                "string" => "",
                _ => null
            };
            // Enum에 따른 추가 작업 필요
        }

        private static void SaveScriptableObject(ScriptableObject asset, string sheetName)
        {
            if (!Directory.Exists(BaseDataPath))
                Directory.CreateDirectory(BaseDataPath);

            string assetPath = Path.Combine(BaseDataPath, $"{sheetName}Table.asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created ScriptableObject at {assetPath}");
        }

        private static string GetGoogleSheetColumnLetter(int columnNumber)
        {
            string columnLetter = string.Empty;
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnLetter = Convert.ToChar('A' + modulo) + columnLetter;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnLetter;
        }

        private static string FilterColumns(string csvLine, bool[] ignoreColumns)
        {
            if (string.IsNullOrEmpty(csvLine)) return string.Empty;

            string[] values = csvLine.Split(',');
            if (values.Length != ignoreColumns.Length)
            {
                Debug.LogError(
                    $"Column count mismatch. CSV has {values.Length} columns but ignore array has {ignoreColumns.Length} columns");
                return csvLine;
            }

            var filteredValues = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                if (!ignoreColumns[i])
                {
                    filteredValues.Add(values[i]);
                }
            }

            return string.Join(",", filteredValues);
        }

        private static string[] FilterColumnsMultiline(string csv, bool[] ignoreColumns)
        {
            string[] lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var filteredLines = lines.Select(line => FilterColumns(line, ignoreColumns));
            return filteredLines.ToArray();
        }
    }
}