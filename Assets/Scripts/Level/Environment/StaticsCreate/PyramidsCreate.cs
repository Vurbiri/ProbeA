using System.Collections.Generic;
using UnityEngine;

public class PyramidsCreate : AStaticCreate
{
    [SerializeField] private PhysicMaterial _physicMaterial;
    [Space]
    [SerializeField] private float _sideSize = 0.9f;
    [Space]
    [SerializeField] private Material _material;
    [SerializeField] private float _colorMax = 0.9f;
    [SerializeField] private float _colorMin = 0.45f;
#if UNITY_EDITOR
    [Space]
    [SerializeField] private List<PyramidsField> _fields;
#endif

    public override CustomMesh Create(EnvironmentSaveData staticData, Color color)
    {
        List<PyramidsFieldSaveData> data = staticData?.PyramidsField;
        if (data == null || data.Count == 0)
            return null;

        List<PyramidsField> fields = new(data.Count);
        foreach (var d in data)
            fields.Add(new(d));

        CustomMesh customMesh;
        Initialize();

        int currentField = 0;
        foreach (var field in fields)
            Construction(field);

        MaterialSetup();
        foreach (Mesh m in customMesh.ToColliderMeshes())
            CreateCollider(m);

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int count = fields.Count;

            foreach (var p in fields)
                p.Calculate(_sideSize);

            customMesh = new(gameObject.name, 1, count, 1);
            customMesh.AddUV(new(-Vector2.one / 2f, Vector2.one), 0);
        }
        void Construction(PyramidsField f)
        {
            for(int i = 0; i < f.Triangles.Count; i++)
                customMesh.AddTriangle(f.Triangles[i], f.TrianglesUV[i], 0, currentField);

            currentField++;
        }
        void CreateCollider(Mesh mesh)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = _physicMaterial;
        }
        void MaterialSetup()
        {
            _material.SetBaseColorRandom(color, _colorMax, _colorMin);
            customMesh.Materials.Add(_material);
        }
        #endregion
    }

#if UNITY_EDITOR
    public override CustomMesh Create(Color color)
    {
        if (_fields.Count == 0)
            return null;

        CustomMesh customMesh;
        Vector2 size = Vector2.zero;
        Initialize();

        Vector2 offset;
        int currentField = 0;
        foreach (var field in _fields)
            Construction(field);

        MaterialSetup();
        foreach (Mesh m in customMesh.ToColliderMeshes())
            CreateCollider(m);

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int count = _fields.Count;

            foreach (var p in _fields)
            {
                p.Calculate(_sideSize);
                size = Vector2.Max(size, p.Size);
            }

            customMesh = new(gameObject.name, 1, count, count);
        }
        void Construction(PyramidsField f)
        {
            offset = -size + f.Offset + f.Size / 2f;
            customMesh.AddUV(new(offset, size), currentField);

            foreach (var triangle in f.Triangles)
                customMesh.AddTriangle(triangle, triangle, 0, currentField, currentField);

            currentField++;
        }
        void CreateCollider(Mesh mesh)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = _physicMaterial;
        }
        void MaterialSetup()
        {
            _material.SetBaseColorRandom(color, _colorMax, _colorMin);
            customMesh.Materials.Add(_material);
        }
        #endregion
    }
    public List<PyramidsFieldSaveData> Get()
    {
        List<PyramidsFieldSaveData> fields = new(_fields.Count);
        foreach (var field in _fields)
            fields.Add(field.GetData());

        return fields;
    }
    public override void LoadData(EnvironmentSaveData staticData)
    {
        _fields.Clear();
        List<PyramidsFieldSaveData> data = staticData?.PyramidsField;
        if (data == null || data.Count == 0)
            return;

        foreach (var d in data)
            _fields.Add(new(d));
    }
    public override void ClearData()
    {
        _fields.Clear();
    }
#endif
}
