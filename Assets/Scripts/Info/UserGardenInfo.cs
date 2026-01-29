using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [Serializable]
    public partial class User
    {
        [JsonProperty]
        public string[] PlotUniqueIDs = null;

        [JsonProperty]
        public int GardenPlotCount = 0; 
    }
}

