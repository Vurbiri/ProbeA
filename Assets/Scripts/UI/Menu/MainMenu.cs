#if YSDK
#endif
using TMPro;
using UnityEngine;

public class MainMenu : MenuNavigation
{
    [Space]
    [SerializeField] private TMP_Text _name;


    private void Start()
    {
        SetLocalizationName();
        Localization.Instance.EventSwitchLanguage += SetLocalizationName;
    }

    void SetLocalizationName()
    {
        _name.text = Localization.Instance.GetText("Guest");
    }


    private void OnDestroy()
    {
        if(Localization.Instance != null)
            Localization.Instance.EventSwitchLanguage -= SetLocalizationName;
    }

}
