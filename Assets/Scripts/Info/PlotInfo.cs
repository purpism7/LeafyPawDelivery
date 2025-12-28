using System;
using Unity.Collections;
using UnityEngine;

namespace Info
{
    [Serializable]
    public class PlotInfo : BaseInfo
    {
        public string objectUniqueID = string.Empty;
        // [ReadOnly]
        // public string uniqueID = string.Empty;
        public DateTime? growthEndTime = null;
        public int cropID = 0;


        // public PlotInfo()
        // {
        //     if (string.IsNullOrEmpty(uniqueID))
        //     {
        //         uniqueID = GameUtils.GenerateUniqueID("P");
        //     }
        // }
    }
}

