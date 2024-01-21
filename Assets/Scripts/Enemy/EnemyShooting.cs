using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyShooting : Enemy
{
    [Header("Shooting")]
    [SerializeField] private float _damage = 1f;
    [SerializeField] private int _maxBullets = 10;
    [SerializeField] private float _spawnBulletsTime = 1.35f;
    [SerializeField] private float _spawnBulletsDistance = 0.1f;
    [Space]
    [SerializeField] private EnemyBullet _bullet;
    [Space]
    [SerializeField] private ParticleSystem _psBullet;
    [SerializeField] private string _psBulletTargetTag = "PS_Force_Red";

    public event Action<float, bool> EventDamage;

    public float Damage => _damage;
    public float MaxBullets => _maxBullets;
    public float SpawnBulletsTime => _spawnBulletsTime;

    public Enemy Setup(ParticleSystemForceField[] psBulletTargets, int priority)
    {
        base.Setup(priority);

        foreach (var ff in psBulletTargets)
        {
            if(ff.gameObject.CompareTag(_psBulletTargetTag))
            {
                ff.endRange = RangeAttack;
                _psBullet.externalForces.RemoveAllInfluences();
                _psBullet.externalForces.AddInfluence(ff);
                break;
            }
        }

        _bullet.EventHit += OnEventHit;
        EventStop += OnStop;

        ParticleSystem.MainModule main = _psBullet.main;
        main.maxParticles = _maxBullets;

        ParticleSystem.EmissionModule emission = _psBullet.emission;
        emission.rateOverTime = _spawnBulletsTime;
        emission.rateOverDistance = _spawnBulletsDistance;

        return this;
    }

    public void OnStop()
    {
        _bullet.EventHit -= OnEventHit;
        EventDamage = null;
        _psBullet.externalForces.RemoveAllInfluences();
        _psBullet.Stop();
        _psBullet.Clear();
    }

    private void OnEventHit(int count, bool isDebuff) => EventDamage?.Invoke(count * _damage, isDebuff);

    private void OnDestroy()
    {
        _bullet.EventHit -= OnEventHit;
        EventDamage = null;
    }
}
