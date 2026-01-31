using System;
using Unity.Collections;
using UnityEngine;

using Newtonsoft.Json;

namespace Info
{
    [Serializable]
    public class PlotInfo : BaseInfo
    {
        [ReadOnly]
        [JsonProperty]
        public string uniqueID = string.Empty;

        [JsonProperty]
        public string objectUniqueID = string.Empty;

        [JsonProperty]
        public DateTime? growthEndTime = null;

        [JsonProperty]
        public int cropID = 0;

        public PlotInfo(string objectUID)
        {
            objectUniqueID = objectUID;
            uniqueID = GameUtils.GenerateUniqueID("P");
        }
    }
}

