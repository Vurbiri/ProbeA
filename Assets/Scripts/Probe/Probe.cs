using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Probe : MonoBehaviour
{
    [SerializeField] private ProbeSFX _SFX;

    private float Speed => _player.Speed * _rateSpeedDebuff;
    private float _rateSpeedDebuff = 1f;

    private Coroutine _coroutine;
    private readonly WaitForSeconds _timeDebuff = new(5f);
    private Rigidbody _thisRigidbody;
    
    private PlayerStates _player;
    private GameData _gameData;
    private PlayerController _controller;
    private Game _game;

    private const string _tagObstacles = "Obstacles";
    private const string _tagEnvironment = "Environment";
    private const string _tagPlatforms = "Platforms";
    private const string _tagGround = "Ground";


    private void Awake()
    {
        _player = PlayerStates.Instance;
        _gameData = GameData.Instance;
        _controller = PlayerController.InstanceF;
        _game = Game.InstanceF;

        _thisRigidbody = GetComponent<Rigidbody>();

        _game.EventPause += SoundMotorSwitch;
        _game.EventLevelCompleted += OnLevelCompleted;
        _game.EventGameOver += OnGameOver;
    }

    private void OnGameOver(int i)
    {
        _SFX.Explosion(transform.position);
        this.enabled = false;
    }

    private void OnLevelCompleted()
    {
        _SFX.LevelCompleted();
        this.enabled = false;
    }

    public void OnDamage(float damage, bool isDebuff)
    {
        if (_gameData.HPCurrent <= 0 || !_controller.IsGameplay)
            return;

        _gameData.HPCurrent -= damage;

        _SFX.PlayBulletHit(isDebuff);

        if (!isDebuff)
            return;

        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Debuff());

        IEnumerator Debuff()
        {
            _thisRigidbody.velocity = Vector3.zero;
            _rateSpeedDebuff = _player.Debuff;
            yield return _timeDebuff;
            _rateSpeedDebuff = 1f;

            _coroutine = null;
        }
    }

    private void SoundMotorSwitch(bool isStop)
    {
        if(isStop)
            _SFX.ProbeMotorStop();
        else
            _SFX.ProbeMotorPlay();
    }


    private void FixedUpdate()
    {
        if (_gameData.HPCurrent <= 0 || !_controller.IsGameplay)
        {
            _SFX.ProbeMotorStop();
            return;
        }

        float magnitude = _controller.TorqueMagnitude;
        
        if (magnitude == 0f)
        {
            _SFX.ProbeMotorStop();
        }
        else
        {
            _thisRigidbody.AddTorque(_controller.Torque * Speed, ForceMode.VelocityChange);

            _SFX.ProbeMotor(magnitude);
            _SFX.ProbeMotorPlay();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gObj = collision.gameObject;
        float velocity = _thisRigidbody.velocity.magnitude * 0.1f;
        const float verticalSpeed = -0.2f;

        if (gObj.CompareTag(_tagObstacles))
            _SFX.PlayKnockObstacles(velocity);
        else if (gObj.CompareTag(_tagEnvironment))
            _SFX.PlayKnock(velocity);
        else if (gObj.CompareTag(_tagPlatforms))
            PlayOfAngle();
        else if (gObj.CompareTag(_tagGround))
            PlayOfHeight();

        void PlayOfAngle()
        {
            Vector3 normal = collision.GetContact(0).normal;
            float dot = Mathf.Abs(Vector3.Dot(Vector3.up, normal));
            if (dot < 0.01f || (dot > 0.99f && _thisRigidbody.velocity.y < verticalSpeed))
            {
                _SFX.PlayKnock(velocity);
            }
        }
        void PlayOfHeight()
        {
            if (_thisRigidbody.velocity.y < verticalSpeed)
            {
                _SFX.PlayKnock(velocity);
            }
        }
    }

    private void OnDisable()
    {
        if (Game.Instance == null) return;
        
        _game.EventPause -= SoundMotorSwitch;
        _game.EventLevelCompleted -= OnLevelCompleted;
        _game.EventGameOver -= OnGameOver;
    }
}
