using System.Collections.Generic;
using UnityEngine;

public class PlatformsCreate : AStaticCreate
{
    [SerializeField] private PhysicMaterial _physicMaterial;
    [Space]
    [SerializeField] private Material _materialBevel;
    [SerializeField] private Material _materialTop;
    [SerializeField] private Material _materialTopTwo;
#if UNITY_EDITOR
    [Space]
    [SerializeField] private List<Platform> _platforms;
#endif

    private const int idSubMeshBevel = 0;
    private const int idSubMeshTop = 1;
    private const int idSubMeshTopTwo = 2;
    private const int minSubMeshCount = idSubMeshBevel + 1;
    private const int baseSubMeshCount = idSubMeshTop + 1;
    private const int extraSubMeshCount = idSubMeshTopTwo + 1;

    public override CustomMesh Create(EnvironmentSaveData staticData, Color color)
    {
        List<PlatformSaveData> data = staticData?.Platforms;
        if (data == null || data.Count == 0)
            return null;

        List<Platform> platforms = new(data.Count);
        foreach (PlatformSaveData d in data)
            platforms.Add(new(d));

        CustomMesh customMesh;
        int subMeshCount = minSubMeshCount;
        Vector2 sizeTopUV = Vector2.zero;
        Vector2 sizeTopTwoUV = Vector2.zero;
        bool isTop = false;
        Initialize();

        int currentPlatform = 0;
        foreach (Platform platform in platforms)
            Construction(platform);

        SetupMaterials();
        foreach (Mesh m in customMesh.ToColliderMeshes())
            CreateCollider(m);

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int platformsCount = platforms.Count;
            int colliderCount = 1;
            int colliderPlatform;
            Platform p;
            for (int i = 0; i < platformsCount; i++)
            {
                p = platforms[i];
                p.Calculate();

                colliderPlatform = p.IdCollider + 1;
                if (colliderPlatform > colliderCount)
                    colliderCount = colliderPlatform;

                isTop = isTop || p.IsTop;
                if (!p.IsTop)
                    continue;

                if (p.IsTwoFloor)
                {
                    subMeshCount = extraSubMeshCount;
                    sizeTopTwoUV = Vector2.Max(sizeTopTwoUV, p.SizeTop);
                }
                else 
                {
                    subMeshCount = Mathf.Max(subMeshCount, baseSubMeshCount);
                    sizeTopUV = Vector2.Max(sizeTopUV, p.SizeTop);
                }
            }
            customMesh = new(gameObject.name, subMeshCount, colliderCount, platformsCount);
        }
        void Construction(Platform p)
        {
            customMesh.AddUV(new(Vector2.zero, p.IsTwoFloor ? sizeTopTwoUV : sizeTopUV), currentPlatform);
            if(p.IsTop)
                customMesh.AddPolygon(p.Top, p.TopUV, p.IsTwoFloor ? idSubMeshTopTwo : idSubMeshTop, p.IdCollider, currentPlatform);

            for (int i = 0; i < p.Bevels.Count; i++)
            {
               if (p.IgnoreBevel[i])
                    continue;
                customMesh.AddPolygon(p.Bevels[i], p.BevelsUV[i], idSubMeshBevel, p.IdCollider, currentPlatform);
                if (!p.IsTop)
                    customMesh.AddPolygon(p.InnerSide[i], p.InnerSideUV[i], -1, p.IdCollider);
            }
            currentPlatform++;
        }
        void CreateCollider(Mesh mesh)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = _physicMaterial;
        }
        void SetupMaterials()
        {
            _materialBevel.SetColor(color);
            List<Material> materials = new(subMeshCount)
            {
                _materialBevel,
            };

            if (isTop)
            {
                _materialTop.SetColor(color);
                _materialTop.SetTailing(sizeTopUV);
                _materialTop.SetSeed(1f, 9999f);
                materials.Add(_materialTop);
                if (subMeshCount == extraSubMeshCount)
                {
                    _materialTopTwo.SetColor(color);
                    _materialTopTwo.SetTailing(sizeTopTwoUV);
                    materials.Add(_materialTopTwo);
                }
            }
            customMesh.Materials.AddRange(materials);
        }
        #endregion
    }

