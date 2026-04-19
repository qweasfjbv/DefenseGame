using UnityEngine;

namespace Defense.Routing
{
    public class Node 
    {
        //private Node parentNode;
        private Vector2Int position;
        private Vector3 realPosition;

        private int id;
        private bool isObstacle;

        //public Node ParentNode { get => parentNode; set => parentNode = value; }
        public Vector2Int Position => position;
        public Vector3 RealPosition => realPosition;
        //public float GridCost { get => gridCost; set => gridCost = value; }
        //public float FinalCost => heuristicCost + gridCost;
        public int Id => id;
        public bool IsObstacle { get => isObstacle; set => isObstacle = value; }

        public Node(int id, Vector2Int pos, Vector3 realPos)
        {
            this.id = id;
            this.position = pos;
            this.realPosition = realPos;

            isObstacle = false;
        }
    }

    public class PathNode
    {
        private Node originNode;
        private PathNode parentNode;

        private float heuristicCost;
        private float gridCost;

        public Node OriginNode { get => originNode; set => originNode = value; }

        public PathNode ParentNode { get => parentNode; set => parentNode = value; }

        public float GridCost { get => gridCost; set => gridCost = value; }
        public float FinalCost => heuristicCost + gridCost;

        public void CalculateFinalCost(float gridCost, Vector3 finalPos)
        {
            this.gridCost = gridCost;

            // heuristicCost 계산
            float hWidthCost = Mathf.Abs(finalPos.x - originNode.RealPosition.x);
            float hHeightCost = Mathf.Abs(finalPos.z - originNode.RealPosition.z);

            heuristicCost = hWidthCost + hHeightCost;
        }
    }
}
