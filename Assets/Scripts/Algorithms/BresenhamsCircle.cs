using System.Collections.Generic;
using UnityEngine;

public struct BresenhamsCircle {
    public static List<Vector2Int> CircleBres(Vector2Int center, int radius){
        List<Vector2Int> list = new List<Vector2Int>();
        int x = 0, y = radius;
        int d = 3 - 2 * radius;
        list.AddRange(DrawCircle(center.x, center.y, x, y));
        while (y >= x) {
            x++;
            if (d > 0) {
                y--;
                d = d + 4 * (x - y) + 10;
            }
            else{
                d = d + 4 * x + 6;
            }
            list.AddRange(DrawCircle(center.x, center.y, x, y));
        }
        return list;
    }
    private static List<Vector2Int> DrawCircle(int xc, int yc, int x, int y){
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(new Vector2Int(xc+x, yc+y));
        list.Add(new Vector2Int(xc-x, yc+y));
        list.Add(new Vector2Int(xc+x, yc-y));
        list.Add(new Vector2Int(xc-x, yc-y));
        list.Add(new Vector2Int(xc+y, yc+x));
        list.Add(new Vector2Int(xc-y, yc+x));
        list.Add(new Vector2Int(xc+y, yc-x));
        list.Add(new Vector2Int(xc-y, yc-x));
        return list;
    }
}