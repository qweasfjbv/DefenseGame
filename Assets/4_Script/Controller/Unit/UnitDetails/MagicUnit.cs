using Defense.Components;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class MagicUnit : UnitController
	{
		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 2;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{
			// HACK - 스킬 테스트용
			if (targets == null || targets[0] == null) return;
			targets[0].GetComponent<Damagable>().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
			PoolingManager.Instance.SpawnParticle(Utils.ParticleType.Lightning, targets[0].position);
		}
	}
}
