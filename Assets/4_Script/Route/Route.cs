using Defense.Manager;
using Defense.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Routing
{
    public static class Route
    {
        private static int[] dx = { 0, -1, 0, 1 };
        private static int[] dy = { -1, 0, 1, 0 };

        public static List<Vector3> FindPath(Grids grid, Vector3 unitPosition)
        {
            Dictionary<Node, PathNode> pathNodes = new();

            PathNode GetPathNode(Node node)
            {
                if(!pathNodes.TryGetValue(node, out PathNode pathNode))
                {
                    pathNode = new PathNode();
                    pathNode.OriginNode = node;

                    pathNodes[node] = pathNode;
                }

                return pathNodes[node];
            }

            SortedSet<PathNode> openNodeSet = new SortedSet<PathNode>(Comparer<PathNode>.Create((a, b) => {
                int result = a.FinalCost.CompareTo(b.FinalCost);
                if(result == 0)
                {
                    result = a.OriginNode.Id.CompareTo(b.OriginNode.Id);
                }
                return result;
            }));

            SortedSet<PathNode> closedNodeSet = new SortedSet<PathNode>(Comparer<PathNode>.Create((a, b) =>
            {
                int result = a.FinalCost.CompareTo(b.FinalCost);
                if (result == 0)
                {
                    result = a.OriginNode.Id.CompareTo(b.OriginNode.Id);
                }
                return result;
            }));

            Node endNode = grid.EndNode;

            (Node, float) data = grid.GetNearestNode(unitPosition);
            Debug.Log($"{data.Item1.RealPosition} node");

            PathNode startNode = GetPathNode(data.Item1);

            startNode.GridCost = data.Item2;

            openNodeSet.Add(startNode);

            while(openNodeSet.Count > 0)
            {
                // 오픈 리스트에서 F 값이 가장 작은 노드를 꺼낸다.
                PathNode openedPathNode = openNodeSet.Min;
                
                openNodeSet.Remove(openedPathNode);

                if (openedPathNode.OriginNode.Id == endNode.Id) break;

                // 꺼낸 노드의 인접한 노드들을 확인해서 넣는다.
                for(int i = 0; i < 4; i++)
                {
                    Node adjacentNode = grid.GetNode(openedPathNode.OriginNode.Position.x + dx[i], openedPathNode.OriginNode.Position.y + dy[i]);

                    if (adjacentNode == null) continue;

                    if (adjacentNode.IsObstacle) continue;

                    PathNode adjacentPathNode = GetPathNode(adjacentNode);

                    if (closedNodeSet.Contains(adjacentPathNode)) continue;

                    // 인접한 노드가 오픈 리스트에 있다면 
                    if (openNodeSet.Contains(adjacentPathNode))
                    {
                        // 현재 노드에서 인접한 노드까지 이동할 때 gridCost가 낮아지면
                        // 인접 노드의 부모 노드를 현재 노드로 변경 
                        // 인접 노드의 final Cost 다시 계산
                        if (adjacentPathNode.GridCost <= openedPathNode.GridCost + Constants.SLOT_WIDTH) continue;

                        adjacentPathNode.ParentNode = openedPathNode;
                        adjacentPathNode.CalculateFinalCost(openedPathNode.GridCost + Constants.SLOT_WIDTH, endNode.RealPosition);
                    }

                    // 부모 노드 설정
                    adjacentPathNode.ParentNode = openedPathNode;

                    // 인접한 노드의 비용 설정
                    adjacentPathNode.CalculateFinalCost(openedPathNode.GridCost + Constants.SLOT_WIDTH, endNode.RealPosition);

                    openNodeSet.Add(adjacentPathNode);
                }

                // 이 노드를 클로즈 리스트에 넣어
                closedNodeSet.Add(openedPathNode);
            }

            List<Vector3> path = new List<Vector3>();

            PathNode pathNode = GetPathNode(endNode);

            while(pathNode.ParentNode != null)
            {
                path.Add(pathNode.OriginNode.RealPosition);
                pathNode = pathNode.ParentNode;
            }

            path.Add(pathNode.OriginNode.RealPosition);

            path.Reverse();

            return path;
        }
    }
}
