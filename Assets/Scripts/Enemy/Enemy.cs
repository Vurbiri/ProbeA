using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Hunt")]
    [SerializeField] private float _speed = 1.3f;
    [SerializeField] private float _acceleration = 2.25f;
    [SerializeField] private float _stoppingDistance = 1.35f;
    [Header("Patrol")]
    [SerializeField] private float _speedPatrol = 0.97f;
    [SerializeField] private float _accelerationPatrol = 1.2f;
    [SerializeField] private float _stoppingDistancePatrol = 0f;
    [Header("Attack")]
    [SerializeField] private float _rangeAttack = 2.1f;
    

    private NavMeshAgent _thisAgent;
    private Transform _thisTransform;

    public event Action EventStop;

    public float RangeAttack => _rangeAttack;
    public float Speed => _speed;
    public Vector3 Position => _thisTransform.position;
    public float NavHeight => _thisAgent.height;
    public float RemainingDistance => _thisAgent.remainingDistance;
    public bool IsNotPatch => _thisAgent.pathStatus == NavMeshPathStatus.PathInvalid;
    public bool IsPatrol {get; private set;}
    public bool PermitHunting { get; set; } = true;

    public void Setup(int priority)
    {
        _thisTransform = transform;

        _thisAgent = GetComponent<NavMeshAgent>();
        _thisAgent.avoidancePriority = priority;
        SetStateForPatrol();
        PermitHunting = true;
    }

    public void Patrol(Vector3 target)
    {
        if (!IsPatrol)
            SetStateForPatrol();

        _thisAgent.ResetPath();
        _thisAgent.SetDestination(target);
    }

    public void Hunt(Vector3 target)
    {
        if (IsPatrol)
            SetStateForHunt();

        _thisAgent.ResetPath();
        _thisAgent.SetDestination(target);
    }

    public virtual void Stop()
    {
        _thisAgent.ResetPath();
        EventStop?.Invoke();
    }

    private void SetStateForPatrol()
    {
        _thisAgent.speed = _speedPatrol;
        _thisAgent.acceleration = _accelerationPatrol;
        _thisAgent.stoppingDistance = _stoppingDistancePatrol;
        IsPatrol = true;
    }
    private void SetStateForHunt()
    {
        _thisAgent.speed = _speed;
        _thisAgent.acceleration = _acceleration;
        _thisAgent.stoppingDistance = _stoppingDistance;
        IsPatrol = false;
    }

}
