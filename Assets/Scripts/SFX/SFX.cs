using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(ParticleSystem))]
public class SFX : MonoBehaviour
{
    protected AudioSource _thisAudioSource;
    protected ParticleSystem _thisParticleSystem;

    private void Awake()
    {
        _thisAudioSource = GetComponent<AudioSource>();
        _thisParticleSystem = GetComponent<ParticleSystem>();
    }


    public void Setup()
    {
        if (_thisAudioSource == null)
            _thisAudioSource = GetComponent<AudioSource>();
        if (_thisParticleSystem == null)
            _thisParticleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        _thisAudioSource.Play();
        _thisParticleSystem.Play();
    }

    public void Stop()
    {
        _thisAudioSource.Stop();
        _thisParticleSystem.Stop();
    }

    public void Clear()
    {
        _thisParticleSystem.Clear();
    }

    public void Pause()
    {
        _thisAudioSource.Pause();
        _thisParticleSystem.Pause();
    }

    public void UnPause()
    { 
        _thisAudioSource.UnPause();
        _thisParticleSystem.Play();
    }

}
