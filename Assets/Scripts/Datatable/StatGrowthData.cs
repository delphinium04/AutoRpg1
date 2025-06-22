
namespace Datatable
{
	[System.Serializable]
	public class StatGrowthData
	{
		public int statId; // 스텟아이디
		public string statType; // 스탯 타입
		public int defaultValue; // 기본값
		public int statIncreasement; // 스탯 증가량
		public int defaultCost; // 기본 비용
		public float costIncreasementRate; // 비용 증가 비율
		public int maxLevel; // 최대 레벨

	}
}
