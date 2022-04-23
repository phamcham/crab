using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid : MonoBehaviour {
    public static AStarGrid current { get; private set; }
    public AStarNode[,] nodes;
    Tilemap mainTilemap;
    public bool displayGridGizmos;
    public int obstacleProximityPenalty = 30;
    private Vector2Int gridWorldSize;
    private Vector2Int gridBottomLeftPosition;

    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MinValue;

    private void Awake() {
        current = this;
    }

    public int MaxSize {
        get {
            return gridWorldSize.x * gridWorldSize.y;
        }
    }

    public void UpdateGrid(Tilemap mainTilemap) {
        this.mainTilemap = mainTilemap;
        gridWorldSize = (Vector2Int)mainTilemap.cellBounds.size;
        gridBottomLeftPosition = (Vector2Int)mainTilemap.cellBounds.min;
        nodes = new AStarNode[gridWorldSize.x, gridWorldSize.y];

        for (int x = 0; x < gridWorldSize.x; x++) {
            for (int y = 0; y < gridWorldSize.y; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                UpdateGridNode(pos);
            }
        }

        BlurPenaltyMap(1);
    }

    private void BlurPenaltyMap(int blurSize) {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridWorldSize.x, gridWorldSize.y];
        int[,] penaltiesVerticalPass = new int[gridWorldSize.x, gridWorldSize.y];

        for (int y = 0; y < gridWorldSize.y; y++) {
            for (int x = -kernelExtents; x <= kernelExtents; x++) {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += nodes[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridWorldSize.x; x++) {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridWorldSize.x);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridWorldSize.x - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - nodes[removeIndex, y].movementPenalty + nodes[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridWorldSize.x; x++) {
            for (int y = -kernelExtents; y <= kernelExtents; y++) {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            nodes[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridWorldSize.y; y++) {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridWorldSize.y);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridWorldSize.y - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                nodes[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax) {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin) {
                    penaltyMin = blurredPenalty;
                }
            }
        }
    }

    public List<AStarNode> GetNeighbours(AStarNode node) {
        List<AStarNode> neighbours = new List<AStarNode>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                Vector2Int check = new Vector2Int(node.gridIndex.x + x, node.gridIndex.y + y);

                if (IsGridPointValid(check)) {
                    if (Mathf.Abs(x) == Mathf.Abs(y)) {
                        bool isCross = (nodes[check.x, node.gridIndex.y].walkable == false && nodes[node.gridIndex.x, check.y].walkable == false);
                        if (!isCross) {
                            neighbours.Add(nodes[check.x, check.y]);
                        }
                    } else {
                        neighbours.Add(nodes[check.x, check.y]);
                    }
                }
            }
        }

        return neighbours;
    }

    public bool IsGridPointValid(Vector2Int check){
        return 0 <= check.x && check.x < gridWorldSize.x && 0 <= check.y && check.y < gridWorldSize.y;
    }

    public AStarNode NodeFromWorldPoint(Vector3 worldPosition) {
        Vector2Int index = (Vector2Int)mainTilemap.WorldToCell((Vector2)worldPosition - gridBottomLeftPosition);
        if (!IsGridPointValid(index)) return null;
        return nodes[index.x, index.y];
    }

    public AStarNode NodeFromGridPoint(Vector2Int point) {
        if (!IsGridPointValid(point)) return null;
        return nodes[point.x, point.y];
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 size = new Vector3(gridWorldSize.x, gridWorldSize.y, 1);
        Vector3 center = (2 * gridBottomLeftPosition + (Vector2)size) / 2;
        Gizmos.DrawWireCube(center, size);
        Gizmos.DrawLine(center - size / 2, center + size / 2);
        Gizmos.DrawLine(center + new Vector3(-size.x, size.y) / 2, center + new Vector3(size.x, -size.y) / 2);
        Gizmos.color = Color.white;
        if (nodes != null && displayGridGizmos) {
            foreach (AStarNode n in nodes) {
                Gizmos.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(0, 0, 0, 1), Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.walkable) ? Gizmos.color : new Color(1, 0, 0, 1f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one);
            }
        }

        // debug nay rat nang
        /*
        UnityEditor.Handles.color = Color.white;
        for (int i = 0; i < gridWorldSize.x; i++) {
            for (int j = 0; j < gridWorldSize.y; j++) {
                int penaty = nodes[i, j].movementPenalty + 10 * nodes[i, j].tempObstacleUnit;
                Vector2Int cellIndex = new Vector2Int(i, j);
                Vector3Int cellPosition = (Vector3Int)IndexToPosition(cellIndex);
                Vector3 pos = (Vector2)mainTilemap.CellToWorld(cellPosition);
                UnityEditor.Handles.Label(pos + Vector3.one / 2, penaty + "", new GUIStyle { fontSize = 9, alignment = TextAnchor.MiddleCenter });
            }
        }
        */
        
    }

    // cellIndex from (0, 0)
    public void UpdateGridNode(Vector2Int cellIndex) {
        if (IsGridPointValid(cellIndex)) {
            int movementPenalty = 0;
            Vector3Int cellPosition = (Vector3Int)IndexToPosition(cellIndex);
            TileBase tile = mainTilemap.GetTile(cellPosition);
            //bool walkable = tile != null && !GridBuildingSystem.current.IsBuilding(tile);
            bool walkable = tile != null;

            if (!walkable) {
                movementPenalty += obstacleProximityPenalty;
            }

            Vector3 worldPos = mainTilemap.CellToWorld(cellPosition) + Vector3.one / 2;
            worldPos.z = 0;
            int temp = nodes[cellIndex.x, cellIndex.y]?.tempObstacleUnit ?? 0;
            nodes[cellIndex.x, cellIndex.y] = new AStarNode(walkable, worldPos, cellIndex, movementPenalty, temp);
        }
    }

    public void AddTempObstacle(Vector2Int cellIndex){
        if (IsGridPointValid(cellIndex)){
            nodes[cellIndex.x, cellIndex.y].tempObstacleUnit++;
        }
    }
    public void DeleteTempObstacle(Vector2Int cellIndex){
        if (IsGridPointValid(cellIndex)){
            nodes[cellIndex.x, cellIndex.y].tempObstacleUnit--;
        }
    }

    public Vector2Int IndexToPosition(Vector2Int index){
        return index + gridBottomLeftPosition;
    }
    public Vector2Int PositionToIndex(Vector2Int position){
        return position - gridBottomLeftPosition;
    }
}