
namespace Defense.Interfaces
{
	/// <summary>
	/// 
	/// 한 웨이브/게임이 끝났을 때, 초기화하는 인터페이스
	/// 
	/// - Unit, Building 등 에서 사용
	/// - 스탯, 버프, 데이터 등 초기화
	/// 
	/// </summary>
	public interface IResettable
	{
		public void OnGameReset();
		public void OnWaveReset();
	}
}
