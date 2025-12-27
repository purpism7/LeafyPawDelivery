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
        public DateTime? growthEndTime = null;

        public PlotInfo()
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = GameUtils.GenerateUniqueID("P");
            }
        }
    }
}

