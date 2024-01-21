using System.Collections.Generic;
using UnityEngine;

public class GlassFloorCreate : AStaticCreate
{
    [SerializeField] private PhysicMaterial _physicMaterial;
    [Space]
    [SerializeField] private Material _material;
    [Space]
    [SerializeField] private float _height = 1f;
    [Space]
    [SerializeField] private Vector2 _density = new(2f,6f);
    [SerializeField] private Vector2 _seed = new(5f, 555f);
    [SerializeField] private float _offsetUV = 0.15f;
#if UNITY_EDITOR
    [Space]
    [SerializeField] private List<GlassFloor> _floors;
#endif

    public override CustomMesh Create(EnvironmentSaveData staticData, Color color)
    {
        List<SizeOffsetSaveData> data = staticData?.GlassFloors;
        if (data == null || data.Count == 0)
            return null;

        List<GlassFloor> floors;
        CustomMesh customMesh;
        Vector2 sizeUV;
        Initialize();

        for (int i = 0; i < floors.Count; i++)
        {
            customMesh.AddUV(new(Vector2.zero, sizeUV), i);
            customMesh.AddPolygon(floors[i].Floor, floors[i].FloorUV, 0, -1, i);
            CreateCollider(floors[i]);
        }

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int count = data.Count;
            sizeUV = Vector2.zero;
            customMesh = new(gameObject.name, 1, 0, count);
            floors = new(count);
            GlassFloor floor;
            for(int i = 0; i < count; i++)
            {
                floor = new(data[i]);
                floor.Calculate(_height, i * _offsetUV);
                floors.Add(floor);
                sizeUV = Vector2.Max(sizeUV, floor.Size);
            }
            sizeUV += _offsetUV * (count - 1) * Vector2.one;

            MaterialSetup(sizeUV);
        }
        void MaterialSetup(Vector2 size)
        {
            _material.SetColor(color);
            _material.SetTailing(size);
            _material.SetSeed(_seed.x, _seed.y);
            _material.SetDensity(_density);

            customMesh.Materials.Add(_material);
        }
        void CreateCollider(GlassFloor f)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = f.SizeCollider;
            collider.center = f.CenterCollider;
        }
        #endregion
    }

#if UNITY_EDITOR

    public override CustomMesh Create(Color color)
    {
        CustomMesh customMesh;
        Vector2 sizeUV;
        Initialize();

        for (int i = 0; i < _floors.Count; i++)
        {
            customMesh.AddUV(new(Vector2.zero, sizeUV), i);
            customMesh.AddPolygon(_floors[i].Floor, _floors[i].FloorUV, 0, -1, i);
            CreateCollider(_floors[i]);
        }

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int count = _floors.Count;
            sizeUV = Vector2.zero;
            customMesh = new(gameObject.name, 1, 0, count);
            GlassFloor floor;
            for (int i = 0; i < count; i++)
            {
                floor = _floors[i];
                floor.Calculate(_height, i * _offsetUV);
                sizeUV = Vector2.Max(sizeUV, floor.Size);
            }
            sizeUV += _offsetUV * (count - 1) * Vector2.one;

            MaterialSetup(sizeUV);
        }
        void MaterialSetup(Vector2 size)
        {
            _material.SetColor(color);
            _material.SetTailing(size);
            _material.SetSeed(_seed.x, _seed.y);
            _material.SetDensity(_density);

            customMesh.Materials.Add(_material);
        }
        void CreateCollider(GlassFloor f)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = f.SizeCollider;
            collider.center = f.CenterCollider;
        }
        #endregion
    }
    public List<SizeOffsetSaveData> Get()
    {
        List<SizeOffsetSaveData> floors = new(_floors.Count);
        foreach (var floor in _floors)
            floors.Add(floor.GetData());

        return floors;
    }
    public override void LoadData(EnvironmentSaveData staticData)
    {
        _floors.Clear();
        List<SizeOffsetSaveData> data = staticData?.GlassFloors;
        if (data == null || data.Count == 0)
            return;

        foreach (var d in data)
            _floors.Add(new(d));

    }
    public override void ClearData()
    {
        _floors.Clear();
    }
#endif
}
