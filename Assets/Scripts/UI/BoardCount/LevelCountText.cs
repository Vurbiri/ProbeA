using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LevelCountText : MonoBehaviour
{
    private TMP_Text _textCount;

    private void Awake()
    {
        _textCount = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _textCount.text = PlayerStates.Instance.Level;
    }
}
