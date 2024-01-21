using NaughtyAttributes;
using UnityEngine;

public class ProbeSpawn : MonoBehaviour
{
    public void Create(float[] position)
    {
        transform.position = position.ToVector3();
        gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    [Button]
    public void Delete()
    {
       gameObject.SetActive(false);
    }
    public float[] Get()
    {
        return transform.position.ToArray();
    }
    public void LoadData(float[] position)
    {

        transform.position = position.ToVector3();
        gameObject.SetActive(true);
    }
#endif
}
