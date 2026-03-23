using Defense.Controller;
using Defense.Props;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Defense.Manager
{
	public class GameManagerEx : MonoBehaviour
	{
		#region Singleton
		private static GameManagerEx s_instance;
		public static GameManagerEx Instance { get { return s_instance; } }

		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}
		}
		#endregion

		[SerializeField] private GameObject personPrefab;
		[SerializeField] private int testCount;

		[SerializeField] private GameObject slotPrefab;

		[SerializeField, Range(0.5f, 3.0f)]
		private float timeScale = 1.0f;

		private Transform slotParent = null;
		private List<PlacementSlot> playerSlotList = new List<PlacementSlot>();

		private void Awake()
		{
			Init();
			SpawnSlots();
		}

		private void Update()
		{
			Time.timeScale = timeScale;

		}

		private void SpawnSlots()
		{
			slotParent = new GameObject{ name = "SlotParent" }.transform;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					PlacementSlot slot = Instantiate(slotPrefab, new Vector3(-10 + 5 * j, 0.01f,5f + 5 * i), Quaternion.Euler(90f, 0, 0), slotParent).GetComponent<PlacementSlot>();
					playerSlotList.Add(slot);
				}
			}
		}

		// HACK
		int randId = 0;

		[ContextMenu("SpawnUnit")]
		private void SpawnUnit()
		{
			int rand = UnityEngine.Random.Range(0, 2);
			if (!playerSlotList.Any((item) => item.IsEmpty())) return;

			// 뭘 Spawn할지 결정
			int id = (randId++) % 4;
			UnitController newController = Instantiate(Managers.Resource.GetUnitPrefab(id, 0), Vector3.zero, Quaternion.identity)
				.GetComponent<UnitController>();
			newController.InitUnit(id);

			int emptyIdx = -1;
			int sameIdx = -1;
			for(int i=0; i<playerSlotList.Count; i++)
			{
				if (playerSlotList[i].IsEmpty())
				{
					emptyIdx = i;
					continue;
				}

				if (playerSlotList[i].CanAdd(newController as ISlottable))
				{
					sameIdx = i;
				}
			}

			int finalIndex = -1;

			if (emptyIdx >= 0) finalIndex = emptyIdx;
			if (sameIdx >= 0) finalIndex = sameIdx;

			if(finalIndex < 0)
			{
				Debug.Log("필드 가득참!!");
				return;
			}

			playerSlotList[finalIndex].TryAdd(newController as ISlottable);
		}

		// Change Input, hide slots
		[ContextMenu("StartStage")]
		private void StartStage()
		{
			for (int i = 0; i < playerSlotList.Count; i++)
			{
				playerSlotList[i].OnStartStage();
			}
		}

		// Change Input, Show slot, revive units
		private void EndStage()
		{
			for (int i = 0; i < playerSlotList.Count; i++)
			{
				playerSlotList[i].OnEndStage();
			}
		}

		private int currentWave = 0;
		public void OnGameStartButtonClicked()
		{

		}

	}
}
