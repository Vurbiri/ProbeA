using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScreenOnOff : MonoBehaviour
{
    [SerializeField] private Animator _animatorTopPanel;
    [SerializeField] private Animator _animatorBottomPanel;
    [SerializeField] private string _animatoParameter = "isClose";
    private AudioSource _thisAudioSource;
    private readonly WaitForSecondsRealtime delay = new(0.22f);

    Coroutine _coroutine = null;

    private void Awake()
    {
        _thisAudioSource = GetComponent<AudioSource>();
    }

    public void Switch(bool isClose)
    {
        StopControl();
        _coroutine = StartCoroutine(Control(isClose));
    }

    public void Open()
    {
        StopControl();
        _coroutine = StartCoroutine(Control(false));
    }
    public void Close()
    {
        StopControl();
        _coroutine = StartCoroutine(Control(true));
    }

    private IEnumerator Control(bool isClose)
    {
        _thisAudioSource.Play();
        yield return delay;
        _animatorTopPanel.SetBool(_animatoParameter, isClose);
        _animatorBottomPanel.SetBool(_animatoParameter, isClose);
        _coroutine = null;
    }

    private void StopControl()
    {
        if (_coroutine != null) 
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
