using System;
using Unity.Collections;
using UnityEngine;

namespace Defense
{
	public class ItemDataBase : ScriptableObject
	{
		[Header("Item Data")]
		[SerializeField, ReadOnly] private string itemID;

		public string ItemID => itemID;

		[ContextMenu("Generate ID")]
		private void GenerateID()
		{
			itemID = Guid.NewGuid().ToString();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (string.IsNullOrEmpty(itemID))
			{
				itemID = Guid.NewGuid().ToString();
			}
		}
#endif
	}
}
