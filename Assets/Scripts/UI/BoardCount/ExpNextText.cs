using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ExpNextText : MonoBehaviour
{
    [SerializeField] private string _cap = "-----";
    private TMP_Text _textCount;
    private PlayerStates Player => PlayerStates.Instance;

    private void Awake()
    {
        _textCount = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if(Player.IsCap)
            _textCount.text = _cap;
        else
            _textCount.text = Player.ExpToLevel;
    }
}
