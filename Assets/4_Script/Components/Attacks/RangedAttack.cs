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

			damagable.ReserveDamage(stat.DamageType, stat.CurrentAtk, stat.AttackDelay);
			TrailBase tb = PoolingManager.Instance.Spawn(projectileType, stat.AttackDelay).GetComponent<TrailBase>();
			tb.SetTrail(transform.position, target, stat.AttackDelay);
		}
	}
}
