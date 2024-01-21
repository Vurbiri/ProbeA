using UnityEngine;

public class GroundCreate : AStaticCreate
{
    [SerializeField] private PhysicMaterial _physicMaterial;
    [Space]
    [SerializeField] private Material _material;
    [Space]
    [SerializeField] protected float _height;
    [SerializeField] protected Vector2 _size;
    [Space]
    [SerializeField] private Vector2 _density = new(1f, 3f);

    public override CustomMesh Create(EnvironmentSaveData staticData, Color color)
    {
        CustomMesh customMesh = new(gameObject.name, 1, 0, 1);
        customMesh.AddUV(new(-_size / 2f, _size), 0);
        Polygon polygon = new(_size);
        customMesh.AddPolygon(polygon, polygon, 0);

        CreateCollider();
        MaterialSetup();

        return customMesh;

        #region Local Functions
        void CreateCollider()
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = _size.To3D(_height); ;
            collider.center = new(0f, -_height / 2f, 0f); ;
        }
        void MaterialSetup()
        {
            _material.SetColor(color);
            _material.SetTailing(_size);
            _material.SetSeed(1.0f, 999.9f);
            _material.SetDensity(_density);

            customMesh.Materials.Add(_material);
        }
        #endregion
    }

#if UNITY_EDITOR
    public override CustomMesh Create(Color color)
    {
        CustomMesh customMesh = new(gameObject.name, 1, 0, 1);
        customMesh.AddUV(new(-_size / 2f, _size), 0);
        Polygon polygon = new(_size);
        customMesh.AddPolygon(polygon, polygon, 0);

        CreateCollider();
        MaterialSetup();

        return customMesh;

        #region Local Functions
        void CreateCollider()
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.sharedMaterial = _physicMaterial;
            collider.size = _size.To3D(_height); ;
            collider.center = new(0f, -_height / 2f, 0f); ;
        }
        void MaterialSetup()
        {
            _material.SetColor(color);
            _material.SetTailing(_size);
            _material.SetSeed(1.0f, 999.9f);
            _material.SetDensity(_density);
            customMesh.Materials.Add(_material);
        }
        #endregion
    }

    public override void LoadData(EnvironmentSaveData staticData)
    {
        
    }

    public override void ClearData()
    {
        
    }
#endif
}

