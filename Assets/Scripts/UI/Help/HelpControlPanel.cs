using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelpControlPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panelGamepad;
    [SerializeField] private float _timeCheckGamepad = 1f;

    private WaitForSecondsRealtime _delayRealtime;
    private Coroutine _coroutine = null;

    private void Awake()
    {
        _delayRealtime = new(_timeCheckGamepad);
    }

    private void OnEnable()
    {
        bool isActive = Gamepad.current != null;
        _panelGamepad.SetActive(isActive);
        if (!isActive)
            _coroutine = StartCoroutine(CheckGamepad());

        IEnumerator CheckGamepad()
        {
            while (Gamepad.current == null)
                yield return _delayRealtime;

            _panelGamepad.SetActive(true);
            _coroutine = null;
        }
    }

    private void OnDisable()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
