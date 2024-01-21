using Newtonsoft.Json;
using System;
using UnityEngine;

public class PlayerStates : Singleton<PlayerStates>
{
    [Space]
    [SerializeField] private State _HP;
    [Space]
    [SerializeField] private State _speed;
    [SerializeField] private float _speedDebuff = 0.7f;
    [Space]
    [SerializeField] private State _stealth;
    [Space]
    [SerializeField] private int _expBase = 100;
    [SerializeField] private int _expIncrement = 100;

    private State this[StateType type] => type switch { StateType.HP => _HP, StateType.Speed => _speed, StateType.Stealth => _stealth, _ => null };

    public const string key = "pst";


    private int _pointsSpent = 0;
    private int _pointsTempSpent = 0;
    private int _level = 0;
    private float _exp = 0;
    private int _expNextLevel = 0;
    private readonly int levelMax = State.LevelMax * 3;

    public float HP => _HP.Value;
    public float Speed => _speed.Value;
    public float Debuff => _speedDebuff;
    public float Stealth => _stealth.Value;

    public bool IsCap => _level >= levelMax;
    public bool IsResetPossible => _pointsSpent > 0;
    private bool IsLevelUp => _HP.IsLevelUp || _speed.IsLevelUp || _stealth.IsLevelUp;

    public int Points => _level - _pointsSpent - _pointsTempSpent;
    public string Level => _level.ToString();
    public string ExpToLevel => (_expNextLevel - ((int)Mathf.Round(_exp))).ToString();

    public event Action<int> EventChangePoints;
    public event Action<bool> EventChangePointsSpent;

    public bool Initialize(bool isLoad)
    {
        bool result = false;

        if (isLoad)
            result = Load();

        if (!result)
            ExpForNextLevel();

        return result;
    }

    public void Save(bool isSaveHard = true, Action<bool> callback = null)
    {
        PlayerStatesSave save = new(_exp, _HP.Level, _speed.Level, _stealth.Level);
        Storage.Save(key, save, isSaveHard, callback);
    }
    private bool Load()
    {
        var (result, value) = Storage.Load<PlayerStatesSave>(key);
        if (result)
        {
            _exp = value.exp;

            _pointsSpent = 0;
            int lvl;
            foreach (var type in Enum<StateType>.GetValues())
            {
                lvl = value.lvlStates[type.ToInt()];
                
                _pointsSpent += lvl;
                this[type].Initialize(lvl);
            }

            _level = 0;
            ExpForNextLevel();
            CalcLevel();
        }

        return result;
    }

    public bool IsStateCap(StateType type) => this[type].IsCap;
    public bool IsStateLevelUp(StateType type) => this[type].IsLevelUp;
    public int StateLevel(StateType type) => this[type].LevelFull;
    public void SubscribeEventChangeLevel(StateType type, Action<int> action) => this[type].EventChangeLevel += action;
    public void UnsubscribeEventChangeLevel(StateType type, Action<int> action) => this[type].EventChangeLevel -= action;

    public void AddExp(float exp)
    {
        if (IsCap) return;

        _exp += exp;
        CalcLevel();
    }

    private void CalcLevel()
    {
        while (_exp >= _expNextLevel)
        {
            _level = Mathf.Clamp(++_level, 0, levelMax);
            ExpForNextLevel();

            if (IsCap) return;
        }
    }

    public void LevelUp(StateType type)
    {
        if (Points <= 0) return;

        if (this[type].LevelUp())
        {
            _pointsTempSpent++;
            EventChangePoints?.Invoke(Points);
        }
    }

    public void LevelDown(StateType type)
    {
        if (_pointsTempSpent <= 0) return;

        if (this[type].LevelDown())
        {
            _pointsTempSpent--;
            EventChangePoints?.Invoke(Points);
        }
    }

    public void Cancel()
    {
        if (!IsLevelUp) return;
        
        foreach(var type in Enum<StateType>.GetValues())
            _pointsTempSpent -= this[type].Cancel();
        EventChangePoints?.Invoke(Points);
    }
    public void Apply()
    {
        if (!IsLevelUp) return;

        foreach (var type in Enum<StateType>.GetValues())
            Apply(this[type]);

        EventChangePoints?.Invoke(Points);
        EventChangePointsSpent?.Invoke(IsResetPossible);

        void Apply(State state)
        {
            int points = state.Apply();
            _pointsTempSpent -= points;
            _pointsSpent += points;
        }
    }
    public void ResetStates()
    {
        foreach (var type in Enum<StateType>.GetValues())
            Reset(this[type]);

        EventChangePoints?.Invoke(Points);
        EventChangePointsSpent?.Invoke(false);

        void Reset(State state)
        {
            _pointsTempSpent -= state.Cancel(); ;
            _pointsSpent -= state.Reset();
        }
    }

    private void ExpForNextLevel()
    {
        _expNextLevel = ExpForLevel(_level + 1);

        int ExpForLevel(int level) => (2 * _expBase + _expIncrement * (level - 1)) * level / 2;
    }

    #region Nested Classe
    [Serializable]
    private class State
    {
        [SerializeField] private float _valueStart;
        [SerializeField] private float _perLvl;
        [SerializeField] private int _level = 0;

        private int _levelTemp = 0;
        private const int levelMax = 50;

        public float Value => _valueStart + _perLvl * _level;

        public static int LevelMax => levelMax;
        public int LevelFull => _level + _levelTemp;
        public int Level => _level;
        public bool IsLevelUp => _levelTemp > 0;
        
        public bool IsCap => LevelFull >= levelMax;

        public event Action<int> EventChangeLevel;

        public void Initialize(int level) => _level = level;

        public bool LevelUp()
        {
            if (LevelFull >= levelMax) return false;

            _levelTemp++;
            EventChangeLevel?.Invoke(LevelFull);
            return true;
        }

        public bool LevelDown()
        {
            if (_levelTemp <= 0) return false;

            _levelTemp--;
            EventChangeLevel?.Invoke(LevelFull);
            return true;
        }

        public int Apply()
        {
            _level = Mathf.Clamp(_level + _levelTemp, 0, levelMax);
            return Cancel();
        }

        public int Cancel()
        {
            if (!IsLevelUp) return 0;
            
            int levelTemp = _levelTemp;
            _levelTemp = 0;
            EventChangeLevel?.Invoke(LevelFull);
            return levelTemp;
        }

        public int Reset()
        {
            int level = _level;
            _level = 0;
            EventChangeLevel?.Invoke(0);
            return level;
        }
    }

    private class PlayerStatesSave
    {
        [JsonProperty("exp")]
        public float exp;
        [JsonProperty("lss")]
        public int[] lvlStates;

        [JsonConstructor]
        public PlayerStatesSave(float exp, int[] lvlStates)
        {
            this.exp = exp;

            this.lvlStates = new int[lvlStates.Length];
            lvlStates.CopyTo(this.lvlStates, 0);
        }

        public PlayerStatesSave(float exp, int lvlHp, int lvlSpeed, int lvlStealth)
        {
            this.exp = exp;

            lvlStates = new int[]{ lvlHp, lvlSpeed, lvlStealth};
        }
    }
    #endregion
    }
