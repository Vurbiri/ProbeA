using Newtonsoft.Json;
using System.Collections.Generic;

public class EnvironmentSaveData
{
    [JsonProperty("PF")]
    public List<PyramidsFieldSaveData> PyramidsField { get; }
    [JsonProperty("GF")]
    public List<SizeOffsetSaveData> GlassFloors { get; }
    [JsonProperty("Plt")]
    public List<PlatformSaveData> Platforms { get; }
    [JsonProperty("Clm")]
    public List<float[]> Columns { get; }
    [JsonProperty("SSD")]
    public List<SlabSaveData> Slabs { get; }

    [JsonConstructor]
    public EnvironmentSaveData(List<PyramidsFieldSaveData> pyramidsField, List<SizeOffsetSaveData> glassFloors, List<PlatformSaveData> platforms, List<float[]> columns, List<SlabSaveData> slabs)
    {
        PyramidsField = pyramidsField;
        GlassFloors = glassFloors;
        Platforms = platforms;
        Columns = columns;
        Slabs = slabs;
    }
}
