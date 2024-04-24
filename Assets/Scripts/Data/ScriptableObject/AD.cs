using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leafy Parcels/ScriptableObject/Data/AD")]
public class AD : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public Game.Type.ECategory eCategory = Game.Type.ECategory.None;
        [SerializeField]
        private string ios = string.Empty;
        [SerializeField]
        private string and = string.Empty;
        [Header("Iron Source")]
        [SerializeField]
        private string placement = string.Empty;

        public float coolTimeSec = 0;

        public string adId
        {
            get
            {
                //return placement;
#if UNITY_IOS
                return ios;
#elif UNITY_ANDROID
                return and;
#endif
            }
        }
    }

    [SerializeField]
    private Data[] datas = null;

    public Data[] Datas { get { return datas; } }

    public Data GetData(Game.Type.ECategory eCategory)
    {
        if (datas == null)
            return null;

        foreach(var data in datas)
        {
            if (data == null)
                continue;

            if(data.eCategory == eCategory)
            {
                return data;
            }
        }

        return null;
    }
}
