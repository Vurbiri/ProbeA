using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AudioSettings;

public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private EnemyCreateType[] _enemiesCreateType;
    [SerializeField] private ParticleSystemForceField[] _psBulletTargets;
#if UNITY_EDITOR
    [Space]
    [SerializeField] private List<EnemyData> _enemiesData;
#endif

    public List<Enemy> Create(List<EnemySaveData> data)
    {
        int count = data?.Count ?? 0;
        if (count == 0)
            return null;

        Transform thisTransform = transform;
        bool isDifficulty = GameData.Instance.IsDifficulty;
        List<Enemy> enemies = new(count);
        Dictionary<EnemyType, int> priorities = new(_enemiesCreateType.Length);
        foreach (var t in _enemiesCreateType)
            priorities[t.type] = t.priority;
        
        for (int i = 0; i < count - 1; i++)
            Spawn(new(data[i]));
        if (isDifficulty)
            Spawn(new(data[count - 1]));

        return enemies;

        #region Local Functions
        void Spawn(EnemyData enemyData)
        {
            EnemyType type = enemyData.Type;
            if (isDifficulty && type == EnemyType.Orange)
                type = EnemyType.Red;

            foreach (var t in _enemiesCreateType)
            {
                if(type == t.type)
                {
                    Enemy e = Instantiate(t.prefab, enemyData.Position, Quaternion.identity, thisTransform);
                    if (t.type == EnemyType.Gravity)
                        e.Setup(priorities[t.type]++);
                    else
                        (e as EnemyShooting).Setup(_psBulletTargets, priorities[t.type]++);
                    enemies.Add(e);
                }
            }
        }
        #endregion
    }


#if UNITY_EDITOR
    public List<Enemy> CreateTest(List<EnemySaveData> data)
    {
        int count = data?.Count ?? 0;
        if (count == 0)
            return null;

        Transform thisTransform = transform;
        List<Enemy> enemies = new(count);
        Dictionary<EnemyType, int> priorities = new(_enemiesCreateType.Length);
        foreach (var t in _enemiesCreateType)
            priorities[t.type] = t.priority;

        for (int i = 0; i < count; i++)
            Spawn(new(data[i]));

        return enemies;

        #region Local Functions
        void Spawn(EnemyData enemyData)
        {
            foreach (var t in _enemiesCreateType)
            {
                if (enemyData.Type == t.type)
                {
                    Enemy e = Instantiate(t.prefab, enemyData.Position, Quaternion.identity, thisTransform);
                    if (t.type == EnemyType.Gravity)
                        e.Setup(priorities[t.type]++);
                    else
                        (e as EnemyShooting).Setup(_psBulletTargets, priorities[t.type]++);
                    enemies.Add(e);
                }
            }
        }
        #endregion
    }
    public void Create()
    {
        Delete();

        int count = _enemiesData?.Count ?? 0;
        if (count == 0)
            return;

        Transform thisTransform = transform;
        foreach (var e in _enemiesData)
            Spawn(e);

        #region Local Functions
        void Spawn(EnemyData enemyData)
        {
            foreach (var t in _enemiesCreateType)
            {
                if (enemyData.Type == t.type)
                {
                    Enemy e = Instantiate(t.prefab, enemyData.Position, Quaternion.identity, thisTransform);
                    if (t.type == EnemyType.Gravity)
                        e.Setup(t.priority++);
                    else
                        (e as EnemyShooting).Setup(_psBulletTargets, t.priority++);
                }
            }
        }
        #endregion
    }
    public void Delete()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
    public List<EnemySaveData> Get()
    {
        List<EnemySaveData> enemies = new(_enemiesData.Count);
        foreach (var s in _enemiesData)
            enemies.Add(new(s));
        return enemies;
    }
    public void LoadData(List<EnemySaveData> data)
    {
        _enemiesData.Clear();
        if (data == null || data.Count == 0)
            return;

        foreach(var s in data)
            _enemiesData.Add(new(s));
    }
    public void ClearData()
    {
        _enemiesData.Clear();
    }
#endif

    [System.Serializable]
    private class EnemyCreateType
    {
        public EnemyType type;
        public int priority;
        public Enemy prefab;
    }
}
