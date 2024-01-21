using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(ParticleSystem))]
public class ArtefactSFX : MonoBehaviour
{
    private AudioSource _thisAudioSource;
    private ParticleSystem _thisParticleSystem;

    public void Setup()
    {
        _thisAudioSource = GetComponent<AudioSource>();
        _thisParticleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        _thisAudioSource.Play();
        _thisParticleSystem.Play();
    }
}
