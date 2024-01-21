using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSound : MonoBehaviour
{
    [SerializeField] private MixerGroup _audioMixerGroup;

    protected Slider _thisSlider;

    protected virtual void Awake() => _thisSlider = GetComponent<Slider>();

    private void Start()
    {
        SettingsGame settings = SettingsGame.Instance;

        _thisSlider.minValue = settings.MinValue;
        _thisSlider.maxValue = settings.MaxValue;
        _thisSlider.onValueChanged.AddListener((v) => settings.SetVolume(_audioMixerGroup, v));
    }

    private void OnEnable() => _thisSlider.value = SettingsGame.Instance.GetVolume(_audioMixerGroup);
}
