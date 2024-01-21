#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RenderStaticCreate : MonoBehaviour
{
    [SerializeField] private AStaticCreate[] _staticCreate;

    public void Create(EnvironmentSaveData staticData, Color color)
    {
        int current = 0;
        int count = _staticCreate.Length;
        CustomMesh tempMesh = null;
        while(tempMesh == null)
        {
            if (current == count)
            {
                gameObject.SetActive(false);
                return;
            }
            tempMesh = _staticCreate[current].Create(staticData, color);
            current++;
        }
        CustomMesh customMesh = new("Mesh_" + gameObject.name, tempMesh);
        for(; current < count; current++)
        {
            tempMesh = _staticCreate[current].Create(staticData, color);
            if(tempMesh != null)
                customMesh.AddCustomMesh(tempMesh);
        }

        GetComponent<MeshFilter>().sharedMesh = customMesh.ToMesh();
        GetComponent<MeshRenderer>().SetMaterials(customMesh.Materials);

    }

#if UNITY_EDITOR

    public static Color colorDefault = new(0.6509804f, 0.6509804f, 0.3529412f, 1f);

    [Button]
    public void Create()
    {
        Delete();

        Color color = colorDefault;

        int current = 0;
        int count = _staticCreate.Length;
        CustomMesh tempMesh = null;
        while (tempMesh == null)
        {
            if (current == count)
                return;
            tempMesh = _staticCreate[current].Create(color);
            current++;
        }
        CustomMesh customMesh = new("Mesh_" + gameObject.name, tempMesh);
        for (; current < count; current++)
        {
            tempMesh = _staticCreate[current].Create(color);
            if (tempMesh != null)
                customMesh.AddCustomMesh(tempMesh);
        }

        GetComponent<MeshFilter>().sharedMesh = customMesh.ToMesh();
        GetComponent<MeshRenderer>().SetMaterials(customMesh.Materials);
    }
    [Button]
    public void Delete()
    {
        GetComponent<MeshFilter>().sharedMesh = null;
        GetComponent<MeshRenderer>().SetSharedMaterials(new());
        foreach (var s in _staticCreate)
            s.Delete();
    }

    public AStaticCreate[] Static => _staticCreate;

    public void LoadData(EnvironmentSaveData staticData)
    {
        foreach (var s in _staticCreate)
            s.LoadData(staticData);
    }
    public void ClearData()
    {
        foreach (var s in _staticCreate)
            s.ClearData();
    }

#endif
}
