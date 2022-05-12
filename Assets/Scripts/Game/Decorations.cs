using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Decorations : MonoBehaviour {
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] List<Sprite> decorSprites;
    [SerializeField] int number;
    [ContextMenu("Generate Decors")]
    private void Generate() {
        transform.PCDestroyChildren(true);
        List<Vector2> list = new List<Vector2>();
        foreach (Vector3Int pos in groundTilemap.cellBounds.allPositionsWithin) {
            if (pos.x == groundTilemap.cellBounds.xMax || pos.y == groundTilemap.cellBounds.xMax) continue;
            if (pos.x == groundTilemap.cellBounds.xMin || pos.y == groundTilemap.cellBounds.xMin) continue;
            if (groundTilemap.GetTile(pos) != null) {
                list.Add((Vector3)pos);
            }
        }
        list.PCShuffer();
        Stack<Vector2> stack = new Stack<Vector2>(list);

        for (int i = 0; i < number; i++) {
            Sprite sprite = GetRamdomSprite();
            if (sprite == null || stack.Count == 0) continue;
            Vector2 pos = stack.Pop();
            GameObject obj = new GameObject(string.Format("{0} ({1})", sprite.name, sprite.GetInstanceID()));
            obj.transform.SetParent(transform);
            obj.transform.position = pos;
            SpriteRenderer spRender = obj.AddComponent<SpriteRenderer>();
            spRender.sortingLayerID = SortingLayer.NameToID("Entity");
            spRender.sprite = sprite;
        }
    }

    private Sprite GetRamdomSprite() {
        if (decorSprites == null || decorSprites.Count == 0) return null;
        int index = Random.Range(0, decorSprites.Count);
        return decorSprites[index];
    }
}
