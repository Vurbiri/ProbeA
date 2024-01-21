#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemiesSpawner))]
public class EnemiesOvermind : MonoBehaviour
{
    [SerializeField] private Probe _probe;
    [SerializeField] private float _patrolDistance = 10f;
    [SerializeField] private float _huntDistance = 10f;
    [SerializeField] private float _difficultyDistance = 1f;
    [SerializeField] private Vector2 _navArea = new(30, 20);
    [SerializeField] private float _rateUpdate = 0.65f;
    
    private Transform _probeTransform;
    private bool _exit = false;
    private List<Enemy> _enemies;

    private const int maxAttempt = 10;

    public void Create(List<EnemySaveData> data)
    {
        if (data == null || data.Count == 0)
            return;

        _enemies = GetComponent<EnemiesSpawner>().Create(data);

    }

    public void Thinking()
    {
        if(_enemies == null || _enemies.Count == 0) return;
        
        _probeTransform = _probe.transform;

        float huntDistance = (_huntDistance + (GameData.Instance.IsDifficulty? _difficultyDistance : 0f)) * PlayerStates.Instance.Stealth;

        WaitForSeconds pause = new(_rateUpdate);
        WaitForFixedUpdate pauseFixedUpdate = new();

        _exit = false;
        foreach (Enemy enemy in _enemies)
        {
            if (enemy is EnemyShooting)
                (enemy as EnemyShooting).EventDamage += _probe.OnDamage;

            StartCoroutine(FindTarget(enemy));
        }

        _enemies.Clear();
        _enemies = null;

        #region Local Functions
        IEnumerator FindTarget(Enemy enemy)
        {
            yield return new WaitForSeconds(Random.Range(_rateUpdate * 0.75f, _rateUpdate * 1.5f));
            
            NavMeshHit navMeshHit;
            int attempt = 0;

            while (true)
            {
                if (_exit)
                {
                    enemy.Stop();
                    break;
                }

                if (!enemy.PermitHunting || Vector3.Distance(enemy.Position, _probeTransform.position) > huntDistance)
                {
                    if (enemy.IsNotPatch || !enemy.IsPatrol || float.IsInfinity(enemy.RemainingDistance) || enemy.RemainingDistance < 0.01f)
                    {
                        attempt = 0;
                        while (!NavMesh.SamplePosition(RandomPoint(enemy.Position), out navMeshHit, enemy.NavHeight * 2f, NavMesh.AllAreas) && attempt++ < maxAttempt)
                            yield return pauseFixedUpdate;

                        enemy.Patrol(navMeshHit.position);
                    }
                }
                else
                {
                    enemy.Hunt(_probeTransform.position);
                }

                yield return pause;
            }
        }

        Vector3 RandomPoint(Vector3 center)
        {
            float x = _navArea.x / 2f, z = _navArea.y / 2f;
            Vector3 randomPoint = center + Random.insideUnitCircle.To3D() * _patrolDistance;
            if (randomPoint.x < -x || randomPoint.x > x)
                randomPoint.x *= -1;
            if (randomPoint.z < -z || randomPoint.z > z)
                randomPoint.z *= -1;

            return randomPoint;
        }
        #endregion
    }

    public void Stop()
    {
        _exit = true;
    }

#if UNITY_EDITOR

    public void CreateTest(List<EnemySaveData> data)
    {
        GetComponent<EnemiesSpawner>().CreateTest(data);
    }
    [Button]
    public void Create()
    {
        GetComponent<EnemiesSpawner>().Create();
    }
    [Button]
    public void Delete()
    {
        GetComponent<EnemiesSpawner>().Delete();
    }
    public List<EnemySaveData> Get()
    {
        return GetComponent<EnemiesSpawner>().Get();
    }
    public void LoadData(List<EnemySaveData> data)
    {
        GetComponent<EnemiesSpawner>().LoadData(data);
    }
    public void ClearData()
    {
        GetComponent<EnemiesSpawner>().ClearData();
    }
#endif
}
