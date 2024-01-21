using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProbeSFX : MonoBehaviour
{
    [SerializeField] private AudioSource _probeMotorAudioSource;
    [Space]
    [SerializeField] private GameObject _psExplosion;
    [SerializeField] private GameObject _probeMesh;
    [Space]
    [SerializeField] private float _pitchMotorMin = 0.75f;
    [SerializeField] private float _pitchMotorMax = 1.5f;
    [SerializeField] private float _pitchMotorAddMin = -0.1f;
    [SerializeField] private float _pitchMotorAddMax = 0.1f;
    [SerializeField] private float _pitchTimeWave = 3f;
    [Space]
    [SerializeField] private AudioClip _bulletHit;
    [SerializeField, Range (0f, 1f)] private float _bulletHitValue = 1f;
    [SerializeField] private AudioClip _bulletHitDebuff;
    [SerializeField, Range(0f, 1f)] private float _bulletHitDebuffValue = 1f;
    [Space]
    [SerializeField] private AudioClip _knock;
    [SerializeField, Range(0f, 1f)] private float _knockValue = 0.55f;
    [SerializeField] private AudioClip _knockObstacles;
    [SerializeField, Range(0f, 1f)] private float _knockObstaclesValue = 0.45f;
    [Space]
    [SerializeField] private AudioClip _levelCompleted;
    [SerializeField, Range(0f, 1f)] private float _levelCompletedValue = 0.85f;

    private AudioSource _thisAudioSource;
    
    private float _pitchScale;
    private float _pitchAdd;
    private const float step = 30f;
    private WaitForSeconds _stepTime;
    private float _pitchStep;

    public void Start()
    {
        _thisAudioSource = GetComponent<AudioSource>();

        _pitchScale = _pitchMotorMax - _pitchMotorMin;
        _stepTime = new(_pitchTimeWave / step);
        _pitchStep = (_pitchMotorAddMax - _pitchMotorAddMin) / step;

        _probeMesh.SetActive(true);
    }

    public void Explosion(Vector3 position)
    {
        _probeMotorAudioSource.Stop();
        _probeMesh.SetActive(false);
        Instantiate(_psExplosion, position, Quaternion.identity);
    }

    public void LevelCompleted()
    {
        _probeMotorAudioSource.Stop();
        _thisAudioSource.PlayOneShot(_levelCompleted, _levelCompletedValue);
    }

    public void ProbeMotor(float velocity)
    {
        _probeMotorAudioSource.pitch = velocity * _pitchScale + _pitchMotorMin + _pitchAdd;
    }

    public void ProbeMotorPlay()
    {
        if (!_probeMotorAudioSource.isPlaying)
        {
            _probeMotorAudioSource.Play();
            StartCoroutine(ProbeMotorPitchWave());
        }
    }

    public void ProbeMotorStop()
    {
         _probeMotorAudioSource.Stop();
    }

    public IEnumerator ProbeMotorPitchWave()
    {
        float pitchStep = _pitchStep;
        _pitchAdd = Random.Range(_pitchMotorAddMin, _pitchMotorAddMax);

        while (_probeMotorAudioSource.isPlaying)
        {
            yield return _stepTime;

            if(_pitchAdd > _pitchMotorAddMax)
                pitchStep = -_pitchStep;
            else if(_pitchAdd < _pitchMotorAddMin) 
                pitchStep = _pitchStep;

            _pitchAdd += pitchStep;
        }
    }


    public void PlayKnockObstacles(float rate = 1f)
    {
        float strength = _knockObstaclesValue * rate;
        _thisAudioSource.PlayOneShot(_knockObstacles, strength);
    }
    public void PlayKnock(float rate = 1f)
    {
        float strength = _knockValue * rate;
        _thisAudioSource.PlayOneShot(_knock, strength);
    }

    public void PlayBulletHit(bool isDebuff)
    {
        if (isDebuff)
            _thisAudioSource.PlayOneShot(_bulletHitDebuff, _bulletHitDebuffValue);
        else
            _thisAudioSource.PlayOneShot(_bulletHit, _bulletHitValue);
    }
}
