using System.Collections.Generic;
using UnityEngine;

public static class ExtensionsCollection
{
    public static int Left<T>(this T[] self, int index) => (index == 0 ? self.Length : index) - 1;
    public static int Right<T>(this T[] self, int index) => (index + 1) % self.Length;
    public static int Next<T>(this T[] self, int index) => self.Right(index);

    public static Vector2 ToVector2(this float[] self)
    {
        Vector2 vector = Vector2.zero;
        for(int i = 0; i < self.Length; i++)
        {
            if (i == 2)
                break;

            vector[i] = self[i];
        }
        return vector;
    }
    public static Vector3 ToVector3(this float[] self)
    {
        Vector3 vector = Vector3.zero;
        for (int i = 0; i < self.Length; i++)
        {
            if (i == 3)
                break;

            vector[i] = self[i];
        }
        return vector;
    }

    public static bool BinaryContains<T>(this List<T> self, T item) => self.BinarySearch(item) >= 0;
}
