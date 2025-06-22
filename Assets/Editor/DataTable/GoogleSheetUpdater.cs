using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

// EditorCoroutineUtility 사용

namespace Editor.Datatable
{
    [System.Serializable]
    public class Sheet
    {
        public string sheetName;
        public int sheetId;
    }

    [System.Serializable]
    public class SheetList
    {
        public Sheet[] sheetInfos;
    }

    public class GoogleSheetDownloader : EditorWindow
    {
        private const string STATE_PREF_KEY = "GoogleSheetDownloader_State";
    
        private enum State
        {
            Idle,
            SheetDownloaded,
            ClassGenerated,
            Done
        }

        private static State _state;

        private string _urlSpreadSheet =
            "https://docs.google.com/spreadsheets/d/1VlvBPiuNu68FzotLLMY4joOnXOeZ56C_3fHiTAVCKLQ";

        private string _urlScriptAPI =
            "https://script.google.com/macros/s/AKfycbxsDPEGYqYsQc9pt7suqQVnaSsKuYuslXJziVSVCDppEfoSHHR2F9ASHp7kFwQVqGOv/exec";


        private List<Sheet> _sheets = new List<Sheet>();


        [MenuItem("Tools/Google Sheet Downloader")]
        public static void ShowWindow()
        {
            _state = State.Idle;
            GetWindow<GoogleSheetDownloader>("Google Sheet Downloader");
        }
    
        private void OnEnable()
        {
            // 저장된 상태 복원
            _state = (State)EditorPrefs.GetInt(STATE_PREF_KEY, 0);
        }
    
        private void SaveState(State newState)
        {
            _state = newState;
            EditorPrefs.SetInt(STATE_PREF_KEY, (int)_state);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Cleanup"))
            {
                bool confirmed = EditorUtility.DisplayDialog("Cleanup", "Do it?", "Confirm", "Cancel");
                if (confirmed)
                {
                    _state = State.Idle;
                    SaveState(_state);
                    DatatableCsvManager.DeleteAllGeneratedFiles();
                    AssetDatabase.Refresh();
                }
            }

            GUILayout.Space(20);

            switch (_state)
            {
                case State.Idle:
                    ShowSheetDownloadButton();
                    break;
                case State.SheetDownloaded:
                    ShowClassGenerateButton();
                    break;
                case State.ClassGenerated:
                    ShowDataSaveButton();
                    break;
                case State.Done:
                    GUILayout.Label("Done");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowSheetDownloadButton()
        {
            GUILayout.Label("Google Spreadsheet Url", EditorStyles.boldLabel);
            _urlSpreadSheet = EditorGUILayout.TextField("Sheet URL", _urlSpreadSheet);
            GUILayout.Label("Google Apps Script URL", EditorStyles.boldLabel);
            _urlScriptAPI = EditorGUILayout.TextField("Sheet URL", _urlScriptAPI);

            // 시트 데이터 가져오기
            if (GUILayout.Button("Fetch Sheet Data"))
            {
                _sheets.Clear();
                if (string.IsNullOrEmpty(_urlScriptAPI))
                {
                    EditorUtility.DisplayDialog("Error", "Invalid URL", "Ok");
                    return;
                }

                // Download and add to sheetList
                EditorCoroutineUtility.StartCoroutineOwnerless(Download(_urlScriptAPI,
                    sheetData =>
                    {
                        Debug.Log("Sheet Information Download succeed\n");
                        try
                        {
                            var sheetList = JsonUtility.FromJson<SheetList>(sheetData);
                            foreach (var t in sheetList.sheetInfos)
                            {
                                if (t.sheetName.StartsWith("#")) continue;
                                _sheets.Add(t);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            Debug.LogWarning("Need latest google apps scripts url");
                        }
                        finally
                        {
                            SaveState(State.SheetDownloaded);
                        }
                    }));
            }
        }

        private void ShowClassGenerateButton()
        {
            if (_sheets.Count == 0)
            {
                Debug.LogWarning("No sheet to generate");
                return;
            }

            // Show fetched sheets 
            GUILayout.Space(20);
            GUILayout.Label("Fetched Sheet List", EditorStyles.boldLabel);
            foreach (var sheet in _sheets)
            {
                GUILayout.Label($"{sheet.sheetName}: {sheet.sheetId}");
            }

            // Download data as csv and parse
            if (GUILayout.Button("Generate Class"))
            {
                foreach (var sheet in _sheets)
                {
                    string url = GetSheetUrl(_urlSpreadSheet, sheet.sheetId);
                    EditorCoroutineUtility.StartCoroutineOwnerless(Download(url,
                        csv => DatatableCsvManager.Parse(sheet.sheetName, csv)));
                }

                SaveState(State.ClassGenerated);
            }
        }

        private void ShowDataSaveButton()
        {
            if (GUILayout.Button("Save Data"))
            {
                DatatableCsvManager.CreateScriptableObject();
                SaveState(State.Done);
            }
        }


        #region GoogleSheets

        private string GetSheetUrl(string url, int sheetId)
        {
            var match = System.Text.RegularExpressions.Regex.Match(url, @"\/spreadsheets\/d\/([a-zA-Z0-9-_]+)");
            if (!match.Success) return null;

            return $"https://docs.google.com/spreadsheets/d/{match.Groups[1]}/export?format=csv&gid={sheetId}";
        }

        private IEnumerator Download(string url, Action<string> callback = null)
        {
            // 첫 번째 시트(gid=0) 기준, 필요시 gid 값 수정
            // string downloadUrl = $"{baseUrl}/export?format=csv&gid=0";
            EditorUtility.DisplayProgressBar("구글 시트 다운로드", "데이터를 다운로드 중입니다...", 0.1f);

            using var request = UnityEngine.Networking.UnityWebRequest.Get(url);
            var async = request.SendWebRequest();
            while (!async.isDone)
            {
                EditorUtility.DisplayProgressBar("Downloading...", "Downloading from " + url, async.progress);
                yield return null;
            }

            EditorUtility.ClearProgressBar();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                callback?.Invoke(request.downloadHandler.text);
            }
            else
            {
                var resultText = $"다운로드 실패: {request.error}\n\n{request.downloadHandler.text}";
                Debug.LogError(resultText);
                EditorUtility.DisplayDialog("오류", $"다운로드 실패: {request.error}\n\n{request.downloadHandler.text}",
                    "확인");
            }

            Repaint(); // 결과 갱신
        }

        #endregion
    }
}