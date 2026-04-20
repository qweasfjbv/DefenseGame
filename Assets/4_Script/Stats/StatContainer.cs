using Defense.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Components
{
	public interface IStatOwner
	{
		public StatContainer StatContainer { get; }
	}

	/// <summary>
	/// 
	/// - 스탯 관리
	/// - 버프/디버프 등 최종 스탯 내부에서 계산
	/// 
	/// </summary>
	public class StatContainer
	{
		private Dictionary<Type, IStat> stats = new();
		private Dictionary<StatType, Stat> targetToStatDict = new();

		public void Reset()
		{
			foreach(var stat in stats)
			{
				stat.Value.Reset();
			}
		}

		public void AddStat<T>(T stat) where T : IStat
		{
			if (stats.ContainsKey(typeof(T)))
				Debug.LogWarning("Already has same key.");

			stats[typeof(T)] = stat;
			(stat as IStat).RegisterStat(ref targetToStatDict);
		}

		public bool TryGet<T>(out T stat) where T : class, IStat
		{
			if (stats.TryGetValue(typeof(T), out var value))
			{
				stat = value as T;
				return true;
			}

			stat = null;
			return false;
		}

		public T Get<T>() where T : class, IStat
		{
			return stats[typeof(T)] as T;
		}

		public void ApplyBuff(object source, StatType type, float buffValue)
		{
			if (!targetToStatDict.TryGetValue(type, out var stat)) return;

			stat.AddModfier(source, buffValue);
		}

		// source가 적용한 모든 버프 제거
		public void RemoveBuff(object source)
		{
			foreach (var stat in targetToStatDict)
			{
				stat.Value.RemoveModifierBySource(source);
			}
		}
		
		public void RemoveBuff(object source, StatType type)
		{
			if (!targetToStatDict.TryGetValue(type, out var stat)) return;

			stat.RemoveModifierBySource(source);
		}
	}
}
