using UnityEngine;

namespace Defense.Components
{
	public class Skillable : MonoBehaviour
	{
		private ManaStat manaStat = null;

		public bool IsSkilling => false;		// 스킬 duration 체크
		// HACK
		public bool IsAbleToUseSkill => false;	// 마나 체크

		public void Init(StatContainer statContainer)
		{
			if (!statContainer.TryGet(out manaStat)) Debug.LogWarning("Mana Stat doesn't exists");
		}

		public void OnUseSkill()
		{
			// TODO -
		}

		public void OnAttack()
		{
			// TODO -
		}
	}
}
