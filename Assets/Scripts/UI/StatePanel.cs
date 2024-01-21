using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel : MonoBehaviour
{
    [SerializeField] private StateType _stateType;
    [Space]
    [SerializeField] private TextLocalization _name;
    [Space]
    [SerializeField] private TMP_Text _levelText;
    [Space]
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;

    private PlayerStates _states;

    private void Awake()
    {
        _states = PlayerStates.InstanceF;
    }

    private void Start()
    {
        _name.Setup(_stateType.ToString());

        _minusButton.onClick.AddListener(() => _states.LevelDown(_stateType));
        _plusButton.onClick.AddListener(() => _states.LevelUp(_stateType));
    }

    private void OnEnable()
    {
        SetLevelBoard(_states.StateLevel(_stateType));
        _states.SubscribeEventChangeLevel(_stateType, SetLevelBoard);

        SetButtons(_states.Points);
        _states.EventChangePoints += SetButtons;
    }

    private void OnDisable()
    {
        if (PlayerStates.Instance == null) return;
        
        _states.UnsubscribeEventChangeLevel(_stateType, SetLevelBoard);
        _states.EventChangePoints -= SetButtons;
    }

    private void SetLevelBoard(int level) => _levelText.text = level.ToString();

    private void SetButtons(int points)
    {
        _minusButton.interactable = _states.IsStateLevelUp(_stateType);
        _plusButton.interactable = !_states.IsStateCap(_stateType) && points > 0;
    }
}
