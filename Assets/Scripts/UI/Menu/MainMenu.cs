using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MenuNavigation
{
    [Space]
    [SerializeField] private string _keyGuestName = "Guest";
    [SerializeField] private string _keyAnonymName = "Anonym";
    [SerializeField] private Texture _avatarGuest;
    [SerializeField] private Texture _avatarAnonym;
    [Space]
    [SerializeField] private Button _leaderboard;
    [SerializeField] private Button _review;
    [Space]
    [SerializeField] private RawImage _avatar;
    [SerializeField] private TMP_Text _name;

    private YandexSDK _ysdk;
    private Localization _localization;

    private void Start()
    {
        _ysdk = YandexSDK.InstanceF;
        _localization = Localization.InstanceF;

        Personalization().Forget();
        ButtonInitialize().Forget();

        async UniTaskVoid ButtonInitialize()
        {
            _leaderboard.interactable = _ysdk.IsLeaderboard;
            _review.interactable = !YMoney.Instance.IsFirstStart && _ysdk.IsLogOn && await _ysdk.CanReview();
        }

        async UniTaskVoid Personalization()
        {
            if (_ysdk.IsLogOn)
            {
                string name = _ysdk.PlayerName;
                if (!string.IsNullOrEmpty(name))
                    _name.text = name;
                else
                    _name.text = _localization.GetText(_keyAnonymName);

                var (result, texture) = await _ysdk.GetPlayerAvatar(AvatarSize.Medium);
                if (result)
                    _avatar.texture = texture;
                else
                    _avatar.texture = _avatarAnonym;

            }
            else
            {
                _name.text = _localization.GetText(_keyGuestName);
                _avatar.texture = _avatarGuest;
            }
            _localization.EventSwitchLanguage += SetLocalizationName;
        }
    }

    public void OnReward()
    {
        _review.interactable = false;
        _ysdk.RequestReview().Forget();
    }

    public void SetLocalizationName()
    {
        if (_ysdk.IsLogOn)
        {
            string name = _ysdk.PlayerName;
            if (!string.IsNullOrEmpty(name))
                _name.text = name;
            else
                _name.text = _localization.GetText(_keyAnonymName);
        }
        else
        {
            _name.text = _localization.GetText(_keyGuestName);
        }
    }

    private void OnDestroy()
    {
        if(Localization.Instance != null)
            _localization.EventSwitchLanguage -= SetLocalizationName;
    }

}
