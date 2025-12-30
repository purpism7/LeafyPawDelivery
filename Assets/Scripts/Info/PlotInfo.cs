using System;
using Unity.Collections;
using UnityEngine;

namespace Info
{
    [Serializable]
    public class PlotInfo : BaseInfo
    {
        [ReadOnly]
        public string uniqueID = string.Empty;
        public string objectUniqueID = string.Empty;
        public DateTime? growthEndTime = null;
        public int cropID = 0;

        public PlotInfo()
        {
            uniqueID = GameUtils.GenerateUniqueID("P");
        }
    }
}

