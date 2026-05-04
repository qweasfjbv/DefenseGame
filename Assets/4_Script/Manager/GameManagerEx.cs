using Defense.Building;
using Defense.Components;
using Defense.Controller;
using Defense.Props;
using Defense.Routing;
using Defense.Systems;
using Defense.Utils;
using System;
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

		private CurrencySystem currency = new();
		
		private CurrencySystem Currency => currency;
		#endregion

		[SerializeField] private GameObject slotPrefab;
		[SerializeField] private GameObject testPrefab;
		[SerializeField] private GameObject wallPrefab;
		[SerializeField] private GameObject flagPrefab;

		[SerializeField, Range(0.5f, 3.0f)]
		private float timeScale = 1.0f;

		private Transform slotParent = null;
		private List<PlacementSlot> playerSlotList = new List<PlacementSlot>();
		private PlacementSlot firstSlot;

		private Grids grid;

		private int width = 5;
		private int height = 5;

		public List<PlacementSlot> PlayerSlotList => playerSlotList;
		public Grids Grid => grid;
		public int Width => width;
		public int Height => height;

		private void Awake()
		{
			Init();
			SpawnSlots();

			grid = new Grids(playerSlotList, firstSlot, width, height);
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

			firstSlot = Instantiate(slotPrefab, new Vector3(0, 0.01f, 30), Quaternion.Euler(90f, 0, 0), slotParent).GetComponent<PlacementSlot>();
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
			newController.SetPlayerTeam(0);

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

        [ContextMenu("SpawnEnemyUnit")]
        private void SpawnEnemyUnit()
		{
			GameObject enemyObj = Instantiate(testPrefab, transform);
			UnitController enemyUnit = enemyObj.GetComponent<UnitController>();
			enemyUnit.SetPlayerTeam(1);

			if(enemyUnit != null)
			{
                enemyUnit.InitUnit(0);
            }

            Movable movable;
			if(!enemyObj.TryGetComponent<Movable>(out movable))
			{
				movable = enemyObj.AddComponent<Movable>();
			}

			float randomX = UnityEngine.Random.Range(firstSlot.transform.position.x - Constants.SLOT_WIDTH / 2,
				firstSlot.transform.position.x + Constants.SLOT_WIDTH / 2);

			enemyObj.transform.position = new Vector3(randomX, 0, firstSlot.transform.position.z);

			movable.SetWay();
		}
      
		[ContextMenu("SpawnWall")]
		private void SpawnWall()
		{
			Wall wall = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity).GetComponent<Wall>();

			for (int i = 0; i < playerSlotList.Count; i++)
			{
				if (playerSlotList[i].TryAdd(wall as ISlottable)) return;
			}
		}

		[ContextMenu("SpawnFlag")]
		private void SpawnFlag()
		{
			Flag flag = Instantiate(flagPrefab, Vector3.zero, Quaternion.identity).GetComponent<Flag>();

			for (int i = 0; i < playerSlotList.Count; i++)
			{
				if (playerSlotList[i].TryAdd(flag as ISlottable)) return;
			}
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
