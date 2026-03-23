using Defense.Utils;
using UnityEngine;

namespace Defense
{
	public class BuildingDataBase : ScriptableObject
	{
		[Header("Building Data")]
		[SerializeField] private LevelStat[] statsByLevel = new LevelStat[Constants.TOWER_LEVEL_MAX];

		public LevelStat[] StatsByLevel => statsByLevel;
	}
}
