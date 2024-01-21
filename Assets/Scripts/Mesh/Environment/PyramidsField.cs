using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PyramidsField
{
    [SerializeField] private PyramidsFieldType _type;
    [SerializeField] private Vector3 _size;
    [SerializeField] private Vector3 _offset;

    public Vector2 Size { get; private set; }
    public Vector2 Offset { get; private set; }
    public List<Triangle> Triangles { get; private set; }
    public List<Triangle> TrianglesUV { get; private set; }

    public PyramidsField(PyramidsFieldSaveData data)
    {
        _type = data.Type;
        _size = data.Size.ToVector3();
        _offset = data.Offset.ToVector3();
    }

    public PyramidsFieldSaveData GetData() => new(_type, _size.ToArray(), _offset.ToArray());

    public void Calculate(float sideSize)
    {
        Size = _size.To2D();
        Offset = _offset.To2D();

        Triangles = new();
        TrianglesUV = new();
        int x = (int)_size.x - 1;
        int y = (int)_size.z - 1;
        Func<int, int, bool> IsContinue;
        Vector3 offset = _offset - (Size.To3D() - new Vector3(1f, 0f, 1f)) / 2f;
        Vector3 curOffset = Vector3.zero;

        IsContinue = _type switch
        { 
            PyramidsFieldType.Chess => Chess,
            PyramidsFieldType.Rect => Rect,
            PyramidsFieldType.Cross => Cross,
            PyramidsFieldType.InterlaceV => InterlaceV,
            PyramidsFieldType.InterlaceH => InterlaceH,
            PyramidsFieldType.RectOffL => RectOffL,
            PyramidsFieldType.RectOffR => RectOffR,
            PyramidsFieldType.RectOffT => RectOffT,
            PyramidsFieldType.RectOffB => RectOffB,
            PyramidsFieldType.AngleTR => AngleTR,
            PyramidsFieldType.AngleBR => AngleBR,
            PyramidsFieldType.AngleTL => AngleTL,
            PyramidsFieldType.AngleBL => AngleBL,
            _ => Solid
        };

        for (int a = 0; a <= x; a++) 
        {
            for (int b = 0; b <= y; b++)
            {
                if (IsContinue(a, b))
                    continue;

                curOffset.x = a; curOffset.z = b;
                Triangles.AddRange(Triangle.Pyramid(sideSize, _size.y, offset + curOffset));
                TrianglesUV.AddRange(Triangle.Pyramid(1f, 0f, Vector3.zero));
            }
        }

        bool Solid(int i, int j) => false;
        bool Chess(int i, int j) => (j + (i % 2 == 0 ? 0 : 1)) % 2 == 0;
        bool Rect(int i, int j) => i != 0 && i != x && j != 0 && j != y;
        bool Cross(int i, int j) => i != j && (x - i) != j;
        bool InterlaceV(int i, int j) => j % 2 == 0;
        bool InterlaceH(int i, int j) => i % 2 == 1;
        bool RectOffL(int i, int j) => i != x && j != 0 && j != y;
        bool RectOffR(int i, int j) => i != 0 && j != 0 && j != y;
        bool RectOffT(int i, int j) => i != 0 && i != x && j != 0;
        bool RectOffB(int i, int j) => i != 0 && i != x && j != y;
        bool AngleTR(int i, int j) => i != x && j != y;
        bool AngleBR(int i, int j) => i != x && j != 0;
        bool AngleTL(int i, int j) => i != 0 && j != y;
        bool AngleBL(int i, int j) => i != 0 && j != 0;
    }
}

public class PyramidsFieldSaveData : SizeOffsetSaveData
{
    [JsonProperty("Tp")]
    public PyramidsFieldType Type { get; }

    [JsonConstructor]
    public PyramidsFieldSaveData(PyramidsFieldType type, float[] size, float[] offset) : base(size, offset) => Type = type;
}
