using UnityEngine;

namespace Defense.Components
{
	public class Attackable : MonoBehaviour
	{
		private StatBase stat = null;

		private Transform attackTarget = null;

		/** IAttackable Interface **/
		public bool IsAbleToAttack()
		{
			return currentAttackCooltime < 0f;
		}
		public void StartAttackAnim()
		{
			if (targetTransform == null)
			{
				isAttacking = false;
				isChasing = false;
				return;
			}

			attackTarget = targetTransform;
			base.transform.LookAt(attackTarget);
			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDAttack);
			animator.SetFloat(animIDAttackMT, attackClipLength / unitData.AttackCooltime);

			currentAttackCooltime = unitData.AttackCooltime;
		}
		public void UpdateCooltimeTick()
		{
			if (currentAttackCooltime >= 0f)
				currentAttackCooltime -= Time.deltaTime;
		}
	}
}
