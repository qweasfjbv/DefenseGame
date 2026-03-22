using Defense.Controller;
using Defense.Utils;
using IUtil;
using System.Collections.Generic;
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

		private void OnEnable()
		{
			// TODO - Initial Scan Needed
		}

		private void OnTriggerEnter(Collider other)
		{
			if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

			targets.Add(other.GetComponent<UnitController>());
		}

		private void OnTriggerExit(Collider other)
		{

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
