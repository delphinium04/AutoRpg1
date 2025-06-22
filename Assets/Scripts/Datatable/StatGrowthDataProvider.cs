
using System.Collections.Generic;
using UnityEngine;

namespace Datatable
{
	public static class StatGrowthDataProvider
	{
		private static StatGrowthTable _StatGrowthTable;
		private static Dictionary<int, StatGrowthData>  _StatGrowthData;

		public static StatGrowthTable Table
		{
			get
			{
				if (_StatGrowthTable == null)
				{
					_StatGrowthTable = Resources.Load<StatGrowthTable>("Datatable/StatGrowthTable");
					if(_StatGrowthTable == null)
						Debug.LogError($"StatGrowthData was not found");
				}
				return _StatGrowthTable;
			}
		}

		public static StatGrowthData Get(int keyValue)
		{
			if (_StatGrowthData == null)
			{
				_StatGrowthData = new Dictionary<int, StatGrowthData>();
				foreach (var data in Table.list)
				{
					_StatGrowthData.Add(data.statId, data);
				}
			}
			
			if (_StatGrowthData.TryGetValue(keyValue, out StatGrowthData result))
				return result;
			
			Debug.Log($"StatGrowthData which ID is {keyValue} was not found, return last one: {Table.list[^1]}");
			return Table.list[^1];
		} 
	}
}

