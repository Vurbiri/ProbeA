using UnityEngine;

public static class URandom 
{
    public static int Seed => unchecked(_seed++);
    private static int _seed = unchecked(-(int)System.DateTime.Now.Ticks);

    public static void Init() => Random.InitState(Seed);
    public static void InitState(int seed) => Random.InitState(seed);

    public static float Range(float minInclusive, float maxInclusive) => Random.Range(minInclusive, maxInclusive);
    public static int Range(int minInclusive, int maxExclusive) => Random.Range(minInclusive, maxExclusive);

    public static bool Chance(int chance = 50) => Range(0, 100) < chance;

    public static Vector2 Vector2(float minInclusive, float maxInclusive) => new(Range(minInclusive, maxInclusive), Range(minInclusive, maxInclusive));
}
