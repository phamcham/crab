using UnityEngine;

namespace PhamCham.Extension {
    public static class VectorExtensions {
        public static string PCToString(this Vector2 vector2){
            return $"({vector2.x}, {vector2.y})";
        }
        public static string PCToString(this Vector3 vector3){
            return $"({vector3.x}, {vector3.y}, {vector3.z})";
        }
    }
}