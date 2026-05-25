using Defense.Props;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Routing
{
    public class Grids 
    {
        private Node[,] nodes;

        private Node endNode;

        private int width;
        private int height;

        public Node EndNode => endNode;

        // todo : placement 리스트를 받아서 그리드를 만들도록
        public Grids(List<PlacementSlot> slotList, PlacementSlot firstSlot, int width, int height)
        {
            nodes = new Node[width, height+1];
            this.width = width;
            this.height = height;

            int id = 0;

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    Debug.Log($"Set Node {i},{j}, {slotList[i * width + j].transform.position}");
                    nodes[i, j] = new Node(id++, new Vector2Int(i, j), slotList[i*width+j].transform.position);
                }
            }

            for(int i = 0; i < width; i++)
            {
                nodes[i, height] = null;
            }

            nodes[width / 2, height] = new Node(id, new Vector2Int(width / 2, height), firstSlot.transform.position);

            // HACK
            endNode = nodes[0, 2];
        }

        public void SetObstacleNode(Vector2Int pos, bool isObstacle = true)
        {
            nodes[pos.x, pos.y].IsObstacle = isObstacle;
        }

        public Node GetNode(int i, int j)
        {
            if (i >= width || j >= height || i < 0 || j < 0)
            {
                Debug.LogWarning("OUT OF ARRAY RANGE");
                return null;
            }

            return nodes[i, j];
        }

        public (Node, float) GetNearestNode(Vector3 pos)
        {
            float minDistance = float.MaxValue;
            Node node = null;

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    float distance = Vector3.SqrMagnitude(pos - nodes[i, j].RealPosition);
                    if (distance < minDistance)
                    {
                        Debug.Log($"Get Node {i},{j}");
                        node = nodes[i, j];
                        minDistance = distance;
                    }
                }
            }
            
            return (node, Mathf.Sqrt(minDistance));
        }
    }
}
