using UnityEngine;

namespace Defense.Components
{
	public abstract class Attackable : MonoBehaviour
	{
		protected AttackStat attackStat = null;

		protected Transform attackTarget = null;
		private bool isAbleToAttack = false;
		private float remainedAttackCooltime = 0f;

		public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }
		// 타겟이 공격 범위 안에 들어올 경우 true
		public bool IsAbleToAttack { get => isAbleToAttack; set => isAbleToAttack = value; }
		public bool IsAttacking => remainedAttackCooltime > 0f;

		public abstract void Attack(Transform target);

		public void Init(StatContainer statContainer)
		{
			if(!statContainer.TryGet(out attackStat)) Debug.LogWarning("AttackStat doesn't exist!");
		}

		private void Update()
		{
			if (remainedAttackCooltime >= 0f)
				remainedAttackCooltime -= Time.deltaTime;
		}

		public void ResetCooltime()
		{
			remainedAttackCooltime = attackStat.CurrentAttackCooltime.Value;
		}

		/** Animation Events **/
		public void OnAttack()
		{
			if (attackTarget == null) return;
			
			Attack(attackTarget);
			// TODO - 마나 증가, 필요시
		}

		public void OnEndAttack()
		{
			isAbleToAttack = false;
		}
	}
}
