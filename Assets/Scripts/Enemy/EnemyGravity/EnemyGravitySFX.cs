using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(ParticleSystem))]
public class EnemyGravitySFX : SFX
{
    public ParticleSystem.MainModule Main => _thisParticleSystem.main;
}
