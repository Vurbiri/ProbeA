using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _text;
    [Space]
    [SerializeField] private Material _materialBar;
    [Space]
    [SerializeField] private float _intensityEmission = 1.2f;
    [SerializeField] private Color _colorNormal = Color.green;
    [SerializeField] private Color _colorWarning = Color.yellow;
    [SerializeField] private float _warning = 0.66f;
    [SerializeField] private Color _colorDanger = Color.red;
    [SerializeField] private float _danger = 0.33f;

    private Color _colorEmissionNormal;
    private Color _colorEmissionWarning;
    private Color _colorEmissionDanger;

    private float _maxHP;

    private void Start()
    {
        GameData gameData = GameData.InstanceF;
        float currentHP = gameData.HPCurrent;
        _maxHP = gameData.HPFull;
        
        _slider.maxValue = Mathf.Max(_maxHP, currentHP);

        _colorEmissionNormal = _colorNormal * _intensityEmission;
        _colorEmissionWarning = _colorWarning * _intensityEmission;
        _colorEmissionDanger = _colorDanger * _intensityEmission;

        OnChangeValue(currentHP);
        gameData.EventChangeCurrentHP += OnChangeValue;
    }

    private void OnChangeValue(float currentHP)
    {
        if (currentHP < 0) currentHP = 0;
        
        _slider.value = currentHP;
        _text.text = ((int)Mathf.Round(currentHP)).ToString();
        SetMaterial(currentHP);
    }

    private void SetMaterial(float currentHP)
    {
        float present = currentHP / _maxHP;

        if (present > _warning) 
        {
            _materialBar.color = _colorNormal;
            _materialBar.SetEmission(_colorEmissionNormal);
        }
        else if (present > _danger)
        {
            _materialBar.color = _colorWarning;
            _materialBar.SetEmission(_colorEmissionWarning);
        }
        else
        {
            _materialBar.color = _colorDanger;
            _materialBar.SetEmission(_colorEmissionDanger);
        }
    }

    private void OnDestroy()
    {
        if (GameData.Instance == null)
            return;

        GameData.Instance.EventChangeCurrentHP -= OnChangeValue;
    }
}
