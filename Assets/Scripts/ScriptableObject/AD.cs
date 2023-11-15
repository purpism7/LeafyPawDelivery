using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/ScriptableObject/AD")]
public class AD : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public string Key = string.Empty;
        public string Id = string.Empty;
    }

    public List<Data> AdDataList = new();
}
