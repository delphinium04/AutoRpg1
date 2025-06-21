
using System.Collections.Generic;
using UnityEngine;

namespace DataTable
{
	public static class LevelUpDataProvider
	{
		private static LevelUpTable _LevelUpTable;
		private static Dictionary<int, LevelUpData>  _LevelUpData;

		public static LevelUpTable Table
		{
			get
			{
				if (_LevelUpTable == null)
				{
					_LevelUpTable = Resources.Load<LevelUpTable>("Datatable/LevelUpTable");
					if(_LevelUpTable == null)
						Debug.LogError($"LevelUpData was not found");
				}
				return _LevelUpTable;
			}
		}

		public static LevelUpData Get(int keyValue)
		{
			if (_LevelUpData == null)
			{
				_LevelUpData = new Dictionary<int, LevelUpData>();
				foreach (var data in Table.list)
				{
					_LevelUpData.Add(data.ID, data);
				}
			}
			
			if (_LevelUpData.TryGetValue(keyValue, out LevelUpData result))
				return result;
			
			Debug.Log($"LevelUpData which ID is {keyValue} was not found, return last one: {Table.list[^1]}");
			return Table.list[^1];
		} 
	}
}

