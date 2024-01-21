using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Platform
{
    [SerializeField] private int _idCollider;
    [Space]
    [SerializeField] private bool _isTop = true;
    [Space]
    [SerializeField] private Side _borderOff;
    [SerializeField] private Side _borderAngle90;
    [SerializeField] private Vector2 _size;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _offsetTop;

    public int IdCollider => _idCollider;
    public bool IsTop => _isTop;
    public Vector2 SizeTop { get; private set; }
    public bool IsTwoFloor { get; private set; }
    public Polygon Top { get; private set; }
    public Polygon TopUV { get; private set; }
    public PolygonsLoop Bevels { get; private set; }
    public PolygonsLoop BevelsUV { get; private set; }
    public PolygonsLoop InnerSide { get; private set; }
    public PolygonsLoop InnerSideUV { get; private set; }
    public bool[] IgnoreBevel { get; private set; }

    private bool _noLeft;
    private bool _noTop;
    private bool _noRight;
    private bool _noBottom;
    private bool _90Left;
    private bool _90Top;
    private bool _90Right;
    private bool _90Bottom;

    private const float offsetQuarter = 0.5f;
    private const float offsetHalf = 1f;
    private const float offsetFull = 2f;

    public Platform(PlatformSaveData data)
    {
        _idCollider = data.IdCollider;
        _isTop = data.IsTop == 1;
        _borderOff = data.BorderOff.ToEnum<Side>();
        _borderAngle90 = data.BorderAngle90.ToEnum<Side>();
        _size = data.Size.ToVector2();
        _offset = data.Offset.ToVector3();
        _offsetTop = data.OffsetTop.ToVector3();
    }

    public PlatformSaveData GetData() => new(_idCollider, _isTop? 1: 0, _borderOff.ToInt(), _borderAngle90.ToInt(), _size.ToArray(), _offset.ToArray(), _offsetTop.ToArray());

    public void Calculate()
    {
        CalculatePlatformTypes();

        IsTwoFloor = _offset.y > 0;
        SizeTop = _size - Vector2.one * offsetFull;

        Vector3 totalOffset = _offset + _offsetTop;
        Top = (new Polygon(SizeTop).Offset(totalOffset));
        TopUV = (new Polygon(SizeTop).OffsetInPlane(SizeTop / 2f));

        Bevels = new(CalculateBasis(), Top);
        Polygon bevelUV = (new Polygon(SizeTop / 2f).OffsetInPlane(SizeTop / 2f));
        BevelsUV = new(TopUV, bevelUV);

        Polygon innerBottom = new Polygon(SizeTop * 0.9f ).Offset(totalOffset + new Vector3(0f, -_offsetTop.y, 0f));
        InnerSide = new(Top, innerBottom);
        InnerSideUV = BevelsUV;

        IgnoreBevel = new bool[] { _noLeft, _noTop, _noRight, _noBottom };

        Polygon CalculateBasis()
        {
            if (_borderOff.ToInt() == 255)
                return null;

            Vector2 size = _size;
            if (_noLeft) size.x -= offsetFull;
            else if (_90Left) size.x -= offsetHalf;
            if (_noTop) size.y -= offsetFull;
            else if (_90Top) size.y -= offsetHalf;
            if (_noRight) size.x -= offsetFull;
            else if (_90Right) size.x -= offsetHalf;
            if (_noBottom) size.y -= offsetFull;
            else if (_90Bottom) size.y -= offsetHalf;

            Polygon basis = new(size.To3D());

            Vector3 offset = _offset;
            if (_noLeft) offset.x += offsetHalf;
            else if (_90Left) offset.x += offsetQuarter;
            if (_noTop) offset.z -= offsetHalf;
            else if (_90Top) offset.z -= offsetQuarter;
            if (_noRight) offset.x -= offsetHalf;
            else if (_90Right) offset.x -= offsetQuarter;
            if (_noBottom) offset.z += offsetHalf;
            else if (_90Bottom) offset.z += offsetQuarter;

            basis.Offset(offset);

            return basis;
        }

        void CalculatePlatformTypes()
        {
            _noLeft = _borderOff.HasFlag(Side.Left);
            _noTop = _borderOff.HasFlag(Side.Top);
            _noRight = _borderOff.HasFlag(Side.Right);
            _noBottom = _borderOff.HasFlag(Side.Bottom);

            _90Left = _borderAngle90.HasFlag(Side.Left);
            _90Top = _borderAngle90.HasFlag(Side.Top);
            _90Right = _borderAngle90.HasFlag(Side.Right);
            _90Bottom = _borderAngle90.HasFlag(Side.Bottom);
        }
    }
}

public class PlatformSaveData : SizeOffsetSaveData
{
    [JsonProperty("Id")]
    public int IdCollider { get; }
    [JsonProperty("Tt")]
    public int IsTop { get; }
    [JsonProperty("Of")]
    public int BorderOff { get; }
    [JsonProperty("An")]
    public int BorderAngle90 { get; }
    [JsonProperty("FsT")]
    public float[] OffsetTop { get; }

    [JsonConstructor]
    public PlatformSaveData(int idCollider, int isTop, int borderOff, int borderAngle90, float[] size, float[] offset, float[] offsetTop) : base(size, offset)
    {
        IdCollider = idCollider;
        IsTop = isTop;
        BorderOff = borderOff;
        BorderAngle90 = borderAngle90;
        OffsetTop = offsetTop;
    }
}
