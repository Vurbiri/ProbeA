#if UNITY_EDITOR
using NaughtyAttributes;
#endif

using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private EnvironmentCreate _environment;
    [SerializeField] private Artefacts _artefacts;
    [SerializeField] private EnemiesOvermind _enemies;
    [SerializeField] private ProbeSpawn _probe;
    [Space]
    [SerializeField] private float _levelDifficulty = 1;
    [Space]
    [SerializeField] private Color[] _colors;
    [SerializeField, Range(0,5)] private int _colorID;

    public float Difficulty => _levelDifficulty;

    private string Path => folder + fileName;

    private const string fileName = "level_";
    private const string folder = "Levels/";

#if UNITY_EDITOR
    [Space]
    [SerializeField, Range(1, 33)] private int _fileId = 0;
    private string FullFileName => Path + _fileId;
#endif

    public void Create(int level)
    {
        _colorID = Random.Range(0, _colors.Length);
        var (result, save) = StorageResources.LoadFromJson<LevelSaveData>(Path + level);
        if (result)
        {
            _environment.Create(save.Environment, _colors[_colorID]);
            _artefacts.Create(save.Artefacts);
            _enemies.Create(save.Enemies);
            _probe.Create(save.Probe);
            _levelDifficulty = save.LevelDifficulty;
        }
    }

    public void Play()
    {
        _enemies.Thinking();
    }

    public void Stop()
    {
        _enemies.Stop();
        _environment.Stop();
    }

#if UNITY_EDITOR

    [Button]
    public void LoadTest()
    {

        Delete();

        var (result, save) = StorageResources.LoadFromJson<LevelSaveData>(FullFileName);
        if (result)
        {
            _environment.CreateTest(save.Environment, _colors[_colorID]);
            _artefacts.CreateTest(save.Artefacts);
            _enemies.CreateTest(save.Enemies);
            _probe.Create(save.Probe);
            _levelDifficulty = save.LevelDifficulty;
        }
    }

    [Button]
    public void Save()
    {
        LevelSaveData save = new(_environment.Get(), _artefacts.Get() , _enemies.Get(), _probe.Get(), _levelDifficulty);
        StorageResources.Save(FullFileName + ".json", save);
    }
    [Button]
    public void Delete()
    {
        _environment.Delete();
        _artefacts.Delete();
        _enemies.Delete();
        _probe.Delete();
    }
    [Button]
    public void LoadData()
    {
        var (result, save) = StorageResources.LoadFromJson<LevelSaveData>(FullFileName);
        if (result)
        {
            _environment.LoadData(save.Environment);
            _artefacts.LoadData(save.Artefacts);
            _enemies.LoadData(save.Enemies);
            _probe.LoadData(save.Probe);
            _levelDifficulty = save.LevelDifficulty;
        }
    }
    [Button]
    public void ClearData()
    {
        _environment.ClearData();
        _artefacts.ClearData();
        _enemies.ClearData();
        _levelDifficulty = 1;
    }

#endif
}
