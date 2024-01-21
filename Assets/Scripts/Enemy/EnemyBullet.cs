using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private bool _isDebuff = false;
    [Space]
    [SerializeField] private string _tagProbe = "Probe";

    private ParticleSystem _thisPS;
    private AudioSource _thisAS;
    private List<ParticleCollisionEvent> collisionEvents;

    public event Action<int, bool> EventHit;

    void Start()
    {
        collisionEvents = new();
        _thisPS = GetComponent<ParticleSystem>();
        _thisAS = GetComponent<AudioSource>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(_tagProbe))
            EventHit?.Invoke(_thisPS.GetCollisionEvents(other, collisionEvents), _isDebuff);
        else
            _thisAS.Play();
    }
}
