using Newtonsoft.Json;

public class SizeOffsetSaveData
{
    [JsonProperty("Sz")]
    public float[] Size { get; }
    [JsonProperty("Fs")]
    public float[] Offset { get; }

    [JsonConstructor]
    public SizeOffsetSaveData(float[] size, float[] offset)
    {
        Size = size;
        Offset = offset;
    }
    public SizeOffsetSaveData(SizeOffsetSaveData data)
    {
        Size = data.Size;
        Offset = data.Offset;
    }
}
