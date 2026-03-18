using UnityEngine;

namespace Defense.Components
{
	public class UnitStat : StatBase
	{
		public override void CacheStatData(in ScriptableObject data, int level)
		{
			if (data is not UnitData unitData) return;

			currentHP = unitData.StatsByLevel[level].MaxHealth;
			currentAtk = unitData.StatsByLevel[level].AttackPower;
			currentDef = unitData.StatsByLevel[level].DefensePower;
			currentMP = 0f;
			maxMP = unitData.StatsByLevel[level].MaxMP;

			damageType = unitData.DamageType;
			attackCooltime = unitData.AttackCooltime;
			attackDelay = unitData.AttackDelay;
		}
	}
}
