using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelMenuAd : MonoBehaviour
{
    [SerializeField] private GameObject _repairPanel;
    [SerializeField] private TextFormatLocalization _repairHint;

    private GameData _gameData;
    private YMoney _yMoney;

    private float _repair;
    private int _level;

    private void Start()
    {
        _gameData = GameData.InstanceF;
        _yMoney = YMoney.InstanceF;

        _level = _gameData.LevelCurrent - 1;
        _repair = _yMoney.RepairRewardedAd;
        _repairHint.Setup(_repair * 100f);
        _repairPanel.SetActive(false);

        AdSetupAsync().Forget();

        async UniTaskVoid AdSetupAsync()
        {
            
            float deltaHP = _gameData.HPFull - _gameData.HPCurrent;
            bool isRepair = deltaHP > 0.1f;

            if (_yMoney.LevelHardFullscreenAd <= _level || (isRepair && _yMoney.LevelFullscreenAd <= _level))
                if(await _yMoney.ShowFullscreenAdv(_level))
                    await AdFullscreenReward();

            isRepair = isRepair && _gameData.HPFull * (1f - _repair) > _gameData.HPCurrent && _yMoney.LevelRewardedAd <= _level;
            _repairPanel.SetActive(isRepair);
        
            async UniTask AdFullscreenReward()
            {
                if (!isRepair)
                    return;

                float repairAdFull = _yMoney.RepairFullscreenAd;
                if (deltaHP < repairAdFull)
                    repairAdFull = deltaHP;

                UniTaskCompletionSource<bool> taskEndSave = new();
                _gameData.HPCurrent += repairAdFull;
                _gameData.Save(true, (b) => taskEndSave.TrySetResult(b));
                if (await taskEndSave.Task)
                    Message.Repair(repairAdFull, true);
                else
                    Message.BannerKey("ErrorSave", MessageType.Error, 4500);
            }
        }
    }

    public void OnRepairForAd()
    {
        OnRepairForAdAsync().Forget();

        async UniTaskVoid OnRepairForAdAsync()
        {
            UniTaskCompletionSource<bool> taskEndSave = new();
            float hp = _gameData.HPFull * _repair;
            bool resultSave = false;

            bool result = await _yMoney.ShowRewardedVideo(_level);
            if (result)
            {
                _repairPanel.SetActive(false);

                _gameData.HPCurrent += hp;
                _gameData.Save(true, (b) => taskEndSave.TrySetResult(b));
                resultSave = await taskEndSave.Task;
            }
            if (await _yMoney.AwaitCloseRewardedVideo())
            {
                if (!result) return;

                if (resultSave)
                    Message.Repair(hp);
                else
                    Message.BannerKey("ErrorSave", MessageType.Error, 4500);
            }
        }
    }
}
