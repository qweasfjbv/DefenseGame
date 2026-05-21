using Defense.Building;
using Defense.Manager;
using Defense.Utils;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Props
{
	public enum SlotType
	{
		None,
		Unit,
		Building,
	}

	public interface ISlottable
	{
		public SlotType SlotType { get; }
		public string ItemID { get; }
		public PlacementSlot MySlot { get; set; }

		public void PickUp(float posY);
		public void DropTo(Vector3 position);

		public void OnStartStage();
		public void OnEndStage();
	}

	[System.Serializable]
	public class SlotRule
	{
		private SlotType slotType;
		private int maxCount;

		public SlotType SlotType => slotType;
		public int MaxCount => maxCount;

		public SlotRule(SlotType type,  int maxCount)
		{
			this.slotType = type;
			this.maxCount = maxCount;
		}
	}

	public class PlacementSlot : MonoBehaviour
	{
		private List<ISlottable> items = new();
		private SlotType currentSlotType = SlotType.None;

		private static float[,,] relativePos = new float[4, 3, 2]
		{
			{	// Count 0
				{0f, 0f}, {0f, 0f}, {0f, 0f}
			},
			{	// Count 1
				{0f, 0f}, {0f, 0f}, {0f, 0f}
			},
			{	// Count 2
				{-.2f, 0f}, {.2f, 0f}, {0f, 0f}
			},
			{	// Count 3
				{-.2f, -.15f}, {.2f, -.15f}, {0f, .15f}
			},
		};

		private void SetStartSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_START : Color.white;
		}
		private void SetEndSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_END : Color.white;
		}

		public bool IsEmpty()
		{
			return currentSlotType == SlotType.None;
		}
		public bool HasWall()
		{
			return currentSlotType == SlotType.Building &&
				items[0] is Wall;
		}
		public bool HasObstacle()
		{
			return currentSlotType == SlotType.Building;
		}

		public bool CanAdd(ISlottable item)
		{
			if (currentSlotType == SlotType.None) return true;
			if (item.SlotType != currentSlotType) return false;

			switch (currentSlotType)
			{
				case SlotType.Unit:
					return items.Count < 3 && items[0].ItemID == item.ItemID;
				case SlotType.Building:
					return items.Count < 1;
			}

			return false;
		}

		public bool TryAdd(ISlottable item)
		{
			if(!CanAdd(item)) return false;

			items.Add(item);

			if(currentSlotType == SlotType.None) 
				currentSlotType = item.SlotType;

			GameManagerEx.Instance.RefreshAround(this);
			item.MySlot = this;
			DropAllUnits();
			return true;
		}

		public void Clear()
		{
			// Only Logic
			currentSlotType = SlotType.None;
			items = null;
			GameManagerEx.Instance.RefreshAround(this);
		}

		public void SetUnits(List<ISlottable> items)
		{
			if (items == null || items.Count == 0)
			{
				this.items = new();
				currentSlotType = SlotType.None;
				return;
			}

			this.items = items;
			currentSlotType = items[0].SlotType;
			for (int i = 0; i < items.Count; i++) items[i].MySlot = this;
		}

		public void OnStartStage()
		{
			GetComponent<MeshRenderer>().material.color = Color.clear;
			for(int i=0;i<items.Count;i++)
			{
				items[i].OnStartStage();
			}
		}
		public void OnEndStage()
		{
			GetComponent<MeshRenderer>().material.color = Color.white;
			for (int i = 0; i < items.Count; i++)
			{
				items[i].OnEndStage();
			}
			DropAllUnits();
		}

		private bool isSelected = false;
		public void OnHover()
		{
			if (isSelected) return;
			SetEndSlot(true);
		}
		public void OnUnhover()
		{
			if (isSelected) return;
			SetEndSlot(false);
		}
		public void OnSelect()
		{
			PickAllUnits();
			isSelected = true;
			SetStartSlot(true);
		}
		public void OnRelease()
		{
			DropAllUnits();
			isSelected = false;
			SetStartSlot(false);
		}

		public void ChangeSlot(PlacementSlot slot)
		{
			OnRelease();
			if (slot == this) return;
			slot.OnRelease();

			List<ISlottable> tmpUnit = slot.items;
			slot.SetUnits(items);
			SetUnits(tmpUnit);

			DropAllUnits();
			slot.DropAllUnits();
			GameManagerEx.Instance.RefreshAround(slot);
			GameManagerEx.Instance.RefreshAround(this);
		}

		private void DropAllUnits()
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].DropTo(transform.position + Constants.SLOT_WIDTH * new Vector3(relativePos[items.Count,i,0], 0f, relativePos[items.Count,i,1])); 
			}
		}

		private void PickAllUnits()
		{
			for (int i = 0; i < items.Count; i++) { items[i].PickUp(transform.position.y); }
		}
	}
}