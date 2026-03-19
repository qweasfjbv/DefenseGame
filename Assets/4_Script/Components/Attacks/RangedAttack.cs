using Defense.Manager;
using Defense.Utils;
using Defense.VFX;
using UnityEngine;

namespace Defense.Components
{
	public class RangedAttack : Attackable
	{
		[SerializeField] private ProjectileType projectileType;

		public override void Attack(Transform target)
		{
			if (target == null || !target.TryGetComponent<Damagable>(out var damagable)) return;

			damagable.ReserveDamage(attackStat.DamageType, attackStat.CurrentAttack.Value, attackStat.CurrentAttackDelay.Value);
			TrailBase tb = PoolingManager.Instance.Spawn(projectileType, attackStat.CurrentAttackDelay.Value).GetComponent<TrailBase>();
			tb.SetTrail(transform.position, target, attackStat.CurrentAttackDelay.Value);
		}
	}
}
