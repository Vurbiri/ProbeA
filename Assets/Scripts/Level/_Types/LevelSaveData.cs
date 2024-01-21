using Newtonsoft.Json;
using System.Collections.Generic;

public class LevelSaveData
{
    [JsonProperty("Env")]
    public EnvironmentSaveData Environment { get; }
    [JsonProperty("Art")]
    public List<ArtefactSaveData> Artefacts { get; }
    [JsonProperty("Enm")]
    public List<EnemySaveData> Enemies { get; }
    [JsonProperty("Prb")]
    public float[] Probe { get; }
    [JsonProperty("LD")]
    public float LevelDifficulty { get; }

    [JsonConstructor]
    public LevelSaveData(EnvironmentSaveData environment, List<ArtefactSaveData> artefacts, List<EnemySaveData> enemies, float[] probe, float levelDifficulty)
    {
        Environment = environment;
        Artefacts = artefacts;
        Enemies = enemies;
        Probe = probe;
        LevelDifficulty = levelDifficulty;
    }
}
