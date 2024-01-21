using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class EnvironmentCreate : MonoBehaviour
{
    [SerializeField] private RenderStaticCreate[] _statics;
    [SerializeField] private Slabs _slabs;

    public void Create(EnvironmentSaveData staticData, Color color)
    {
        foreach (var s in _statics) 
            s.Create(staticData, color);

        _slabs.Create(staticData.Slabs, color);

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public void Stop()
    {
        _slabs.Stop();
    }


#if UNITY_EDITOR

    public void CreateTest(EnvironmentSaveData staticData, Color color)
    {
        foreach (var s in _statics)
            s.Create(staticData, color);

        _slabs.CreateTest(staticData.Slabs, color);

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    public void Delete()
    {
        foreach (var s in _statics)
            s.Delete();

        _slabs.Delete();
    }

    public EnvironmentSaveData Get()
    {
        List<PyramidsFieldSaveData> pyramids = AStaticCreateToChild<PyramidsCreate>().Get();
        List<SizeOffsetSaveData> floors = AStaticCreateToChild<GlassFloorCreate>().Get();
        List<PlatformSaveData> platforms = AStaticCreateToChild<PlatformsCreate>().Get();
        List<float[]> columns = AStaticCreateToChild<ColumnsCreate>().Get();

        return new(pyramids, floors, platforms, columns, _slabs.Get());

        T AStaticCreateToChild<T>() where T : AStaticCreate
        {
            foreach (RenderStaticCreate rsc in _statics)
                foreach (AStaticCreate asc in rsc.Static)
                    if (asc is T)
                        return asc as T;
            return null;
        }
    }
    public void LoadData(EnvironmentSaveData staticData)
    {
        foreach (var s in _statics)
            s.LoadData(staticData);

        _slabs.LoadData(staticData.Slabs);
    }
    public void ClearData()
    {
        foreach (var s in _statics)
            s.ClearData();

        _slabs.ClearData();
    }
#endif
}


