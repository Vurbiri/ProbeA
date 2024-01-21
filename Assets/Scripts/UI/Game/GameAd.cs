using UnityEngine;

public class GameAd : MonoBehaviour
{
    YMoney _yMoney;
    Game _game;

    private void OnEnable()
    {
        (_yMoney = YMoney.InstanceF).ShowBannerAdv();
        (_game = Game.InstanceF).EventGameCompleted += ResetLevelShowAd;
        _game.EventGameOver += ResetLevelShowAd;
    }

    private void OnDisable()
    {
        if(YMoney.Instance != null)
            _yMoney.HideBannerAdv();
        if (Game.Instance != null)
        {
            _game.EventGameCompleted -= ResetLevelShowAd;
            _game.EventGameOver -= ResetLevelShowAd;
        }
    }

    private void ResetLevelShowAd(int i) => _yMoney.ResetLevelShowAd();
}
