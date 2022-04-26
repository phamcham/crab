using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace PhamCham.Extension {
    public static class ListExtensions {
        public static void PCShuffer<TSource>(this IList<TSource> list) {
            if (list == null) {
                throw new ArgumentNullException("list");
            }
            int n = list.Count;
            for (int i = 0; i + 1 < n; i++) {
                int j = UnityEngine.Random.Range(i + 1, n);
                list.PCSwap(i, j);
            }
        }
        public static void PCSwap<TSource>(this IList<TSource> list, int firstIndex, int secondIndex) {
            if (list == null) {
                throw new ArgumentNullException("list");
            }
            if (firstIndex < 0 || firstIndex >= list.Count) {
                throw new ArgumentOutOfRangeException("firstIndex");
            }
            if (secondIndex < 0 || secondIndex >= list.Count) {
                throw new ArgumentOutOfRangeException("secondIndex");
            }

            // no swap
            if (firstIndex == secondIndex) {
                return;
            }
            TSource temp = list[secondIndex];
            list[secondIndex] = list[firstIndex];
            list[firstIndex] = temp;
        }
    }
}