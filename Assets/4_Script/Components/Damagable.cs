using Cysharp.Threading.Tasks;
using Defense.Manager;
using Defense.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Defense.Components
{
	public class ReservationKey : IComparable<ReservationKey>
	{
		private int dID;
		private float time;

		public float Time => time;

		public ReservationKey(int dID, float time)
		{
			this.dID = dID;
			this.time = time;
		}

		public int CompareTo(ReservationKey other)
		{
			if (other == null) return 1;

			int timeComparison = time.CompareTo(other.time);
			if (timeComparison != 0)
				return timeComparison;

			return dID.CompareTo(other.dID);
		}
	}

	public class DamageReservation
	{
		private float damage;
		private DamageType type;
		private CancellationTokenSource cancellationTokenSource;

		public float Damage => damage;
		public DamageType Type => type;
		public CancellationTokenSource CTS => cancellationTokenSource;

		public DamageReservation(CancellationTokenSource cancellationTokenSource, float damage, DamageType type)
		{
			this.cancellationTokenSource = cancellationTokenSource;
			this.damage = damage;
			this.type = type;
		}
	}

	public class Damagable : MonoBehaviour
	{
		private HealthStat healthStat = null;
		private DefenseStat defenseStat = null;

		private SortedDictionary<ReservationKey, DamageReservation> reservedDamage = new();

		private int damageId = 0;
		private bool isTargetFlagDirty = true;
		private bool isAbleToTargeted = false;

		private event Action onDead;
		private event Action onDamaged;

		public bool IsDead  => healthStat.IsDead;
		public Action OnDead { get => onDead; set => onDead = value; }
		public Action OnDamaged { get => onDamaged; set => onDamaged = value; }

		public void Init(StatContainer statContainer)
		{
			if (!statContainer.TryGet(out healthStat)) Debug.LogWarning("Health Stat doesn't exists");
			if (!statContainer.TryGet(out defenseStat)) Debug.LogWarning("Defense Stat doesn't exists");

			damageId = 0;
			isTargetFlagDirty = true;
		}

		public bool IsAbleToTargeted(float duration)
		{
			if (healthStat.IsDead) return false;
			// TODO - 리턴 정확한 값으로 수정 필요
			if (!isTargetFlagDirty) return isAbleToTargeted;

			float tmpHP = healthStat.CurrentHP;
			float lastTime = 0;

			foreach (var res in reservedDamage)
			{
				tmpHP -= Calculation.CalculateDamage(defenseStat.CurrentDefense.Value, res.Value.Type, res.Value.Damage);
				if (tmpHP <= 0f)
				{
					lastTime = res.Key.Time;
					break;
				}
			}

			isAbleToTargeted = (tmpHP > 0f || (tmpHP <= 0f && Time.time + duration < lastTime));
			isTargetFlagDirty = false;
			return isAbleToTargeted;
		}
		
		public void ReserveDamage(DamageType type, float damage, float duration)
		{
			if (healthStat.IsDead) return;

			var cts = new CancellationTokenSource();
			ReservationKey resKey = new ReservationKey(++damageId, Time.time + duration);
			reservedDamage[resKey] = new DamageReservation(cts, damage, type);
			isTargetFlagDirty = true;

			DelayedDamage(type, damage, duration, resKey).Forget();
		}

		/// <summary>
		/// Delay 된 데미지를 입히는 함수
		/// 취소 시 catch 부분 실행됨
		/// </summary>
		private async UniTaskVoid DelayedDamage(DamageType type, float damage, float duration, ReservationKey resKey)
		{
			try
			{
				await UniTask.Delay((int)(duration * 1000));

				if (!reservedDamage[resKey].CTS.IsCancellationRequested)
					TakeImmediateDamage(type, damage);
			}
			catch (OperationCanceledException)
			{
				// Cancelled
			}
			finally
			{
				reservedDamage.Remove(resKey);
			}
		}

		public void TakeImmediateDamage(DamageType type, float damage)
		{
			if (healthStat.IsDead) return;

			float trueDamage = Calculation.CalculateDamage(defenseStat.CurrentDefense.Value, type, damage);
			healthStat.TakeTrueDamage(trueDamage);
			UIManager.GameUI.ShowDamage(transform.position + Vector3.up * 1.8f, trueDamage, type, HitResultType.Normal);
			CheckIfDied();
			onDamaged?.Invoke();
		}

		public void CheckIfDied()
		{
			if (healthStat.IsDead)
			{
				onDead?.Invoke();
			}
		}

		private void OnDestroy()
		{
			foreach (var res in reservedDamage.Values)
				res.CTS.Cancel();
		}
	}
}
