using Defense.Utils;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Defense.Components
{
	public interface IStat 
	{
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

		public float CurrentMP => currentMP;
		public Stat MaxMP => maxMP;

		public bool IsAbleToUseSkill => currentMP >= maxMP.Value;

		public ManaStat(float maxMP)
		{
			this.maxMP = new Stat(maxMP);
			this.currentMP = maxMP;
		}

		public void Reset()
		{
			maxMP.Reset();
			currentMP = maxMP.Value;
		}

		public void OnUseSkill()
		{
			currentMP = 0f;
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
	}
}
