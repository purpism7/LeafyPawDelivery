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
        private readonly string KeyCompleteDailyMission = string.Empty;
        private readonly string KeyCompleteAchievement = string.Empty;

        private readonly string KeyCompleteTutorial = string.Empty;

        public Connector()
        {
            var version = Game.Data.PlayerPrefsVersion;

            KeyOpenPlace = GetType().Name + "_OpenPlace_" + version;
            KeyAddAnimal = GetType().Name + "_AddAnimal_{0}_" + version;
            KeyAddObject = GetType().Name + "_AddObject_{0}_" + version;
            KeyCompleteDailyMission = GetType().Name + "_CompleteDailyMission_ " + version;
            KeyCompleteAchievement = GetType().Name + nameof(KeyCompleteAchievement) + "_" + version;

            KeyCompleteTutorial = GetType().Name + nameof(KeyCompleteTutorial) + "_" + version;
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
                return PlayerPrefs.GetInt(string.Format(KeyAddAnimal, GameUtils.ActivityPlaceId));
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
                return PlayerPrefs.GetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId));
            }
        }

        public void SetAddObject(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId), id);

            if(id > 0)
            {
                Game.Notification.Get?.Notify(Game.Notification.EType.AddObject);
            }
        }

        public void ResetAddObject()
        {
            if (AddObjectId <= 0)
                return;

            PlayerPrefs.SetInt(string.Format(KeyAddObject, GameUtils.ActivityPlaceId), 0);

            Game.Notification.Get?.Notify(Game.Notification.EType.AddObject);
        }
        #endregion

        #region Complete DailyMission
        public bool IsCompleteDailyMission
        {
            get
            {
                bool isComplete = false;
                System.Boolean.TryParse(PlayerPrefs.GetString(KeyCompleteDailyMission), out isComplete);

                return isComplete;
            }
        }

        public void SetCompleteDailyMission(bool isComplete)
        {
            PlayerPrefs.SetString(KeyCompleteDailyMission, isComplete.ToString());

            Game.Notification.Get?.Notify(Game.Notification.EType.CompleteDailyMission);
        }

        //public void ResetCompleteDailyMission()
        //{
        //    if (CompleteDailyMission <= 0)
        //        return;

        //    PlayerPrefs.SetInt(KeyCompleteDailyMission, 0);

        //    Game.Notification.Get?.Notify(Game.Notification.EType.CompleteDailyMission);
        //}뉴
        #endregion

        #region Complete Achievement
        public bool IsCompleteAchievement
        {
            get
            {
                bool isComplete = false;
                System.Boolean.TryParse(PlayerPrefs.GetString(KeyCompleteAchievement), out isComplete);

                return isComplete;
            }
        }

        public void SetCompleteAchievement(bool isComplete)
        {
            PlayerPrefs.SetString(KeyCompleteAchievement, isComplete.ToString());

            Game.Notification.Get?.Notify(Game.Notification.EType.CompleteAchievement);
        }

        //public void ResetCompleteAchievement()
        //{
        //    if (!IsCompletedAchievement)
        //        return;

        //    PlayerPrefs.SetString(KeyCompleteAchievement, false.ToString());

        //    Game.Notification.Get?.Notify(Game.Notification.EType.CompleteAchievement);
        //}
        #endregion

        #region Complete Tutorial
        public bool IsCompleteTutorial
        {
            get
            {
                bool isComplete = false;
                System.Boolean.TryParse(PlayerPrefs.GetString(KeyCompleteTutorial, false.ToString()), out isComplete);

                return isComplete;
            }
        }

        public void SetCompleteTutorial(bool isComplete)
        {
            PlayerPrefs.SetString(KeyCompleteTutorial, isComplete.ToString());
        }
        #endregion
    }
}

