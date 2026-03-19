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
	}
}
