using UnityEngine;

public static class ExtensionsVector
{
    public static Vector2 Offset(this Vector2 self, float value) => self + value * Vector2.one;
    public static Vector3[] OffsetSelf(this Vector3[] self, Vector3 value)
    {
        for (int i = 0; i < self.Length; i++)
            self[i] += value;

        return self;
    }

    public static Vector2 To2D(this Vector3 self) => new(self.x, self.z);
    public static Vector3 To3D(this Vector2 self, float z = 0) => new(self.x, z, self.y);

    public static float[] ToArray(this Vector3 self) => new[] { self.x, self.y, self.z };
    public static float[] ToArray(this Vector2 self) => new[] { self.x, self.y};
}
