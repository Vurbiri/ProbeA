using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    public const string key = "gmd";

    public bool IsDifficulty => _dataSave.LevelCurrent > levelUniqueMax;
    public bool IsGameCompleted => _dataSave.LevelCurrent >= levelMax;
    public float HPCurrent { get => _dataSave.HpCurrent; set { _dataSave.HpCurrent = value; EventChangeCurrentHP?.Invoke(_dataSave.HpCurrent); } }
    public float HPFull => _playerStates.HP;
    private float HPStartLevel => _dataSave.HPStartLevel;
    public float HPRelatively => HPCurrent / HPFull;
    public float HPRelativelyLevel => 1f - (HPStartLevel - HPCurrent) / HPFull;
    public int LevelLoad => IsDifficulty ? _dataSave.LevelCurrent - levelUniqueMax : _dataSave.LevelCurrent;
    public int LevelCurrent => _dataSave.LevelCurrent;
    public int Score { get => _dataSave.Score; set => _dataSave.Score = value; }
    public int MaxArtefacts { get => _dataSave.MaxArtefacts; set => _dataSave.MaxArtefacts = value; }
    public List<int> Artefacts => _dataSave.Artefacts;
    public int TimeLevelSeconds { get => _dataSave.TimeLevelSeconds; set => _dataSave.TimeLevelSeconds = value; }
    public GameModeStart ModeStart => _dataSave.ModeStart;

    public int ExpReward { get; private set; }
    public int ScoreReward { get; private set; }

    private GameDataSave _dataSave;
    private PlayerStates _playerStates;

    private const int levelUniqueMax = 33;
    private const int levelMax = levelUniqueMax * 2;

    public event Action<float> EventChangeCurrentHP;
    public event Action<int> EventChangeArtefactsCount;
    
    public bool Initialize(bool isLoad)
    {
        bool result = false;
        _playerStates = PlayerStates.InstanceF;

        if (isLoad)
            result = Load();
        if (!result)
            CreateNewGameData();

        return result;

    }

    private bool Load()
    {
        var (result, value) = Storage.Load<GameDataSave>(key);
        if (result)
            _dataSave = value;
        return result;
    }

    public void Save(bool isSaveHard, Action<bool> callback = null)
    {
        Storage.Save(key, _dataSave, isSaveHard, callback);
    }
    public void ArtefactOff(int id)
    {
        Artefacts.Remove(id);
        EventChangeArtefactsCount?.Invoke(Artefacts.Count);
    }

    public void CreateArtefactsData(int count)
    {
        _dataSave.CreateArtefactsData(count);
    }

    public void CreateNewGameData()
    {
        float hp = HPFull;

        if (_dataSave == null)
            _dataSave = new(hp);
        else
            _dataSave.CreateNewGameData(hp);

        ExpReward = 0;
        ScoreReward = 0;
    }

    public void NextLevel(float hpForCompletion, float addExp, int addScore)
    {
        _dataSave.HpCurrent += _playerStates.HP * hpForCompletion;

        ExpReward = (int)Mathf.Round(addExp);
        ScoreReward = addScore;

        _dataSave.NextLevel();
    }

    public void StartLevel() => _dataSave.StartLevel();


#if UNITY_EDITOR
        public void SetLevelCurrent(int level) => _dataSave.LevelCurrent = level;
#endif

    #region Nested Classe
    private class GameDataSave
    {
        [JsonProperty("hpc")]
        public float HpCurrent { get; set; }
        [JsonProperty("llc")]
        public int LevelCurrent { get; set; } 
        [JsonProperty("scr")]
        public int Score { get; set; }
        [JsonProperty("mat")]
        public int MaxArtefacts { get; set; }
        [JsonProperty("art")]
        public List<int> Artefacts { get; set; }
        [JsonProperty("tls")]
        public int TimeLevelSeconds { get; set; }
        [JsonProperty("hps")]
        public float HPStartLevel { get; set; }
        [JsonProperty("gms")]
        public GameModeStart ModeStart { get; set; }


        [JsonConstructor]
        public GameDataSave(float hpCurrent, int levelCurrent, int score, int maxArtefacts, List<int> artefacts, int timeLevelSeconds, float hpStartLevel, GameModeStart modeStart)
        {
            HpCurrent = hpCurrent;
            LevelCurrent = levelCurrent;
            Score = score;
            MaxArtefacts = maxArtefacts;
            Artefacts = new(artefacts);
            Artefacts.Sort();
            TimeLevelSeconds = timeLevelSeconds;
            HPStartLevel = hpStartLevel;
            ModeStart = modeStart;
        }

        public GameDataSave(float hpCurrent)
        {
            HpCurrent = hpCurrent;
            LevelCurrent = 1;
            Score = 0;
            MaxArtefacts = 0;
            Artefacts = new();
            TimeLevelSeconds = 0;
            HPStartLevel = hpCurrent;
            ModeStart = GameModeStart.GameNew;
        }

        public void CreateNewGameData(float hpCurrent)
        {
            HpCurrent = hpCurrent;
            LevelCurrent = 1;
            Score = 0;
            MaxArtefacts = 0;
            Artefacts.Clear();
            TimeLevelSeconds = 0;
            HPStartLevel = hpCurrent;
            ModeStart = GameModeStart.GameNew;
        }

        public void NextLevel()
        {
            LevelCurrent++;
            Artefacts.Clear();
            MaxArtefacts = 0;
            TimeLevelSeconds = 0;
            ModeStart = GameModeStart.LevelNew;
        }

        public void StartLevel()
        {
            if (ModeStart == GameModeStart.LevelContinue)
                return;

            HPStartLevel = HpCurrent;
            ModeStart = GameModeStart.LevelContinue;
        }

        public void CreateArtefactsData(int count)
        {
            Artefacts.Clear();
            for (int i = 0; i < count; i++)
                Artefacts.Add(i);
            Artefacts.Sort();
            MaxArtefacts = count;
        }
    }

    #endregion
}

