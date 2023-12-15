using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class Connector
    {
        private readonly string KEYOPENPLACEID = string.Empty;

        public Connector()
        {
            KEYOPENPLACEID = GetType().Name + "_OpenPlaceId";
        }

        public int OpenPlaceId
        {
            get
            {
                return PlayerPrefs.GetInt(KEYOPENPLACEID, 0);
            }
        }

        public void SaveOpenPlaceId(int id)
        {
            PlayerPrefs.SetInt(KEYOPENPLACEID, id);
        }

        public void ResetOpenPlaceId()
        {
            PlayerPrefs.SetInt(KEYOPENPLACEID, 0);
        }
    }
}

