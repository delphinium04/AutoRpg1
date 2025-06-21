
using UnityEngine;
using System.Collections.Generic;

namespace DataTable
{
	[CreateAssetMenu(fileName = "StatGrowthTable", menuName = "DataTable/StatGrowthTable")]
	public class StatGrowthTable : ScriptableObject
	{
		public List<StatGrowthData> list;
	}
}
