using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class Connector
    {
        private readonly string KeyOpenPlaceId = string.Empty;

        public Connector()
        {
            KeyOpenPlaceId = GetType().Name + "_OpenPlaceId";
        }

        public int OpenPlaceId
        {
            get
            {
                return PlayerPrefs.GetInt(KeyOpenPlaceId, 0);
            }
        }

        public void SaveOpenPlaceId(int id)
        {
            PlayerPrefs.SetInt(KeyOpenPlaceId, id);
        }

        public void ResetOpenPlaceId()
        {
            PlayerPrefs.SetInt(KeyOpenPlaceId, 0);
        }
    }
}

