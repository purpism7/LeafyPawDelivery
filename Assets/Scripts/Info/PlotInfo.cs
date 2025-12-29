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
        
        // 1. 개화 여부 판별 (현재 시간이 종료 시간보다 지났는가?)
        // cropID가 0이 아니고, 시간이 설정되었으며, 현재 시간이 종료 시간을 넘었을 때 true
        public bool IsBloomed => cropID != 0 && growthEndTime.HasValue && DateTime.Now >= growthEndTime.Value;

        // 2. 현재 성장 중인지 여부
        public bool IsGrowing => cropID != 0 && growthEndTime.HasValue && DateTime.Now < growthEndTime.Value;

        // 3. 아무것도 심기지 않았는지 여부
        public bool IsEmpty => cropID == 0 || !growthEndTime.HasValue;
    }
}

