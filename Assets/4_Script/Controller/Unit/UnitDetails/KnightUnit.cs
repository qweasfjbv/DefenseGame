using Defense.Components;
using Defense.Interfaces;
using UnityEngine;

namespace Defense.Controller
{
	public class KnightUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || !target.TryGetComponent<IGetComponent<Damagable>>(out var damagable)) return;

			damagable.GetComponent().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
		}

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 0;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}

	}
}