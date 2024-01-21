using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PointsCountText : MonoBehaviour
{
    private TMP_Text _textCount;
    private PlayerStates _states;

    private void Awake()
    {
        _states = PlayerStates.InstanceF;
        _textCount = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _textCount.text = _states.Points.ToString();
        _states.EventChangePoints += GetPoints;
    }

    private void OnDisable()
    {
        if(PlayerStates.Instance != null)
            _states.EventChangePoints -= GetPoints;
    }


    private void GetPoints(int points) => _textCount.text = points.ToString();
}
