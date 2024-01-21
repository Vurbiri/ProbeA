using UnityEngine;
using UnityEngine.UI;

public class ControlToggleGroup : ToggleGroup
{
    [SerializeField] private Toggle _toggleFollowing;
    [SerializeField] private Toggle _toggleScreenStick;

    protected override void Awake()
    {
        base.Awake();

        allowSwitchOff = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _toggleScreenStick.isOn = SettingsGame.Instance.IsScreenStick;
        _toggleFollowing.isOn = !SettingsGame.Instance.IsScreenStick;

        _toggleScreenStick.onValueChanged.AddListener((b) => SettingsGame.Instance.IsScreenStick = b);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _toggleScreenStick.onValueChanged.RemoveAllListeners();
    }

}
