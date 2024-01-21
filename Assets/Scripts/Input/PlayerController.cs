using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private Transform _probe;
    [SerializeField] private RectTransform _screen;
    [SerializeField] private Button _buttonMenu;
    [Space]
    [SerializeField] private float _deadZoneDefault = 0.15f;
    [Header("Move to cursor")]
    [SerializeField] private float _deadZoneMove = 0.35f;
    [SerializeField] private float _scaleMove = 1.5f;
    [Header ("ScreenStick")]
    [SerializeField] private float _deadZoneScreenStick = 0.3f;
    [SerializeField] private float _scaleScreenStick = 2.25f;

    #region Private
    private SettingsGame _settings;
    private Camera _camera;

    private InputActions _inputActions;
    private InputAction _actionPosition;

    private Vector2 _position;
    private Coroutine _coroutinePosition;
    #endregion

    #region Public
    public Vector3 Torque { get; private set; } = Vector3.zero;
    public float TorqueMagnitude { get; private set; } = 0;
    public bool IsGameplay => _inputActions.Gameplay.enabled;
    #endregion

    #region Events
    public event Action<Vector2> EventPressStarted;
    public event Action EventPressCanceled;
    public event Action<Vector2, float> EventRotation;
    public event Action EventOpenMenu;
    public event Action EventCloseMenu;
    #endregion

    protected override void Awake()
    {
        _isNotDestroying = false;
        base.Awake();

        _settings = SettingsGame.Instance;
        _camera = Camera.main;

        _inputActions = new();
        _actionPosition = _inputActions.Gameplay.Position;
    }

    private void Start()
    {
        _inputActions.Enable();

        _inputActions.Gameplay.Torque.performed += (ctx) => ConvertToTorque(ctx.ReadValue<Vector2>(), _deadZoneDefault);
        _inputActions.Gameplay.Torque.canceled += _ => TorqueZero();

        _inputActions.Gameplay.Press.started += OnPressStarted;
        _inputActions.Gameplay.Press.canceled += OnPressCanceled;

        _inputActions.Gameplay.Menu.performed += _ => EventOpenMenu?.Invoke();
        _inputActions.UI.Menu.performed += _ => EventCloseMenu?.Invoke();
        _buttonMenu.onClick.AddListener(()=> { if (IsGameplay) EventOpenMenu?.Invoke(); else EventCloseMenu?.Invoke(); });
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        InputSystem.ResetHaptics();
    }

    public void DisableMaps()
    {
        _inputActions.Gameplay.Disable();
        _inputActions.UI.Disable();
    }
    public void EnableGameplayMap()
    {
        _inputActions.Gameplay.Enable();
        _inputActions.UI.Disable();
    }
    public void EnableUIMap()
    {
        _inputActions.Gameplay.Disable();
        _inputActions.UI.Enable();
    }



    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        _position = _actionPosition.ReadValue<Vector2>();
        if (!RectTransformUtility.RectangleContainsScreenPoint(_screen, _position, Camera.main))
            return;

        if (_settings.IsScreenStick)
        {
            EventPressStarted?.Invoke(_position);
            _actionPosition.performed += OnPosition;
        }
        else
        {
            _coroutinePosition = StartCoroutine(PressPosition());
        }
    }
    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        TorqueZero();
        _position = Vector2.zero;
        if (_settings.IsScreenStick)
        {
            _actionPosition.performed -= OnPosition;
            EventPressCanceled?.Invoke();
        } 
        else if(_coroutinePosition != null)
        { 
            StopCoroutine(_coroutinePosition);
            _coroutinePosition = null;
        }
    }

    private IEnumerator PressPosition()
    {
        while (true)
        {
            ConvertToScaleWorldTorque(_position - (Vector2)_camera.WorldToScreenPoint(_probe.position), _deadZoneMove, _scaleMove);
            yield return null;
            _position = _actionPosition.ReadValue<Vector2>();
        }
    }
    private void OnPosition(InputAction.CallbackContext ctx)
    {
        Vector2 rotation = ctx.ReadValue<Vector2>() - _position;
        ConvertToScaleWorldTorque(rotation, _deadZoneScreenStick, _scaleScreenStick);
        EventRotation?.Invoke(rotation, TorqueMagnitude);
    }

    private void ConvertToScaleWorldTorque(Vector2 vector, float dead, float scale = 1f)
    {
        float ratio = Screen.height / (_camera.orthographicSize * 2f);
        ConvertToTorque(vector / ratio, dead, scale);
    }

    private void ConvertToTorque(Vector2 vector, float dead, float scale = 1f)
    {
        Vector3 torque = (new Vector3(vector.y, 0f, -vector.x)) / scale;
        float magnitude = (torque.magnitude - dead) / (0.999f - dead);

        if (magnitude <= 0f)
            TorqueZero();
        else if (magnitude < 1f)
            TorqueSetup(torque, magnitude);
        else
            TorqueSetup(torque.normalized, 1f);
    }

    private void TorqueZero()
    {
        Torque = Vector3.zero;
        TorqueMagnitude = 0f;
    }

    private void TorqueSetup(Vector3 torque, float magnitude)
    {
        Torque = torque;
        TorqueMagnitude = magnitude;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(_buttonMenu != null)
            _buttonMenu.onClick.RemoveAllListeners();
    }

    //public void PlayHaptics(float time, float lowFrequency, float highFrequency)
    //{
    //    if (!(Gamepad.current != null && Gamepad.current.IsActuated()))
    //        return;

    //    StartCoroutine(Play());

    //    IEnumerator Play()
    //    {
    //        Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
    //        yield return new WaitForSecondsRealtime(time);
    //        Gamepad.current.ResetHaptics();

    //        //Gamepad.current.PauseHaptics();
    //        //Gamepad.current.ResumeHaptics();
    //    }
    //}
}
