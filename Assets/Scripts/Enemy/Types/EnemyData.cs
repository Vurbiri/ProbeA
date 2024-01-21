using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [SerializeField] private EnemyType _type;
    [SerializeField] private Vector3 _position;

    public EnemyType Type => _type;
    public Vector3 Position => _position;

    public EnemyData(EnemySaveData saveData)
    {
        _type = saveData.Type.ToEnum<EnemyType>();
        _position = saveData.Position.ToVector3();
    }
}
