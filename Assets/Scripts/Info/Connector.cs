using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class Connector : Statics<Connector>
    {
        private readonly string KeyOpenPlace = string.Empty;
        private readonly string KeyAddAnimal = string.Empty;
        private readonly string KeyAddObject = string.Empty;

        public Connector()
        {
            KeyOpenPlace = GetType().Name + "_OpenPlace";
            KeyAddAnimal = GetType().Name + "_AddAnimal_{0}";
            KeyAddObject = GetType().Name + "_AddObject_{0}";
        }

        #region Open Place
        public int OpenPlaceId
        {
            get
            {
                return PlayerPrefs.GetInt(KeyOpenPlace, 0);
            }
        }

        public void SetOpenPlace(int id)
        {
            PlayerPrefs.SetInt(KeyOpenPlace, id);
        }

        public void ResetOpenPlace()
        {
            PlayerPrefs.SetInt(KeyOpenPlace, 0);

            Game.Notification.Get?.Notify(Game.Notification.EType.OpenPlace);
        }
        #endregion

        #region Add Animal
        public int AddAnimalId
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(KeyAddAnimal, GameUtils.ActivityPlaceId), 0);
            }
        }

        public void SetAddAnimal(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyAddAnimal, GameUtils.ActivityPlaceId), id);
        }

        public void ResetAddAnimal()
        {
            if (AddAnimalId <= 0)
                return;

            PlayerPrefs.SetInt(string.Format(KeyAddAnimal, GameUtils.ActivityPlaceId), 0);

            Game.Notification.Get?.Notify(Game.Notification.EType.AddAnimal);
        }
        #endregion

        #region Add Object
        public int AddObjectId
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId), 0);
            }
        }

        public void SetAddObject(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId), id);
        }

        public void ResetAddObject()
        {
            if (AddObjectId <= 0)
                return;

            PlayerPrefs.SetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId), 0);

            Game.Notification.Get?.Notify(Game.Notification.EType.AddObject);
        }
        #endregion
    }
}

