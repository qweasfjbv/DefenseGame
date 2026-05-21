using Defense.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Components
{
	public interface IStat 
	{
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict);
		public void Reset();
	}

	/// <summary>
	/// 각 Stat의 버프/디버프 정보를 나타냄
	/// 우선, 곱연산만 가능하도록 설계함
	/// 필요시 추가 => Stat의 Calculate 도 수정 필요
	/// </summary>
	public class Modifier
	{
		private float value;
		private object source;

		public float Value => value;
		public object Source => source;

		public Modifier(object source, float value)
		{
			this.source = source;
			this.value = value;
		}
	}

	/// <summary>
	/// 버프에 의해 변경되는 스탯을 나타내는 클래스 (ex. Atk, Def, speed ..)
	/// </summary>
	public class Stat
	{
		private float value = 0f;
		public float Value => Calculate();

		private bool isDirty = true;
		private float cachedMultplier = 1f;
		private List<Modifier> modifiers = new();

		public Stat(float value)
		{
			this.value = value;
		}

		public void Reset() 
		{
			isDirty = false;
			cachedMultplier = 1f;
			modifiers.Clear();
		}

		public void AddModfier(object source, float value)
		{
			isDirty = true;
			modifiers.Add(new Modifier(source, value));
		}

		public void RemoveModifierBySource(object source)
		{
			isDirty = true;
			modifiers.RemoveAll(m => m.Source == source);
		}

		private float Calculate()
		{
			if (!isDirty) return value * cachedMultplier;

			isDirty = false;
			cachedMultplier = 1f;

			foreach (Modifier modifier in modifiers)
			{
				cachedMultplier *= modifier.Value;
			}

			return value * cachedMultplier;
		}
	}


	public class HealthStat : IStat
	{
		private float currentHP;
		private Stat maxHP;

		public float CurrentHP => currentHP;
		public Stat MaxHP => maxHP;

		public bool IsDead => currentHP <= 0f;

		public HealthStat(float maxHP)
		{
			this.maxHP = new Stat(maxHP);
			this.currentHP = maxHP;
		}
		public void Reset()
		{
			maxHP.Reset();
			currentHP = maxHP.Value;
		}
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict)
		{
			targetToStatDict.Add(StatType.MaxHP, maxHP);
		}

		public void TakeTrueDamage(float damage)
		{
			currentHP -= damage;
			currentHP = Mathf.Max(currentHP, 0f);
		}

		public void Heal(float amount)
		{
			currentHP += amount;
			currentHP = Mathf.Min(currentHP, maxHP.Value);
		}

	}

	public class ManaStat : IStat
	{
		private float currentMP;
		private Stat maxMP;
		private Stat mpPerAttack;
		

		public float CurrentMP => currentMP;
		public Stat MaxMP => maxMP;
		public Stat MpPerAttack => mpPerAttack;
		public bool IsAbleToUseSkill => currentMP >= maxMP.Value;

		public ManaStat(float maxMP, float mpPerAttack)
		{
			this.maxMP = new Stat(maxMP);
			this.mpPerAttack = new Stat(mpPerAttack);
			this.currentMP = 0f;
		}

		public void Reset()
		{
			maxMP.Reset();
			mpPerAttack.Reset();
			currentMP = maxMP.Value;
		}
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict)
		{
			targetToStatDict.Add(StatType.MaxMP, maxMP);
		}


		public void OnUseSkill()
		{
			currentMP = 0f;
		}

		public void RecoverMP(float amount)
		{
			currentMP += amount;
			currentMP = Mathf.Min(currentMP, maxMP.Value);
		}
	}

	public class AttackStat : IStat
	{
		private DamageType damageType;
		private Stat currentAttack;
		private Stat currentAttackCooltime;
		private Stat currentAttackDelay;

		public DamageType DamageType => damageType;
		public Stat CurrentAttack => currentAttack;
		public Stat CurrentAttackCooltime => currentAttackCooltime;
		public Stat CurrentAttackDelay => currentAttackDelay;

		public AttackStat(DamageType damageType, float attack, float attackCooltime, float attackDelay)
		{
			this.damageType = damageType;
			this.currentAttack = new Stat(attack);
			this.currentAttackCooltime = new Stat(attackCooltime);
			this.currentAttackDelay = new Stat(attackDelay);
		}

		public void Reset()
		{
			currentAttack.Reset();
			currentAttackCooltime.Reset();
			currentAttackDelay.Reset();
		}
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict)
		{
			targetToStatDict.Add(StatType.AttackPower, currentAttack);
			targetToStatDict.Add(StatType.AttackCooltime, currentAttackCooltime);
			targetToStatDict.Add(StatType.AttackDelay, currentAttackDelay);
		}

	}

	public class DefenseStat : IStat
	{
		private Stat currentDefense;

		public Stat CurrentDefense => currentDefense;

		public DefenseStat(float defense)
		{
			this.currentDefense = new Stat(defense);
		}

		public void Reset()
		{
			currentDefense.Reset();
		}
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict)
		{
			targetToStatDict.Add(StatType.DefensePower, currentDefense);
		}

	}

	public class MovementStat : IStat
	{
		private Stat currentSpeed;

		public Stat CurrentSpeed => currentSpeed;

		public MovementStat(float speed)
		{
			this.currentSpeed = new Stat(speed);
		}

		public void Reset()
		{
			currentSpeed.Reset();
		}
		public void RegisterStat(ref Dictionary<StatType, Stat> targetToStatDict)
		{
			targetToStatDict.Add(StatType.MoveSpeed, currentSpeed);
		}

	}
}
