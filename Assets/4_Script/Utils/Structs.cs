using System;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Utils
{
	/// <summary>
	/// 레벨마다 달라지는 Stat 구조체
	/// </summary>
	[System.Serializable]
	public struct LevelStat
	{
		public float MaxHealth;
		public float MaxMP;
		public float AttackPower;
		public float DefensePower;
		public float CritProb;
	}

	[System.Serializable]
	public class ParticleEntry
	{
		[HideInInspector] public Queue<ParticleSystem> Pool = new();

		public GameObject Prefab;
		public int PoolSize;
		public ParticleType Key;
	}

	[System.Serializable]
	public class ProjectileEntry
	{
		[HideInInspector] public Queue<GameObject> Pool = new();

		public GameObject Prefab;
		public int PoolSize;
		public ProjectileType Key;
	}

	public struct HitResult
	{
		public float FinalDamage;
		public HitResultType ResultType;
		public DamageType DamageType;

		public HitResult(HitResultType resultType, float finalDamage, DamageType damageType)
		{
			ResultType = resultType;
			FinalDamage = finalDamage;
			DamageType = damageType;
		}
	}

	public struct DamageLog
	{
		public int AtkSlotId;
		public int DefSlotId;
		public int AtkDamage;
	}

	public enum BridgeDirection { Horizontal, Vertical }

	public readonly struct BridgeKey : IEquatable<BridgeKey>
	{
		public readonly Vector2Int Index;
		public readonly BridgeDirection Direction;

		public BridgeKey(Vector2Int index, BridgeDirection direction)
		{
			Index = index;
			Direction = direction;
		}

		public static BridgeKey FromPair(Vector2Int a, Vector2Int b)
		{
			if (a.y == b.y)
			{
				Vector2Int min = a.x < b.x ? a : b;
				return new BridgeKey(min, BridgeDirection.Horizontal);
			}
			if (a.x == b.x)
			{
				Vector2Int min = a.y < b.y ? a : b;
				return new BridgeKey(min, BridgeDirection.Vertical);
			}
			throw new ArgumentException($"두 슬롯이 인접하지 않습니다: {a}, {b}");
		}

		public bool Equals(BridgeKey other) =>
			Index == other.Index && Direction == other.Direction;

		public override bool Equals(object obj) =>
			obj is BridgeKey other && Equals(other);

		public override int GetHashCode() =>
			HashCode.Combine(Index, Direction);

		public static bool operator ==(BridgeKey a, BridgeKey b) => a.Equals(b);
		public static bool operator !=(BridgeKey a, BridgeKey b) => !a.Equals(b);

		public override string ToString() => $"[{Index} | {Direction}]";
	}

}