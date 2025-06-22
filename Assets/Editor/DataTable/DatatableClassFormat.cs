namespace Editor.Datatable
{
	public static class DatatableClassFormat
    {
        /// <summary>
        /// 0: namespace
        /// 1: sheetName
        /// 2: variables
        /// </summary>
        public const string Data =
            @"
namespace {0}
{{
	[System.Serializable]
	public class {1}Data
	{{
{2}
	}}
}}
";

        /// <summary>
        /// 0: namespace
        /// 1: sheetName
        /// </summary>
        public const string Table =
            @"
using UnityEngine;
using System.Collections.Generic;

namespace {0}
{{
	[CreateAssetMenu(fileName = ""{1}Table"", menuName = ""DataTable/{1}Table"")]
	public class {1}Table : ScriptableObject
	{{
		public List<{1}Data> list;
	}}
}}
";

        /// <summary>
        /// 0: namespace
        /// 1: sheetName
        /// 2: key variable name
        /// </summary>
        public const string Provider =
@"
using System.Collections.Generic;
using UnityEngine;

namespace {0}
{{
	public static class {1}DataProvider
	{{
		private static {1}Table _{1}Table;
		private static Dictionary<int, {1}Data>  _{1}Data;

		public static {1}Table Table
		{{
			get
			{{
				if (_{1}Table == null)
				{{
					_{1}Table = Resources.Load<{1}Table>(""Datatable/{1}Table"");
					if(_{1}Table == null)
						Debug.LogError($""{1}Data was not found"");
				}}
				return _{1}Table;
			}}
		}}

		public static {1}Data Get(int keyValue)
		{{
			if (_{1}Data == null)
			{{
				_{1}Data = new Dictionary<int, {1}Data>();
				foreach (var data in Table.list)
				{{
					_{1}Data.Add(data.{2}, data);
				}}
			}}
			
			if (_{1}Data.TryGetValue(keyValue, out {1}Data result))
				return result;
			
			Debug.Log($""{1}Data which ID is {{keyValue}} was not found, return last one: {{Table.list[^1]}}"");
			return Table.list[^1];
		}} 
	}}
}}

";
        
        
    }
}