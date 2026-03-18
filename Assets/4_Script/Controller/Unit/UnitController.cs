using Defense.Components;
using Defense.Interfaces;
using Defense.Manager;
using Defense.Props;
using Defense.Utils;
using DG.Tweening;
using IUtil;
using UnityEngine;

namespace Defense.Controller
{
	[RequireComponent(typeof(UnitStat))]
	[RequireComponent(typeof(Damagable))]
	[RequireComponent(typeof(Attackable))]
	public partial class UnitController : MonoBehaviour
		,ISkillable
	{
		/** Components **/
		private Animator animator = null;

		private UnitStat unitStat = null;
		private Damagable damagable = null;
		private Attackable attackable = null;

		/** SO Datas **/
		protected UnitData unitData = null;

		/** Target Infos **/
		private Collider[] targets;
		private Transform targetTransform = null;
		private Vector3 targetPosition = Vector3.zero;

		/** Pre-load Variables **/
		private PlacementSlot mySlot = null;
		public PlacementSlot MySlot { get => mySlot; set => mySlot = value; }

		private int targetLayer = 0;
		private int mySlotID = -1;

		private float attackClipLength = 0f;
		private float damagedClipLength = 0f;
		private float deathClipLength = 0f;
		private float skillClipLength = 0f;

		private int animIDAttack = 0;
		private int animIDAttackMT = 0;
		private int animIDDamagedMT = 0;
		private int animIDDeathMT = 0;
		private int animIDSpeed = 0;
		private int animIDDamaged = 0;
		private int animIDDeath = 0;
		private int animIDSkill = 0;
		private int animIDSkillMT = 0;

		/** State Variables **/
		private bool isChasing = false;
		private bool isInGame = false;          // Wait for game start

		public virtual bool IsSameUnit(int unitId, int rarity) { return false; }
		protected virtual void ExecuteSkill(Transform[] targets, int targetCounts) { }

		private void Awake()
		{
			animator = GetComponent<Animator>();
			unitStat = GetComponent<UnitStat>();
			damagable = GetComponent<Damagable>();
			attackable = GetComponent<Attackable>();

			damagable.Init(unitStat);
			attackable.Init(unitStat);

			attackClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_ATTACK);
			damagedClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DAMAGE);
			deathClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_DEATH);
			skillClipLength = animator.GetAnimationClipLength(Constants.ANIM_NAME_SKILL);

			animIDAttack = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK);
			animIDAttackMT = Animator.StringToHash(Constants.ANIM_PARAM_ATTACK_MT);
			animIDDamagedMT = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED_MT);
			animIDDeathMT = Animator.StringToHash(Constants.ANIM_PARAM_DEATH_MT);
			animIDSpeed = Animator.StringToHash(Constants.ANIM_PARAM_SPEED);
			animIDDamaged = Animator.StringToHash(Constants.ANIM_PARAM_DAMAGED);
			animIDDeath = Animator.StringToHash(Constants.ANIM_PARAM_DIED);
			animIDSkill = Animator.StringToHash(Constants.ANIM_PARAM_SKILL);
			animIDSkillMT = Animator.StringToHash(Constants.ANIM_PARAM_SKILL_MT);

			damagable.OnDamaged += ApplyKnockback;
			damagable.OnDead += OnDead;
		}

		[Header("DEBUG")]
		[ReadOnly] public int enemyId;

		private void Update()
		{
			if (!isInGame) return;

			if (unitData == null) return;
			UpdateKnockbackRemainedTime();

			if (IsKnockBack || unitStat.IsDied) return;
			OnUpdateUnit();
		}

		/// <summary>
		/// Update logics for walking/chasing on the ground
		/// </summary>
		public void OnUpdateUnit()
		{
			CheckNearbyTarget();

			if (attackable.IsAttacking)
			{
				if (!attackable.IsAbleToAttack) return;

				if (unitStat.IsAbleToUseSkill)
				{
					StartSkillAnim();
				}
				else
				{
					StartAttackAnim(targetTransform);
				}
			}
			else if (isChasing)
			{
				ChaseTarget();
			}
		}
		public void OnStopTargetting()
		{
			attackable.IsAttacking = false;
			isChasing = false;
		}

		/// <summary>
		/// Unit을 초기화합니다.
		/// </summary>
		public void InitUnit(int unitId)
		{
			unitData = Managers.Resource.GetUnitData(unitId);
			unitStat.CacheStatData(unitData, 0);

			targets = new Collider[unitData.MaxDetectCounts];
		}
		public void SetPlayerTeam(int playerIdx, int slotID)
		{
			mySlotID = slotID;
			
			Quaternion lookRot = Quaternion.LookRotation(new Vector3(0, 0, playerIdx == 0 ? 1 : -1));
			base.transform.rotation = lookRot;

			if (playerIdx == 0)
			{
				gameObject.layer = Constants.INTLAYER_PLAYER_1;
				targetLayer = Constants.LAYER_PLAYER_2;
			}
			else if (playerIdx == 1)
			{
				gameObject.layer = Constants.INTLAYER_PLAYER_2;
				targetLayer = Constants.LAYER_PLAYER_1;
			}
			else Debug.LogError("Unit Initizlization - wrong parameter \n `playerIdx` must be 0 or 1.");
		}

		private int targetCounts = 0;
		private void CheckNearbyTarget()
		{
			targetCounts = Physics.OverlapSphereNonAlloc(base.transform.position, unitData.SearchRange, targets, targetLayer);
			if (targetCounts > 0)
			{
				float minDistance = float.MaxValue;
				Transform closestTarget = null;

				for (int i = 0; i < targetCounts; i++)
				{
					if (targets[i] == null) break;
					if (!targets[i].TryGetComponent<Damagable>(out var target) ||
						!target.IsAbleToTargeted(unitData.AttackDelay))	continue;
					float distance = Vector3.SqrMagnitude(base.transform.position - targets[i].transform.position);

					if (distance < minDistance)
					{
						minDistance = distance;
						closestTarget = targets[i].transform;
					}
				}

				targetTransform = closestTarget;
				if (targetTransform == null) return;
				isChasing = true;
			}
			else
			{
				targetTransform = null;
				isChasing = false;
			}

			if (targetTransform != null && Vector3.SqrMagnitude(base.transform.position- targetTransform.position) <= unitData.AttackRange * unitData.AttackRange)
			{
				attackable.IsAttacking = true;
				isChasing = false;
			}
		}
		private void ChaseTarget()
		{
			if (targetTransform == null)
			{
				isChasing = false;
				return;
			}

			Vector3 dir = targetTransform.position - transform.position;
			dir.y = 0;

			Quaternion targetRotation = Quaternion.LookRotation(dir);
			transform.rotation = Quaternion.Lerp(base.transform.rotation, targetRotation, unitData.RotationSpeed * Time.deltaTime);
			transform.position = Vector3.MoveTowards(base.transform.position, targetTransform.position, unitData.MoveSpeed * Time.deltaTime);

			GetComponent<Animator>().SetFloat(animIDSpeed, (targetTransform.position - transform.position).AbsSum());
		}

		private void OnDrawGizmos()
		{
			if (unitData == null) return;

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, unitData.SearchRange);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(base.transform.position, unitData.AttackRange);
		}

		public static float hoverHeight = 1f;
		public static float hoverDuration = 0.2f;
		public static float moveDuration = 0.2f;

		private Tween currentTween = null;
		private bool isDragging = false;

		public void PickUp(float baseHeight)
		{
			if (currentTween != null) currentTween.Kill();

			currentTween = transform.DOMoveY(baseHeight + hoverHeight, hoverDuration)
				.SetEase(Ease.OutQuad);
		}
		public void DropTo(Vector3 targetSlotPos)
		{
			isDragging = false;

			if (currentTween != null) currentTween.Kill();
			base.transform.position = new Vector3(targetSlotPos.x, targetSlotPos.y + hoverHeight, targetSlotPos.z);

			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOMoveY(targetSlotPos.y, hoverDuration).SetEase(Ease.InQuad));
			currentTween = seq;
		}

	}
}
