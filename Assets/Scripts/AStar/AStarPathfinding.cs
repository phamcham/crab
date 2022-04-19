using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using PhamCham.Extension;

public class AStarPathfinding : MonoBehaviour {
    [SerializeField] private AStarGrid aStarGrid;

    public void FindPath(PathRequest request, Action<PathResult> callback) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        AStarNode startNode = aStarGrid.NodeFromWorldPoint(request.pathStart);
        AStarNode targetNode = aStarGrid.NodeFromWorldPoint(request.pathEnd);

        if (startNode == null || targetNode == null) {
            callback(new PathResult(waypoints, false, request.callback));
            return;
        }

        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable) {
            Heap<AStarNode> openSet = new Heap<AStarNode>(aStarGrid.MaxSize);
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                AStarNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    sw.Stop();
                    //MonoBehaviour.print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                List<AStarNode> adjs = aStarGrid.GetNeighbours(currentNode);
                adjs.Shuffer();
                foreach (AStarNode neighbour in adjs) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    private Vector2[] RetracePath(AStarNode startNode, AStarNode endNode) {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        List<AStarNode> simplify = SimplifyPath(path);
        Vector2[] waypoints = simplify.Select(s => (Vector2)s.worldPosition).ToArray();
        System.Array.Reverse(waypoints);
        return waypoints;
    }
    private List<AStarNode> SimplifyPath(List<AStarNode> path) {
        List<AStarNode> waypoints = new List<AStarNode>();

        if (path.Count > 0) waypoints.Add(path.First());

        for (int i = 1; i < path.Count - 1; i++) {
            Vector2 directionLeft = path[i - 1].grid - path[i].grid;
            Vector2 directionRight = path[i + 1].grid - path[i].grid;

            if (directionLeft.x * directionRight.y != directionLeft.y * directionRight.x) {
                waypoints.Add(path[i]);
            }
        }

        if (path.Count > 1) waypoints.Add(path.Last());

        return waypoints;
    }

    /*private Vector2[] SimplifyPath(List<AStarNode> path) {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count - 1; i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY).normalized;
            if (directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition);
                directionOld = directionNew;
            }
        }
        waypoints.Add(path.Last().worldPosition);

        return waypoints.ToArray();
    }*/

    private int GetDistance(AStarNode nodeA, AStarNode nodeB) {
        int dstX = Mathf.Abs(nodeA.grid.x - nodeB.grid.x);
        int dstY = Mathf.Abs(nodeA.grid.y - nodeB.grid.y);

        int diagonalCost = 14;
        int straightCost = 10;

        if (dstX == 1 && dstY == 1){
            Vector2Int crossA = new Vector2Int(nodeA.grid.x, nodeB.grid.y);
            Vector2Int crossB = new Vector2Int(nodeB.grid.x, nodeA.grid.y);
            // truong hop 2 cai la khong the vi lam sao ma di qua duoc, fix o neighbour roi ma!
            if (aStarGrid.NodeFromGridPoint(crossA)?.walkable == false || aStarGrid.NodeFromGridPoint(crossB)?.walkable == false){
                diagonalCost = 28;
            }
        }

        if (dstX > dstY)
            return diagonalCost * dstY + straightCost * (dstX - dstY);
        return diagonalCost * dstX + straightCost * (dstY - dstX);
    }
}