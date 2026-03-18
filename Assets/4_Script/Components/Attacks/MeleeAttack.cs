using UnityEngine;

namespace Defense.Components
{
	public class MeleeAttack : Attackable
	{
		public override void Attack(Transform target)
		{
			if (target == null || !target.TryGetComponent<Damagable>(out var damagable)) return;

			damagable.GetImmediateDamage(stat.DamageType, stat.CurrentAtk);
		}
	}
}
