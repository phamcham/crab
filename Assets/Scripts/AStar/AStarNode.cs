using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode> {
    public Vector3 worldPosition;
    public bool walkable;
    public Vector2Int gridIndex;
    public AStarNode parent;
    public int movementPenalty;
    public int tempObstacleUnit;

    public int gCost;
    public int hCost;

    private int heapIndex;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public AStarNode(bool _walkable, Vector3 _worldPos, Vector2Int _gridIndex, int _penalty, int _tempObstacleUnit = 0) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridIndex = _gridIndex;
        movementPenalty = _penalty;
        tempObstacleUnit = _tempObstacleUnit;
    }

    public int CompareTo(AStarNode nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}