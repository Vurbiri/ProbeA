using UnityEngine;

[System.Serializable]
public class GlassFloor
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private Vector3 _offset;

    public Vector2 Size => _size;
    public Vector3 SizeCollider { get; private set; }
    public Vector3 CenterCollider { get; private set; }
    public Polygon Floor { get; private set; }
    public Polygon FloorUV { get; private set; }


    public GlassFloor(SizeOffsetSaveData data)
    {
        _size = data.Size.ToVector3();
        _offset = data.Offset.ToVector3();
    }
    public SizeOffsetSaveData GetData() => new(_size.ToArray(), _offset.ToArray());

    public void Calculate(float height, float offsetUV)
    {
        SizeCollider = _size.To3D(height);
        CenterCollider = _offset + new Vector3(0f, -height / 2f, 0f);

        Floor = (new Polygon(_size).Offset(_offset));
        FloorUV = (new Polygon(_size).OffsetInPlane((_size / 2f) + Vector2.one * offsetUV));
    }
}

