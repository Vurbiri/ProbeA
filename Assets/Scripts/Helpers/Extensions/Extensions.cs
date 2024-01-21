using System;
using UnityEngine;

public static class Extensions
{
    public static string ToTimeString(this int self) => TimeSpan.FromSeconds(self).ToString(@"%m\:ss");
    public static void MoveTowards(this Rigidbody self, Vector3 target, float maxDistanceDelta)
    {
        self.MovePosition(Vector3.MoveTowards(self.position, target, maxDistanceDelta));
    }
    public static void Rotation(this Rigidbody self, Quaternion angle)
    {
        self.MoveRotation(self.rotation * angle);
    }
}
