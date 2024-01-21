using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyGravityField : MonoBehaviour
{
    [SerializeField] private EnemyGravitySFX _attractive;
    [SerializeField] private EnemyGravitySFX _repulsive;
    [Space]
    [SerializeField] private Enemy _enemy;
    [Space]
    [SerializeField] private float _maxDistance = 2.5f;
    [SerializeField] private float _minDistance = 1f;
    [SerializeField] private float _chargeSpeed = 0.4f;
    [Space]
    [SerializeField] private float _preTimeRepulsive = 0.5f;
    [SerializeField] private float _postTimeRepulsive = 10f;
    [Space]
    [SerializeField] private float _forceAttractive = 225f;
    [SerializeField] private float _forceRepulsive = 22500f;
    
    public event Action<float> EventColorUpdate;

    private Transform _thisTransform;
    private BoxCollider _thisBoxCollider;
    private Rigidbody _probeRigidbody;
    private Transform _probeTransform;

    private ParticleSystem.MainModule _mainPS;
    private float _alfaPS;
    private Color _colorBlack;
    private float _charge = 0f;
    private float _chargeStep = 0f;

    private WaitForSeconds _prePauseRepulsive;
    private WaitForSeconds _postPauseRepulsive;

    private Vector3 _difference;
    private float _distance;
    private Vector3 _direction;

    void Start()
    {
        _thisTransform = transform;
        _thisBoxCollider = GetComponent<BoxCollider>();
        _mainPS = _attractive.Main;

        _alfaPS = _mainPS.startColor.color.a;
        
        _prePauseRepulsive = new(_preTimeRepulsive);
        _postPauseRepulsive = new(_postTimeRepulsive);
        
        _colorBlack = new Color(0f, 0f, 0f, _alfaPS);

        _thisBoxCollider.size = Vector3.one * (_enemy.RangeAttack * 2f);
        _mainPS.startSize = _enemy.RangeAttack * 3f;
        
        _attractive.Play();
        _repulsive.Stop();
        
        _enemy.EventStop += OnStop;
    }

    public void OnStop()
    {
        _attractive.Stop();
        _attractive.Clear();
        _repulsive.Stop();
        _repulsive.Clear();
        EventColorUpdate?.Invoke(0f);

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_probeRigidbody != null)
            return;

        _probeRigidbody = other.attachedRigidbody;
        _probeTransform = other.transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_charge >= 1f || (_probeRigidbody == null))
            return;

        CalculateVectors();

        if (_distance < _maxDistance)
        {
            _chargeStep += (_chargeSpeed * Time.fixedDeltaTime);
            if (_chargeStep >= 0.05f)
            {
                _charge += _chargeStep;
                _chargeStep = 0f;
                _mainPS.startColor = new Color(_charge, _charge, _charge, _alfaPS);
                EventColorUpdate?.Invoke(_charge);
            }
        
            if (_charge >= 1f)
            {
                StartCoroutine(Repulse());
                return;
            }
        }

        if (_distance < _minDistance)
            return;

        _probeRigidbody.AddForce(_direction * _forceAttractive / _distance);

        IEnumerator Repulse()
        {
            _attractive.Stop();
            _thisBoxCollider.enabled = false;
            yield return _prePauseRepulsive;

            _repulsive.Play();
            yield return new WaitForSeconds(_distance / 7.5f);
            CalculateVectors();
            _probeRigidbody.AddForce(-_direction * _forceRepulsive / _distance);

            _mainPS.startColor = _colorBlack;
            EventColorUpdate?.Invoke(0f);
            _repulsive.Stop();
            _enemy.PermitHunting = false;

            yield return _postPauseRepulsive;
            _charge = 0f;
            _thisBoxCollider.enabled = true;
            _attractive.Play();
            _enemy.PermitHunting = true;
        }

        void CalculateVectors()
        {
            _difference = _thisTransform.position - _probeTransform.position;
            _distance = _difference.magnitude;
            _difference.y = 0;
            _direction = _difference.normalized;
        }
    }

    private void OnDisable()
    {
        _enemy.EventStop -= OnStop;
    }
}
