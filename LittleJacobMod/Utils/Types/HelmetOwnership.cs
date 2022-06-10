using System.Collections.Generic;

namespace LittleJacobMod.Utils.Types;
using Newtonsoft.Json;

public class HelmetOwnership
{
    [JsonProperty("MpFemaleThermalVision", Required = Required.Always)]
    public bool MpFemaleThermalVision { get; set; }
    
    [JsonProperty("MpFemaleNightVision1", Required = Required.Always)]
    public bool MpFemaleNightVision1 { get; set; }
    
    [JsonProperty("MpFemaleNightVision2", Required = Required.Always)]
    public bool MpFemaleNightVision2 { get; set; }
    
    [JsonProperty("MpMaleThermalVision", Required = Required.Always)]
    public bool MpMaleThermalVision { get; set; }
    
    [JsonProperty("MpMaleNightVision1", Required = Required.Always)]
    public bool MpMaleNightVision1 { get; set; }

    [JsonProperty("MpMaleNightVision2", Required = Required.Always)] 
    public bool MpMaleNightVision2 { get; set; }
    
    [JsonProperty("OwnedColors")]
    public Dictionary<int, List<int>>? OwnedColors { get; set; }
}