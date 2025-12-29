using UnityEngine;

using Newtonsoft.Json;

[System.Serializable]
public class CropData : Data.Base
{
    [JsonProperty("img_name")] public string ImgName { get; private set; } = string.Empty;

    [JsonProperty("bloom_time")] public int BloomTime { get; private set; } = 0;
}
