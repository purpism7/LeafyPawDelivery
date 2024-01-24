using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "LeafyPawDelivery/ScriptableObject/Data")]
public class Boost : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public Game.Type.EBoost eBoost = Game.Type.EBoost.None;
        public Sprite iconSprite = null;
        public int timeSec = 0;
        public string adId = string.Empty;
        public string localKey = string.Empty;
    }

    [SerializeField]
    private Data[] datas = null;

    public Data[] Datas { get { return datas; } }
}

