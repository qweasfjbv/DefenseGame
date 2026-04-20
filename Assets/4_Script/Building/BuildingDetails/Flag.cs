using Defense.Components;

namespace Defense.Building
{
	/// <summary>
	/// 깃발 버프 관련 로직
	/// </summary>
	public class Flag : BuildingBase
	{
		private AuraEmitter auraEmitter = null;

		protected override void Awake()
		{
			base.Awake();

			auraEmitter = GetComponent<AuraEmitter>();
		}

		public override void OnStartStage()
		{
			base.OnStartStage();

			auraEmitter.OnStartStage();
		}

		public override void OnEndStage()
		{
			base.OnEndStage();

			auraEmitter.OnEndStage();
		}
	}
}
