using Cysharp.Threading.Tasks;
using System;

public class ResearchesMenu : MenuNavigation
{
    private PlayerStates _states;
    private GameData _gameData;

    protected override void Awake()
    {
        base.Awake();

        _states = PlayerStates.InstanceF;
        _gameData = GameData.InstanceF;
    }


    public void OnOk()
    {
        SetHP(_states.Apply);
        _states.Save(true, (b) => Message.Saving("GoodSaveStates", b));
    }

    public void OnCancel()
    {
        _states.Cancel();
    }

    public void OnResetStates()
    {
        OnResetStatesAsync().Forget();

        async UniTaskVoid OnResetStatesAsync()
        {
            UniTaskCompletionSource<bool> taskEndSave = new();
            bool result = false;

            if (await YMoney.Instance.ShowRewardedVideo())
            {
                SetHP(_states.ResetStates);
                _states.Save(true, (b) => taskEndSave.TrySetResult(b));
                result = await taskEndSave.Task;
            }
            if (await YMoney.Instance.AwaitCloseRewardedVideo())
                Message.Saving("GoodSaveStates", result);
        }
    }

    private void SetHP(Action action)
    {
        float oldHP = _states.HP;
        float oldHPRelatively = _gameData.HPRelatively;

        action();

        float nowHP = _states.HP;
        if (oldHP != nowHP)
        {
            _gameData.HPCurrent += (nowHP - oldHP) * oldHPRelatively;
            _gameData.Save(false);
        }
    }
}
