using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

namespace Defense.Components
{
	public abstract class Skillable : MonoBehaviour
	{
		private const int MAX_SKILL_TARGET_COUNT = 10;
		
		protected ManaStat manaStat = null;

		protected List<Transform> skillTargets;
		protected int skillTargetCount;


		private bool isSkilling = false;

		public bool IsSkilling => isSkilling;   // 스킬 duration 체크
												// HACK
		public bool IsAbleToUseSkill => manaStat.CurrentMP >= manaStat.MaxMP.Value;


        public abstract void ExecuteSkill(List<Transform> targets, int targetCount);

		public void Init(StatContainer statContainer)
		{
			if (!statContainer.TryGet(out manaStat)) Debug.LogWarning("Mana Stat doesn't exists");

			skillTargets = new List<Transform>(MAX_SKILL_TARGET_COUNT);
			skillTargetCount = 0;
		}

		public void OnAttack()
		{
			manaStat.RecoverMP(manaStat.MpPerAttack.Value);
		}

		// 타겟 설정 함수
		public void AddTarget(Transform target)
		{
			if (skillTargets.Contains(target)) return;

			if(skillTargetCount < MAX_SKILL_TARGET_COUNT)
			{
				skillTargets.Add(target);
                skillTargetCount++;
			}
		}

		/** Animation Events **/
		public void OnSkill()
		{
            if (skillTargetCount <= 0) return;

			Debug.Log("곰곰히 스킬 써");

            isSkilling = true;

			manaStat.OnUseSkill();
			ExecuteSkill(skillTargets, skillTargetCount);
        }

		public void OnEndSkill()
		{
			Debug.Log("스킬 끝나써");
			isSkilling = false;
		}
	}
}
