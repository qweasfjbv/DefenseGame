using UnityEngine;

namespace Defense.Components
{
	public abstract class Attackable : MonoBehaviour
	{
		protected StatBase stat = null;
		protected Transform attackTarget = null;

		private bool isAttacking = false;
		private float currentAttackCooltime = 0f;

		public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }
		public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
		public bool IsAbleToAttack => currentAttackCooltime < 0f;

		public abstract void Attack(Transform target);

		public void Init(StatBase stat)
		{
			this.stat = stat;
		}

		private void Update()
		{
			if (currentAttackCooltime >= 0f)
				currentAttackCooltime -= Time.deltaTime;
		}

		public void ResetCooltime()
		{
			currentAttackCooltime = stat.AttackCooltime;
		}

		/** Animation Events **/
		public void OnAttack()
		{
			if (attackTarget == null) return;
			Attack(attackTarget);
			stat.OnAttack();
		}

		public void OnEndAttack()
		{
			isAttacking = false;
		}
	}
}
