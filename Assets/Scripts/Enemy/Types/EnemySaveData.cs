using Newtonsoft.Json;

public class EnemySaveData
{
    [JsonProperty("Tp")]
    public int Type { get; }
    [JsonProperty("Ps")]
    public float[] Position { get; }

    [JsonConstructor]
    public EnemySaveData(int type, float[] position)
    {
        Type = type;
        Position = position;
    }

    public EnemySaveData(EnemyData data)
    {
        Type = data.Type.ToInt();
        Position = data.Position.ToArray();
    }
}
