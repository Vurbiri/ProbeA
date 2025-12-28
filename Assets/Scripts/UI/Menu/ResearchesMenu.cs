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
        float oldHP = _states.HP;
        float oldHPRelatively = _gameData.HPRelatively;

        _states.Apply();

        float nowHP = _states.HP;
        if (oldHP != nowHP)
        {
            _gameData.HPCurrent += (nowHP - oldHP) * oldHPRelatively;
            _gameData.Save(false);
        }

        _states.Save(true, (b) => Message.Saving("GoodSaveStates", b));
    }

    public void OnCancel()
    {
        _states.Cancel();
    }
}
