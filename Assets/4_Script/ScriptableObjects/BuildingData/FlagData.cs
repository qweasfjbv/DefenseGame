using Defense.Utils;
using UnityEngine;

namespace Defense
{
	[CreateAssetMenu(fileName = "FlagData", menuName = "GameData/Flag Data")]
	public class FlagData : BuildingDataBase
	{
		[Header("Flag Data")]

		[SerializeField] private StatType buffType;
		[SerializeField] private float buffValue;
		[SerializeField] private float buffRange;

		public StatType BuffType => buffType;
		public float BuffValue => buffValue;
		public float BuffRange => buffRange;
	}
}
