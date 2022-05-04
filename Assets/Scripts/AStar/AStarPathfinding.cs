using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using PhamCham.Extension;

public class AStarPathfinding : MonoBehaviour {

    public void FindPath(PathRequest request, Action<PathResult> callback) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        Vector2[] fullPath = new Vector2[0];
        PathResultType pathSuccess = PathResultType.NotFound;

        AStarNode startNode = AStarGrid.current.NodeFromWorldPoint(request.pathStart);
        AStarNode targetNode = AStarGrid.current.NodeFromWorldPoint(request.pathEnd);

        if (startNode == null || targetNode == null) {
            callback(new PathResult(fullPath, waypoints, PathResultType.NotFound, request.callback));
            return;
        }

        //print(request.pathEnd + " " + request.pathStart);
        if (startNode == targetNode){
            if ((Vector2)targetNode.worldPosition != request.pathStart) {
                //print("found: " + request.pathStart + ", target: " + targetNode.worldPosition);
                fullPath = new Vector2[1] { targetNode.worldPosition };
                waypoints = new Vector2[1] { targetNode.worldPosition };
                
                callback(new PathResult(fullPath, waypoints, PathResultType.Found, request.callback));
            }
            else {
                callback(new PathResult(fullPath,waypoints, PathResultType.IsCompleted, request.callback));
            }
            return;
        }

        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable) {
            Heap<AStarNode> openSet = new Heap<AStarNode>(AStarGrid.current.MaxSize);
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                AStarNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    sw.Stop();
                    //MonoBehaviour.print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = PathResultType.Found;
                    break;
                }

                List<AStarNode> adjs = AStarGrid.current.GetNeighbours(currentNode);
                adjs.PCShuffer();
                foreach (AStarNode neighbour in adjs) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    // TODO: need fix hard code: neighbour.tempObstacleUnit * 10
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty + neighbour.tempObstacleUnit * 500;
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

        if (pathSuccess == PathResultType.Found) {
            List<AStarNode> retrace = RetracePath(startNode, targetNode);
            // kiem tra con duong nay co unit nao tren con duong minh dang di khong
            List<AStarNode> checkRetrace = new List<AStarNode>();
            foreach (AStarNode node in retrace) {
            //    if (node.tempObstacleUnit > 0) {
             //       pathSuccess = PathResultType.NotEnough;
            //        break;
            //    }
            //    else {
                    checkRetrace.Add(node);
             //   }
            }
            //
            fullPath = checkRetrace.Select(r => (Vector2)r.worldPosition).ToArray();
            waypoints = SimplifyPath(checkRetrace);
            if (waypoints.Length == 0) pathSuccess = PathResultType.NotFound;
        }
        callback(new PathResult(fullPath, waypoints, pathSuccess, request.callback));
    }

    private List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode) {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
    private Vector2[] SimplifyPath(List<AStarNode> path) {
        List<Vector2> waypoints = new List<Vector2>();

        if (path.Count > 0) waypoints.Add(path.First().worldPosition);

        for (int i = 1; i < path.Count - 1; i++) {
            Vector2 directionLeft = path[i - 1].gridIndex - path[i].gridIndex;
            Vector2 directionRight = path[i + 1].gridIndex - path[i].gridIndex;

            if (directionLeft.x * directionRight.y != directionLeft.y * directionRight.x) {
                waypoints.Add(path[i].worldPosition);
            }
        }

        if (path.Count > 1) waypoints.Add(path.Last().worldPosition);

        return waypoints.ToArray();
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
        int dstX = Mathf.Abs(nodeA.gridIndex.x - nodeB.gridIndex.x);
        int dstY = Mathf.Abs(nodeA.gridIndex.y - nodeB.gridIndex.y);

        int diagonalCost = 14;
        int straightCost = 10;

        if (dstX == 1 && dstY == 1){
            Vector2Int crossA = new Vector2Int(nodeA.gridIndex.x, nodeB.gridIndex.y);
            Vector2Int crossB = new Vector2Int(nodeB.gridIndex.x, nodeA.gridIndex.y);
            // truong hop 2 cai la khong the vi lam sao ma di qua duoc, fix o neighbour roi ma!
            if (AStarGrid.current.NodeFromGridPoint(crossA)?.walkable == false || AStarGrid.current.NodeFromGridPoint(crossB)?.walkable == false){
                diagonalCost = 28;
            }
        }

        if (dstX > dstY)
            return diagonalCost * dstY + straightCost * (dstX - dstY);
        return diagonalCost * dstX + straightCost * (dstY - dstX);
    }
}