using UnityEngine;
using Defense.Routing;
using System.Collections.Generic;
using Defense.Manager;
using Defense.Utils;


namespace Defense.Components
{
    public class Movable : MonoBehaviour
    {
        private MovementStat movementStat;

        private List<Vector3> wayList = new();

        private List<Vector3> pathList = new();

        private float widthOffset;

        [SerializeField] private int currentWayIdx = 0;

        private bool isMoving = false;

        public List<Vector3> PathList => pathList;

        public int CurrentWayIdx => currentWayIdx;

        public bool IsMoving { get => isMoving; set => isMoving = value; }

        private void Awake()
        {
            Debug.Log("Movable");
        }

        private void OnEnable()
        {
            Debug.Log("Movable enable");
        }

        public void Init(StatContainer statContainer)
        {
            if (!statContainer.TryGet(out movementStat)) Debug.LogWarning("Move Stat doesn't exist");
        }

        public void SetWay(bool isIgnoreObs = false)
        {
            pathList.Clear();

            pathList = Route.FindPath(GameManagerEx.Instance.Grid, transform.position, isIgnoreObs);
            
            for(int i = 0; i < pathList.Count; i++)
            {
                Debug.Log($"PATH {i} : {pathList[i]}");
            }

            wayList.Clear();

            widthOffset = pathList[0].x - transform.position.x;
            Debug.Log($"유닛의 widthOffset {widthOffset}");
            
            Vector3 point = Vector3.zero;
            
            for(int i = 0; i < pathList.Count; i++)
            {
                Vector3 forwardPrev = Vector3.zero;
                Vector3 forwardNext = Vector3.zero;

                if(i > 0)
                {
                    forwardPrev = (pathList[i] - pathList[i - 1]).normalized;
                }

                if (i < pathList.Count - 1)
                {
                    forwardNext = (pathList[i + 1] - pathList[i]).normalized;
                }

                if (i == 0)
                {
                    Vector3 right = new Vector3(forwardNext.z, 0, -forwardNext.x);
                    wayList.Add(pathList[i] + right * widthOffset);
                    continue;
                }

                // 끝점
                if (i == pathList.Count - 1)
                {
                    // 이전 방향 기준으로 offset
                    Vector3 right = new Vector3(forwardPrev.z, 0, -forwardPrev.x);
                    wayList.Add(pathList[i] + right * widthOffset);
                    continue;
                }

                // 중간 (코너 or 직선)
                float dot = Vector3.Dot(forwardPrev, forwardNext);

                // 거의 직선
                if (dot > 0.999f)
                {
                    Vector3 right = new Vector3(forwardNext.z, 0, -forwardNext.x);
                    wayList.Add(pathList[i] + right * widthOffset);
                }
                else
                {
                    // 코너 처리
                    Vector3 bisector = Calculation.GetConsistentBisector(-forwardPrev, forwardNext);

                    Vector3 cornerPoint = pathList[i] + bisector * widthOffset *1.4f;

                    wayList.Add(cornerPoint);
                }
            }

            isMoving = true;
        }

        public void Move()
        {
            if (wayList == null || wayList.Count == 0) return;

            if (currentWayIdx >= wayList.Count) return;

            //움직일 위치
            Vector3 targetPos = wayList[currentWayIdx];

            // 방향
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 50f * Time.deltaTime);
            }

            //움직임
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movementStat.CurrentSpeed.Value * Time.deltaTime);

            //도착하면 다음 인덱스로
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                currentWayIdx++;
                Debug.Log($"change index {currentWayIdx}");
                if(currentWayIdx >= wayList.Count)
                {
                    isMoving = false;
                }
            }
        }
    }
}
