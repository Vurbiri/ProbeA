using Newtonsoft.Json;

public class ArtefactSaveData 
{
    [JsonProperty("Id")]
    public int Id { get; }
    [JsonProperty("Ps")]
    public float[] Position { get; }

    [JsonConstructor]
    public ArtefactSaveData(int id, float[] position)
    {
        Id = id;
        Position = position;
    }
}
