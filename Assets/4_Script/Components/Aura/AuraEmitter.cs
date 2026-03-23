using Defense.Controller;
using Defense.Utils;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Defense.Components
{
	[RequireComponent(typeof(SphereCollider))]
	public class AuraEmitter : MonoBehaviour
	{
		[SerializeField, ReadOnly] private StatType statType;
		[SerializeField, ReadOnly] private float buffValue;
		[SerializeField, ReadOnly] private float buffRange;
		[SerializeField] private LayerMask targetLayer;

		private new SphereCollider collider = null;
		private HashSet<UnitController> targets = new();

		private void Awake()
		{
			collider = GetComponent<SphereCollider>();
			collider.enabled = false;
		}

		public void Init(StatType type, float buffValue, float buffRange)
		{
			this.statType = type;
			this.buffValue = buffValue;
			this.buffRange = buffRange;

			collider.radius = buffRange;
		}

		public void OnStartStage()
		{
			collider.enabled = true;
		}

		public void OnEndStage()
		{
			collider.enabled = false;
			
			foreach(UnitController controller in targets)
			{
				controller.StatContainer.RemoveBuff(this);
			}
		}

		private void OnEnable()
		{
			// TODO - Initial Scan Needed
		}

		private void OnTriggerEnter(Collider other)
		{
			if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

			targets.Add(other.GetComponent<UnitController>());
			other.GetComponent<UnitController>().StatContainer.ApplyBuff(this, statType, buffValue);
		}

		private void OnTriggerExit(Collider other)
		{
			if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

			targets.Remove(other.GetComponent<UnitController>());
			other.GetComponent<UnitController>().StatContainer.RemoveBuff(this, statType);
		}

		private void OnDisable()
		{
			
		}

		private void OnDrawGizmos()
		{
			Color cachedColor = Gizmos.color;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, buffValue);
			Gizmos.color = cachedColor;
		}
	}
}
