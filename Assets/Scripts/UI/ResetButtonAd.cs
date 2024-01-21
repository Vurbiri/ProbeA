using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResetButtonAd : MonoBehaviour
{
    private Button _thisButton;
    private PlayerStates _states;
    
    private void Awake()
    {
        _thisButton = GetComponent<Button>();
        _states = PlayerStates.InstanceF;

        _thisButton.interactable = _states.IsResetPossible;
        _states.EventChangePointsSpent += ButtonInteractable;
    }

    private void ButtonInteractable(bool isPossible)
    {
        _thisButton.interactable = isPossible;
    }

    private void OnDestroy()
    {
        if(PlayerStates.Instance != null)
            _states.EventChangePointsSpent -= ButtonInteractable;
    }
}
