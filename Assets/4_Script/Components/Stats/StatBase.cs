using Defense.Utils;
using IUtil;
using UnityEngine;

namespace Defense.Components
{
	/// <summary>
	/// 
	/// - 스탯 관리
	/// - 버프/디버프 등 최종 스탯 내부에서 계산
	/// - 디버깅 용이 및 추가 기능을 위해 컴포넌트로 제작
	/// 
	/// TODO - speed, attackRange 등 버프에 영향받을 능력치 추가 필요
	/// 
	/// </summary>
	public abstract class StatBase : MonoBehaviour
	{
		[SerializeField, ReadOnly] protected float currentHP = 0f;
		[SerializeField, ReadOnly] protected float currentAtk = 0f;
		[SerializeField, ReadOnly] protected float currentDef = 0f;
		[SerializeField, ReadOnly] protected float currentMP = 0f;
		[SerializeField, ReadOnly] protected float maxMP = float.MaxValue;
		[SerializeField, ReadOnly] protected float attackCooltime = 0f;
		[SerializeField, ReadOnly] protected DamageType damageType;
		[SerializeField, ReadOnly] protected float attackDelay = 0f;

		public virtual float CurrentHP => currentHP;
		public virtual float CurrentAtk => currentAtk;
		public virtual float CurrentDef => currentDef;
		public virtual float CurrentMP => currentMP;
		public virtual float MaxMP => maxMP;
		public virtual float AttackCooltime => attackCooltime;
		public virtual DamageType DamageType => damageType;
		public virtual float AttackDelay => attackDelay;


		public virtual bool IsDied => currentHP <= 0f;
		public virtual bool IsAbleToUseSkill => currentMP >= maxMP;

		public abstract void CacheStatData(in ScriptableObject data, int level);

		public void OnAttack()
		{
			// TODO - 떄릴 때마다 MP 증가
		}

		public void OnUseSkill()
		{
			currentMP = 0f;
		}

		public float GetDamage(DamageType type, float damage)
		{
			float trueDamage = Calculation.CalculateDamage(CurrentDef, type, damage);

			currentHP -= trueDamage;
			return trueDamage;
		}

		public void GetTrueDamage(float trueDamage)
		{
			currentHP -= trueDamage;
		}
	}
}
