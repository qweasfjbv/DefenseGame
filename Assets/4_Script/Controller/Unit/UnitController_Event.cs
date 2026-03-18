namespace Defense.Controller
{
	/// <summary>
	/// UnitController의 이벤트 관련 함수들을 담습니다.
	/// </summary>
	public partial class UnitController
	{

		public void OnSkill()
		{
			ExecuteSkill(skillTargets, skillTargetCount);
		}

		/** Game Cycle Events **/
		public void OnEndStage()
		{
			gameObject.SetActive(true);
			InitCombat();

			OnStopTargetting();
			isInGame = false;
		}
		public void OnStartStage()
		{
			isInGame = true;
		}

	}
}
