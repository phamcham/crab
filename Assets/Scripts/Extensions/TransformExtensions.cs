using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PhamCham.Extension {
    public static class TransformExtensions {
        public static void PCDestroyChildren(this Transform transform, bool destroyImmediate = false){
            int n = transform.childCount;
            for (int i = n - 1; i >= 0; i--) {
                if (destroyImmediate) {
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
                else {
                    Object.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}