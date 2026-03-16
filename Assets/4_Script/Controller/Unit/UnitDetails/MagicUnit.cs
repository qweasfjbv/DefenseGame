using Defense.Components;
using Defense.Interfaces;
using Defense.Manager;
using Defense.VFX;
using UnityEngine;

namespace Defense.Controller
{
	public class MagicUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || !target.TryGetComponent<IGetComponent<Damagable>>(out var damagable)) return;
			damagable.GetComponent().ReserveDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower, unitData.AttackDelay);

			TrailBase tb = PoolingManager.Instance.Spawn(Utils.ProjectileType.Lightning, unitData.AttackDelay).GetComponent<TrailBase>();
			tb.SetTrail(transform.position, target, unitData.AttackDelay);
		}

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 2;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{
			// HACK - 스킬 테스트용
			if (targets == null || targets[0] == null) return;
			targets[0].GetComponent<IGetComponent<Damagable>>().GetComponent().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
			PoolingManager.Instance.SpawnParticle(Utils.ParticleType.Lightning, targets[0].position);
		}
	}
}
