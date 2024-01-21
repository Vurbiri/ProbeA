using System;
using System.Linq;

[Flags]
public enum Side : byte
{
    None,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
}
public enum PyramidsFieldType : byte 
{
    Solid,
    Chess,
    Rect,
    Cross,
    InterlaceV,
    InterlaceH,
    RectOffL,
    RectOffR,
    RectOffT,
    RectOffB,
    AngleTR,
    AngleBR,
    AngleTL,
    AngleBL,
}
public enum EnemyType : byte
{
    Orange,
    Red,
    Green,
    Gravity,
}

public enum StateType : byte
{
    HP,
    Speed,
    Stealth,
}

public enum MixerGroup : byte
{
    Music,
    SFX,
}
public enum AvatarSize : byte
{
    Small,
    Medium,
    Large
}

public enum MessageType : byte
{
    Normal,
    Warning,
    Error,
    FatalError
}

public enum GameModeStart : byte
{
    GameNew,
    LevelNew,
    LevelContinue
}

public static class ExtensionsEnum
{
    public static int ToInt<T>(this T self) where T : Enum => Convert.ToInt32(self);
    public static T ToEnum<T>(this string self) where T : Enum => (T)Enum.Parse(typeof(T), self, true);
    public static int ToEnumInt<T>(this string self) where T : Enum => ((T)Enum.Parse(typeof(T), self, true)).ToInt<T>();
    public static T ToEnum<T>(this int self) where T : Enum => (T)Enum.ToObject(typeof(T), self);
}

public class Enum<T> where T : Enum
{
    public static int Count => Enum.GetNames(typeof(T)).Length;
    public static T[] GetValues() => Enum.GetValues(typeof(T)).Cast<T>().ToArray<T>();
}