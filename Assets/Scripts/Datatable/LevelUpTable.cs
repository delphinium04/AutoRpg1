
using UnityEngine;
using System.Collections.Generic;

namespace Datatable
{
	[CreateAssetMenu(fileName = "LevelUpTable", menuName = "DataTable/LevelUpTable")]
	public class LevelUpTable : ScriptableObject
	{
		public List<LevelUpData> list;
	}
}
