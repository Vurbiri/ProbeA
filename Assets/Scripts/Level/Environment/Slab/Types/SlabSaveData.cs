using Newtonsoft.Json;
using System.Collections.Generic;

public class SlabSaveData 
{
    [JsonProperty("Sz")]
    public float[] Size { get; }
    [JsonProperty("Pts")]
    public List<float[]> Points { get; }
    [JsonProperty("Sd")]
    public float Speed { get; }
    [JsonProperty("Ps")]
    public float Pause { get; }
    [JsonProperty("IRt")]
    public int IsRotation { get; }
    [JsonProperty("IRn")]
    public int IsRandom { get; }

    [JsonConstructor]
    public SlabSaveData(float[] size, List<float[]> points, float speed, float pause, int isRotation, int isRandom)
    {
        Size = size;
        Points = points;
        Speed = speed;
        Pause = pause;
        IsRotation = isRotation;
        IsRandom = isRandom;
    }

    public SlabSaveData(SlabData data)
    {
        Size = data.Size.ToArray();
        Speed = data.Speed;
        Pause = data.Pause;
        IsRotation = data.IsRotation ? 1 : 0;
        IsRandom = data.IsRandom ? 1 : 0;
        Points = new(data.Points.Count);
        foreach (var point in data.Points)
            Points.Add(point.ToArray());
    }
}
