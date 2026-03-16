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

	[CreateAssetMenu(fileName = "FlagData", menuName = "GameData/Building Data")]
	public class FlagData : BuildingDataBase
	{
		[Header("Flag Data")]

		[SerializeField] private BuffType buffType;
		[SerializeField] private float buffValue;
		[SerializeField] private float buffRange;

		public BuffType BuffType => buffType;
		public float BuffValue => buffValue;
		public float BuffRange => buffRange;
	}

	[CreateAssetMenu(fileName = "WallData", menuName = "GameData/Wall Data")]
	public class WallData : BuildingDataBase
	{
		// Nothing to add
	}
}