#if UNITY_EDITOR
    public override CustomMesh Create(Color color)
    {
        if (_platforms.Count == 0)
            return null;

        CustomMesh customMesh;
        int subMeshCount = minSubMeshCount;
        Vector2 sizeTopUV = Vector2.zero;
        Vector2 sizeTopTwoUV = Vector2.zero;
        bool isTop = false;
        Initialize();

        int currentPlatform = 0;
        foreach (Platform platform in _platforms)
            Construction(platform);

        SetupMaterials();
        foreach (Mesh m in customMesh.ToColliderMeshes())
            CreateCollider(m);

        return customMesh;

        #region Local Functions
        void Initialize()
        {
            int platformsCount = _platforms.Count;
            int colliderCount = 1;
            int colliderPlatform;
            Platform p;
            for (int i = 0; i < platformsCount; i++)
            {
                p = _platforms[i];
                p.Calculate();

                colliderPlatform = p.IdCollider + 1;
                if (colliderPlatform > colliderCount)
                    colliderCount = colliderPlatform;

                isTop = isTop || p.IsTop;
                if (!p.IsTop)
                    continue;

                if (p.IsTwoFloor)
                {
                    subMeshCount = extraSubMeshCount;
                    sizeTopTwoUV = Vector2.Max(sizeTopTwoUV, p.SizeTop);
                }
                else
                {
                    subMeshCount = Mathf.Max(subMeshCount, baseSubMeshCount);
                    sizeTopUV = Vector2.Max(sizeTopUV, p.SizeTop);
                }
            }
            customMesh = new(gameObject.name, subMeshCount, colliderCount, platformsCount);
        }
        void Construction(Platform p)
        {
            customMesh.AddUV(new(Vector2.zero, p.IsTwoFloor ? sizeTopTwoUV : sizeTopUV), currentPlatform);
            if (p.IsTop)
                customMesh.AddPolygon(p.Top, p.TopUV, p.IsTwoFloor ? idSubMeshTopTwo : idSubMeshTop, p.IdCollider, currentPlatform);

            for (int i = 0; i < p.Bevels.Count; i++)
            {
                if (p.IgnoreBevel[i])
                    continue;
                customMesh.AddPolygon(p.Bevels[i], p.BevelsUV[i], idSubMeshBevel, p.IdCollider, currentPlatform);
                if (!p.IsTop)
                    customMesh.AddPolygon(p.InnerSide[i], p.InnerSideUV[i], -1, p.IdCollider);
            }
            currentPlatform++;
        }
        void CreateCollider(Mesh mesh)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = _physicMaterial;
        }
        void SetupMaterials()
        {
            _materialBevel.SetColor(color);
            List<Material> materials = new(subMeshCount)
            {
                _materialBevel,
            };

            if (isTop)
            {
                _materialTop.SetColor(color);
                _materialTop.SetTailing(sizeTopUV);
                _materialTop.SetSeed(1f, 9999f);
                materials.Add(_materialTop);
                if (subMeshCount == extraSubMeshCount)
                {
                    //_materialTopTwo.SetColor(ShaderRef._Color, color);
                    _materialTopTwo.SetTailing(sizeTopTwoUV);
                    materials.Add(_materialTopTwo);
                }
            }
            customMesh.Materials.AddRange(materials);
        }
        #endregion
    }

    public List<PlatformSaveData> Get()
    {
        List<PlatformSaveData> platforms = new(_platforms.Count);
        foreach (var platform in _platforms)
            platforms.Add(platform.GetData());

        return platforms;
    }
    public override void LoadData(EnvironmentSaveData staticData)
    {
        _platforms.Clear();
        List<PlatformSaveData> data = staticData?.Platforms;
        if (data == null || data.Count == 0)
            return;

        foreach (var d in data)
            _platforms.Add(new(d));
    }
    public override void ClearData()
    {
        _platforms.Clear();
    }
#endif
}


