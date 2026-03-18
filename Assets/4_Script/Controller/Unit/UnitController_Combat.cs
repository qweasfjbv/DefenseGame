using Cysharp.Threading.Tasks;
using Defense.Manager;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	/// <summary>
	/// UnitController의 전투 관련 인터페이스 구현 및 함수 구현
	/// </summary>
	public partial class UnitController
	{
		private int skillTargetCount = 0;
		private Transform[] skillTargets = new Transform[10];

		private void InitCombat()
		{
			animator.SetBool(animIDDeath, false);
			unitStat.CacheStatData(unitData, 0);
		}
		public void StartAttackAnim(Transform target)
		{
			if (target == null)
			{
				OnStopTargetting();
				return;
			}

			transform.LookAt(target);
			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDAttack);
			animator.SetFloat(animIDAttackMT, attackClipLength / unitData.AttackCooltime);

			attackable.AttackTarget = target;
			attackable.ResetCooltime();
		}

		/** ISkillable Interface **/
		public bool IsAbleToUseSkill()
		{
			// LEGACY
			return false;
		}
		public void StartSkillAnim()
		{
			if (targetTransform == null)
			{
				OnStopTargetting();
				return;
			}

			// HACK - 임시 스킬 테스트용
			skillTargets[0] = targetTransform;
			skillTargetCount = 1;

			base.transform.LookAt(targetTransform);
			animator.SetFloat(animIDSpeed, 0);
			animator.SetTrigger(animIDSkill);
			animator.SetFloat(animIDSkillMT, skillClipLength / unitData.SkillDuration);
			
			unitStat.OnUseSkill();
		}

		/** Dying System **/
		private void OnDead()
		{
			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDeathMT, deathClipLength / unitData.DeathAnimDuration);
			animator.SetBool(animIDDeath, true);

			GetComponent<Outline>().OffOutline();

			DelayedDestroy(unitData.DeathAnimDuration, unitData.FadeOutDuration).Forget();
		}
		public async UniTask DelayedDestroy(float deathDuration, float fadeDuration)
		{
			try
			{
				await UniTask.Delay((int)(deathDuration * 1000));
				// TODO - FadeOut
				await UniTask.Delay((int)(fadeDuration * 1000));
				gameObject.SetActive(false);
			}
			catch (System.OperationCanceledException)
			{
				Debug.Log("Delay was cancelled");
			}
		}

		/** Knockback System **/
		private bool IsKnockBack { get => unitData.UseKnockback && knockbackRemainedTime > Mathf.Epsilon; }
		private float knockbackRemainedTime = 0f;
		private void ApplyKnockback()
		{
			knockbackRemainedTime = unitData.KnockbackDuration;

			animator.SetFloat(animIDSpeed, 0f);
			animator.SetFloat(animIDDamagedMT, damagedClipLength / knockbackRemainedTime);
			animator.SetTrigger(animIDDamaged);
			PoolingManager.Instance.SpawnParticle(ParticleType.Hit, transform.position);
		}
		private void UpdateKnockbackRemainedTime()
		{
			if (IsKnockBack)
				knockbackRemainedTime -= Time.deltaTime;

			animator.SetLayerWeight(1, knockbackRemainedTime > Mathf.Epsilon ? 1f : 0f);
		}
	}
}
