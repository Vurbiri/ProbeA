using System;
using System.Collections;
using UnityEngine;

public class Game : Singleton<Game>
{
    [SerializeField] private Level _level;
    [Space]
    [SerializeField] private float _timeBeforeStart = 1f;
    [SerializeField] private float _timeBeforeSave = 0.5f;
    [Space]
    [SerializeField] private int _timeAutosave = 30;
    [Space]
    [SerializeField] private float _baseExp = 14f;
    [SerializeField] private float _baseScore = 500f;
    [SerializeField] private float _gameCompletedBonus = 500f;

    private Coroutine _coroutineSave;
    private Coroutine _coroutineTimer;
    private WaitForSecondsRealtime _beforeSave;

    private GameData _gameData;
    private PlayerStates _states;
    private PlayerController _controller;

    public event Action<bool> EventPause;
    public event Action EventStartGame;
    public event Action EventLevelCompleted;
    public event Action<int> EventGameCompleted;
    public event Action<int> EventGameOver;
    public event Action<string> EventTimer;

#if UNITY_EDITOR
    [Space]
    [SerializeField, Range(0, 33)] private int _levelId = 0;
#endif

    protected override void Awake()
    {
        _isNotDestroying = false;
        base.Awake();
       
        _gameData = GameData.InstanceF;
        _states = PlayerStates.InstanceF;
        _controller = PlayerController.InstanceF;

        _gameData.EventChangeArtefactsCount += OnChangeArtefactsCount;
        _gameData.EventChangeCurrentHP += OnChangeCurrentHP;
        
        _controller.EventOpenMenu += Pause;
        _controller.EventCloseMenu += Continue;

#if UNITY_EDITOR
        if (_levelId > _gameData.LevelCurrent)
            _gameData.SetLevelCurrent(_levelId);
#endif
        _level.Create(_gameData.LevelLoad);
    }


    private IEnumerator Start()
    {
        _controller.DisableMaps();

        _beforeSave = new(_timeBeforeSave);
        WaitForSecondsRealtime halfPause = new(_timeBeforeStart / 2f);
        EventTimer?.Invoke(_gameData.TimeLevelSeconds.ToTimeString());

        yield return halfPause;
        _gameData.StartLevel();
        EventStartGame?.Invoke();

        yield return new WaitForSecondsRealtime(0.6f);
        _controller.EnableGameplayMap();
        _level.Play();
        TimerStart();
    }

    public void Pause()
    {
        _controller.DisableMaps();
        Time.timeScale = 0.000001f;

        EventPause?.Invoke(true);
        _controller.EnableUIMap();
    }

    public void Continue()
    {
        _controller.DisableMaps();
        Time.timeScale = 1f;
        
        EventPause?.Invoke(false);
        _controller.EnableGameplayMap();
    }

    public void Save()
    {
        StopSavingDeferred();
        _gameData.Save(true, (b) => Message.Saving("GoodSave", b));
    }

    private void SaveDeferred()
    {
        StopSavingDeferred();
        _coroutineSave = StartCoroutine(Saving());

        IEnumerator Saving()
        {
            yield return _beforeSave;
            _gameData.Save(true, null);
            _coroutineSave = null;
        }
    }

    private void StopSavingDeferred()
    {
        if (_coroutineSave != null)
        {
            StopCoroutine(_coroutineSave);
            _coroutineSave = null;
            _beforeSave = new(_timeBeforeSave);
        }
    }

    private void OnChangeArtefactsCount(int count)
    {
        if (count <= 0)
        {
            if (_gameData.IsGameCompleted)
                GameCompleted();
            else
                LevelCompleted();
            return;
        }

        SaveDeferred();
    }

    private void OnChangeCurrentHP(float currentHP)
    {
        if(currentHP <= 0)
        {
            GameOver();
            return;
        }

        SaveDeferred();
    }

    private void LevelCompleted()
    {
        GameStop();
        var (addExp, addScore) = AddExpAndScore(_gameData.MaxArtefacts);
        _gameData.NextLevel(addExp, addScore);
        EventLevelCompleted?.Invoke();
        GameSave();
    }

    public void LevelCompletedCheat()
    {
        GameStop();
        _gameData.NextLevel(0, 0);
        EventLevelCompleted?.Invoke();
        GameSave();
        Continue();
    }

    private void GameCompleted()
    {
        GameStop();
        AddExpAndScore(_gameData.MaxArtefacts, _gameCompletedBonus);
        EventGameCompleted?.Invoke(_gameData.Score);
        _gameData.CreateNewGameData();
        GameSave();
    }

    private void GameOver()
    {
        GameStop();
        AddExpAndScore(_gameData.MaxArtefacts - _gameData.Artefacts.Count);
        EventGameOver?.Invoke(_gameData.Score);
        _gameData.CreateNewGameData();
        GameSave();
    }

    private (float addExp, int addScore) AddExpAndScore(int countArtefacts, float bonus = 0f)
    {
        float points = _gameData.LevelCurrent * _gameData.HPRelativelyLevel * countArtefacts + bonus;
        float levelsDifficulty = _level.Difficulty * (_gameData.IsDifficulty ? 2f : 1f);

        float addExp = points * _baseExp;
        int addScore = (int)Mathf.Round(points * levelsDifficulty * _baseScore / _gameData.TimeLevelSeconds);

        _states.AddExp(addExp);
        _gameData.Score += addScore;

        return (addExp, addScore);
    }

    private void GameStop()
    {
        UnsubscribeEvents();
        _controller.EnableUIMap();
        _level.Stop();
        TimerStop();
    }

    public void GameSave()
    {
        StopSavingDeferred();
        _states.Save(false);
        _gameData.Save(true);
    }

    private void TimerStart()
    {
        TimerStop();
        _coroutineTimer = StartCoroutine(TimerStartCoroutine());

        IEnumerator TimerStartCoroutine()
        {
            WaitForSeconds pause = new(1f);
            int time;

            while (true)
            {
                yield return pause;

                time = ++_gameData.TimeLevelSeconds;
                EventTimer?.Invoke(time.ToTimeString());

                if (time % _timeAutosave == 0)
                    SaveDeferred();
            }
        }
    }

    private void TimerStop()
    {
        if (_coroutineTimer != null)
        {
            StopCoroutine(_coroutineTimer);
            _coroutineTimer = null;
        }
    }

    private void UnsubscribeEvents()
    {
        if (GameData.Instance != null)
        {
            _gameData.EventChangeArtefactsCount -= OnChangeArtefactsCount;
            _gameData.EventChangeCurrentHP -= OnChangeCurrentHP;
        }
        if (PlayerController.Instance != null)
        {
            _controller.EventOpenMenu -= Pause;
            _controller.EventCloseMenu -= Continue;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnsubscribeEvents();
    }
}
