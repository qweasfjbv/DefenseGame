using Defense.Utils;
using UnityEngine;

namespace Defense
{
	[CreateAssetMenu(fileName = "EnemyData", menuName = "GameData/Enemy Data")]
	public class UnitData : ScriptableObject
	{
		[Header("Movement")]
		[SerializeField] private float moveSpeed;
		[SerializeField] private float rotationSpeed;

		[Header("Attack")]
		[SerializeField] private DamageType damageType;
		[SerializeField] private float searchRange;
		[SerializeField] private float attackRange;
		[SerializeField] private float attackCooltime;
		[SerializeField] private float attackDelay;
		[SerializeField] private float mpPerAttack;

		[Header("Knockback")]
		[SerializeField] private bool useKnockback;
		[SerializeField] private float knockbackDuration;

		[Header("Detect Settings")]
		[SerializeField] private LayerMask targetLayer;
		[SerializeField] private int maxDetectCounts;

		[Header("Durations")]
		[SerializeField] private float deathAnimDuration;
		[SerializeField] private float fadeOutDuration;
		[SerializeField] private float skillDuration;

		[Header("Stats")]
		[SerializeField] private LevelStat[] statsByLevel = new LevelStat[Constants.ENEMY_LEVEL_MAX];

		// ReadOnly Properties
		public float MoveSpeed => moveSpeed;
		public float RotationSpeed => rotationSpeed;
		public DamageType DamageType => damageType;
		public float SearchRange => searchRange;
		public float AttackRange => attackRange;
		public float AttackCooltime => attackCooltime;
		public float AttackDelay => attackDelay;
		public float MPPerAttack => mpPerAttack;
		public bool UseKnockback => useKnockback;
		public float KnockbackDuration => knockbackDuration;
		public LayerMask TargetLayer => targetLayer;
		public int MaxDetectCounts => maxDetectCounts;
		public float DeathAnimDuration => deathAnimDuration;
		public float FadeOutDuration => fadeOutDuration;
		public float SkillDuration => skillDuration;
		public LevelStat[] StatsByLevel => statsByLevel;
	}
}