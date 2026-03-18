using UnityEngine;

namespace Defense.Controller
{
	public class KnightUnit : UnitController
	{
		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 0;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}

	}
}