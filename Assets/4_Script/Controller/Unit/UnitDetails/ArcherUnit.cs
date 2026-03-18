using UnityEngine;

namespace Defense.Controller
{
	public class ArcherUnit : UnitController
	{
		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 1;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}

	}
}
