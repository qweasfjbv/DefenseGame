using Cysharp.Threading.Tasks;
using Defense.Controller;
using Defense.Manager;
using Defense.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Defense.Components
{
	public class Damagable : MonoBehaviour
	{
		private StatBase stat = null;

		private SortedDictionary<ReservationKey, DamageReservation> reservedDamage = new();

		private int damageId = 0;
		private bool isTargetFlagDirty = true;
		private bool isAbleToTargeted = false;

		private event Action onDead;
		private event Action onDamaged;

		public Action OnDead { get => onDead; set => onDead = value; }
		public Action OnDamaged { get => onDamaged; set => onDamaged = value; }

		public void Init(StatBase stat)
		{
			this.stat = stat;
			damageId = 0;
			isTargetFlagDirty = true;
		}

		public bool IsAbleToTargeted(float duration)
		{
			if (stat.IsDied) return false;
			// TODO - 리턴 정확한 값으로 수정 필요
			if (!isTargetFlagDirty) return isAbleToTargeted;

			float tmpHP = stat.CurrentHP;
			float lastTime = 0;

			foreach (var res in reservedDamage)
			{
				tmpHP -= Calculation.CalculateDamage(stat.CurrentDef, res.Value.Type, res.Value.Damage);
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
			if (stat.IsDied) return;

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
					GetImmediateDamage(type, damage);
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

		public void GetImmediateDamage(DamageType type, float damage)
		{
			if (stat.IsDied) return;

			float trueDamage = stat.GetDamage(type, damage);
			UIManager.GameUI.ShowDamage(transform.position + Vector3.up * 1.8f, trueDamage, type, HitResultType.Normal);
			CheckIfDied();
			onDamaged?.Invoke();
		}

		public void CheckIfDied()
		{
			if (stat.IsDied)
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
