using UnityEngine;

namespace Defense.Components
{
	public class BuildingStat : StatBase
	{
		public override bool IsAbleToUseSkill => false;

		public override void CacheStatData(in ScriptableObject data, int level)
		{
			if (data is not BuildingDataBase buildingData) return;

			currentHP = buildingData.StatsByLevel[level].MaxHealth;
			currentAtk = buildingData.StatsByLevel[level].AttackPower;
			currentDef = buildingData.StatsByLevel[level].DefensePower;
		}
	}
}
