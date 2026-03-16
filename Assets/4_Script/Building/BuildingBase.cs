using Defense.Components;
using Defense.Interfaces;
using Defense.Manager;
using Defense.Utils;
using UnityEngine;

namespace Defense.Building
{
	/// <summary>
	/// 
	/// 건축 가능한 건물의 베이스 클래스
	/// - ex) 벽, 깃발...
	/// 
	/// - 파괴 가능
	/// 
	/// </summary>
	public class BuildingBase : MonoBehaviour
		, IGetComponent<Damagable>
		, IResettable
    {
		[SerializeField] private BuildingDataBase buildingData;

		private float currentHP = 0f;
		private float currentDef = 0f;

		protected virtual void Awake()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

		protected virtual void OnDisable()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

		public Damagable GetComponent()
		{
			// TODO 
			return null;
		}

		/** IResettable Interface **/
		public void OnGameReset()
		{
			throw new System.NotImplementedException();
		}

		public void OnWaveReset()
		{
			throw new System.NotImplementedException();
		}

	}
}