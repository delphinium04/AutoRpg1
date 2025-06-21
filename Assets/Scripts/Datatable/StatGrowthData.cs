
namespace DataTable
{
	[System.Serializable]
	public class StatGrowthData
	{
		public int StatID; // 스텟아이디
		public string StatType; // 스탯 타입
		public int Base; // 기본값
		public int Increase ; // 스탯 증가량
		public int BaseCost; // 기본 비용
		public float CostRate; // 비용 증가 비율
		public int MaxLevel; // 최대 레벨

	}
}
