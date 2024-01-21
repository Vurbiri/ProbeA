using System.Collections.Generic;
using UnityEngine;

public class ColumnsCreate : AStaticCreate
{
    [SerializeField] private ColumnShape[] _columnShapes;
    [Space]
    [SerializeField] private PhysicMaterial _physicMaterial;
    [Space]
    [SerializeField] private Material _material;
    [SerializeField] private float _colorMax = 0.9f;
    [SerializeField] private float _colorMin = 0.25f;
#if UNITY_EDITOR
    [Space]
    [SerializeField] private List<Vector3> _offset;
#endif

    private ColumnShape _currentShape;
    private float SideSizeBase => _currentShape.SideSizeBase;
    private float SideSizeTop => _currentShape.SideSizeTop;
    private float TotalHeight => _currentShape.TotalHeight;
    private float BaseHeight => _currentShape.BaseHeight;

    public override CustomMesh Create(EnvironmentSaveData staticData, Color color)
    {
        List<float[]> data = staticData?.Columns;
        if (data == null || data.Count == 0)
            return null;
        
        _currentShape = _columnShapes[Random.Range(0, _columnShapes.Length)];

        CustomMesh customMesh = new(gameObject.name, 1, 0, 1);

        float pyramidHeight = TotalHeight - BaseHeight;
        Vector3 pyramidOffset = new(0, BaseHeight, 0);
        var(pyramidUV,sideUV) = CalkUV();
        Vector3 sizeCollider = new(SideSizeBase, TotalHeight, SideSizeBase);
        Vector3 centerCollider = new(0, TotalHeight / 2f, 0);

        foreach (float[] pos in data)
            Construction(pos.ToVector3());

        MaterialSetup();

        return customMesh;

        #region Local Functions
        void Construction(Vector3 offset)
        {
            Triangle[] pyramid = Triangle.Pyramid(SideSizeTop, pyramidHeight, offset + pyramidOffset);
            
            for (int i = 0; i < pyramid.Length; i++)
                customMesh.AddTriangle(pyramid[i], pyramidUV[i], 0);

            Polygon bottom = new(new Vector2(SideSizeBase, SideSizeBase));
            Polygon top = new(new Vector3(SideSizeTop, BaseHeight, SideSizeTop));
            PolygonsLoop loop = new(bottom.Offset(offset), top.Offset(offset));

            for (int i = 0; i < loop.Count; i++)
                customMesh.AddPolygon(loop[i], sideUV, 0);

            CreateCollider(offset);
        }
        void CreateCollider(Vector3 offset)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = sizeCollider;
            collider.center = offset + centerCollider;
        }
        void MaterialSetup()
        {
            _material.SetBaseColorRandom(color, _colorMax, _colorMin);
            customMesh.Materials.Add(_material);
        }
        (Triangle[] pyramidUV, Polygon sideUV) CalkUV()
        {
            float sizePyramidUV = pyramidHeight / TotalHeight;
            float sizeSideUV = BaseHeight / TotalHeight;
            customMesh.AddUV(new(Vector2.zero, Vector2.one), 0);
            return (PyramidUV(), SideUV());

            Triangle[] PyramidUV()
            {
                const int sideCount = 4;
                Vector3 of = (new Vector2(0.5f, sizeSideUV + sizePyramidUV / 2f)).To3D();
                Triangle[] triangles = new Triangle[sideCount];
                Vector3[] perimeter = Polygon.VerticesPolygon(new(1f, 0f, sizePyramidUV)).OffsetSelf(of);
                for (int i = 0; i < sideCount; i++)
                    triangles[i] = new(perimeter[i], perimeter[perimeter.Next(i)], of);
                return triangles;
            }
            Polygon SideUV()
            {
                Polygon p = new(new Vector2(1f, sizeSideUV));
                return p.OffsetInPlane(new Vector2(0.5f, sizeSideUV / 2f));
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    public override CustomMesh Create(Color color)
    {
        int columnsCount = _offset.Count;
        if (columnsCount == 0)
            return null;

        _currentShape = _columnShapes[Random.Range(0, _columnShapes.Length)];

         CustomMesh customMesh = new(gameObject.name, 1, 0, 1);

        float pyramidHeight = TotalHeight - BaseHeight;
        Vector3 pyramidOffset = new(0, BaseHeight, 0);
        float sizePyramidUV = pyramidHeight / TotalHeight;
        float sizeSideUV =  BaseHeight / TotalHeight;
        customMesh.AddUV(new(Vector2.zero, Vector2.one), 0);
        Triangle[] pyramidUV = PyramidUV();
        Polygon sideUV = SideUV();

        Vector3 sizeCollider = new(SideSizeBase, TotalHeight, SideSizeBase);
        Vector3 centerCollider = new(0, TotalHeight / 2f, 0);

        int currentColumn = 0;
        foreach (var pos in _offset)
            Construction(pos);

        MaterialSetup();

        return customMesh;

        #region Local Functions
        void Construction(Vector3 offset)
        {
            Triangle[] pyramid = Triangle.Pyramid(SideSizeTop, pyramidHeight, offset + pyramidOffset);
            
            for (int i = 0; i < pyramid.Length; i++)
                customMesh.AddTriangle(pyramid[i], pyramidUV[i], 0);

            Polygon bottom = new(new Vector2(SideSizeBase, SideSizeBase));
            Polygon top = new(new Vector3(SideSizeTop, BaseHeight, SideSizeTop));
            PolygonsLoop loop = new(bottom.Offset(offset), top.Offset(offset));

            for (int i = 0; i < loop.Count; i++)
                customMesh.AddPolygon(loop[i], sideUV, 0);

            CreateCollider(offset);

            currentColumn++;
        }
        void CreateCollider(Vector3 offset)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = sizeCollider;
            collider.center = offset + centerCollider;
        }
        void MaterialSetup()
        {
            _material.SetBaseColorRandom(color, _colorMax, _colorMin);
            customMesh.Materials.Add(_material);
        }
        Triangle[] PyramidUV()
        {
            const int sideCount = 4;
            Vector3 of = (new Vector2(0.5f, sizeSideUV + sizePyramidUV / 2f)).To3D();
            Triangle[] triangles = new Triangle[sideCount];
            Vector3[] perimeter = Polygon.VerticesPolygon(new(1f, 0f, sizePyramidUV)).OffsetSelf(of);
            for (int i = 0; i < sideCount; i++)
                triangles[i] = new(perimeter[i], perimeter[perimeter.Next(i)], of);
            return triangles;
        }
        Polygon SideUV()
        {
            Polygon p = new(new Vector2(1f, sizeSideUV));
            return p.OffsetInPlane(new Vector2(0.5f, sizeSideUV / 2f));
        }
        #endregion
    }
    public List<float[]> Get()
    {
        List<float[]> columns = new(_offset.Count);
        foreach (var pos in _offset)
            columns.Add(pos.ToArray());

        return columns;
    }
    public override void LoadData(EnvironmentSaveData staticData)
    {
        _offset.Clear();
        List<float[]> data = staticData?.Columns;
        if (data == null || data.Count == 0)
            return;

        foreach (var d in data)
            _offset.Add(d.ToVector3());

    }
    public override void ClearData()
    {
        _offset.Clear();
    }
#endif
}