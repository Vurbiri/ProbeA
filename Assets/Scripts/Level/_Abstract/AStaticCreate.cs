using UnityEngine;

public abstract class AStaticCreate : MonoBehaviour
{
    public abstract CustomMesh Create(EnvironmentSaveData staticData, Color color);

#if UNITY_EDITOR
    public abstract CustomMesh Create(Color color);

    public void Delete()
    {
        foreach (var mc in gameObject.GetComponents<Collider>())
            DestroyImmediate(mc);
    }
    public abstract void LoadData(EnvironmentSaveData staticData);

    public abstract void ClearData();

#endif
}
