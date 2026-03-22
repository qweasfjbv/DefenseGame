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
	[RequireComponent(typeof(Damagable))]
	public class BuildingBase : MonoBehaviour
		, IResettable
    {
		[SerializeField] protected BuildingDataBase buildingData;

		private Damagable damagable = null;

		private StatContainer statContainer;

		private void Awake()
		{
			damagable = GetComponent<Damagable>();

			statContainer = new StatContainer();
			statContainer.AddStat<HealthStat>(new HealthStat(buildingData.StatsByLevel[0].MaxHealth));
			statContainer.AddStat<DefenseStat>(new DefenseStat(buildingData.StatsByLevel[0].DefensePower));

			damagable.OnDead += OnDead;
		}

		private void Start()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);

			damagable.Init(statContainer);
		}

		protected virtual void OnDisable()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

		/** Dying System **/
		protected virtual void OnDead()
		{
			gameObject.SetActive(false);
		}


		/** IResettable Interface **/
		public void OnGameReset()
		{

		}

		public void OnWaveReset()
		{

		}
	}
} 