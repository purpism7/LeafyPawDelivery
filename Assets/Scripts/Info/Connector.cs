using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class Connector : Statics<Connector>
    {
        private readonly string KeyOpenPlace = string.Empty;

        private readonly string KeyPossibleBuyAnimal = string.Empty;
        private readonly string KeyPossibleBuyObject = string.Empty;

        private readonly string KeyAddAnimal = string.Empty;
        private readonly string KeyAddObject = string.Empty;
        private readonly string KeyAddStory = string.Empty;

        private readonly string KeyCompleteDailyMission = string.Empty;
        private readonly string KeyCompleteAchievement = string.Empty;

        private readonly string KeyCompleteTutorial = string.Empty;

        public Connector()
        {
            var version = Game.Data.PlayerPrefsVersion;

            KeyOpenPlace = GetType().Name + "_OpenPlace_" + version;

            KeyPossibleBuyAnimal = GetType().Name + Game.Notification.EType.PossibleBuyAnimal + "_{0}";
            KeyPossibleBuyObject = GetType().Name + Game.Notification.EType.PossibleBuyObject + "_{0}";

            KeyAddAnimal = GetType().Name + "_AddAnimal_{0}_" + version;
            KeyAddObject = GetType().Name + "_AddObject_{0}_" + version;
            KeyAddStory = GetType().Name + "_AddStory_{0}_" + version;

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

        #region Possible Buy Animal
        public int PossibleBuyAnimal
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(KeyPossibleBuyAnimal, GameUtils.ActivityPlaceId), 0);
            }
        }

        public bool CheckPossibleBuyAnimal
        {
            get
            {
                return PossibleBuyAnimal > 0;
            }
        }

        public void SetPossibleBuyAnimal()
        {
            if (PossibleBuyAnimal > 0)
                return;

            var openConditionCtr = AnimalOpenConditionContainer.Instance;
            if (openConditionCtr == null)
                return;

            if (!openConditionCtr.CheckPossibleBuy(out int id))
                return;

            SavePossibleBuyAnimal(id);
        }

        public void SetPossibleBuyAnimal(int id)
        {
            if (PossibleBuyAnimal > 0)
                return;

            var animalOpenConditionCtr = AnimalOpenConditionContainer.Instance;
            if (animalOpenConditionCtr == null)
                return;

            if (!animalOpenConditionCtr.CheckPossibleBuy(id))
                return;

            SavePossibleBuyAnimal(id);
        }

        public void ResetPossibleBuyAnimal()
        {
            if (PossibleBuyAnimal <= 0)
                return;

            SavePossibleBuyAnimal(0);
        }

        private void SavePossibleBuyAnimal(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyPossibleBuyAnimal, GameUtils.ActivityPlaceId), id);

            Game.Notification.Get?.Notify(Game.Notification.EType.PossibleBuyAnimal);
        }
        #endregion

        #region Possible Buy Object
        public int PossibleBuyObject
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(KeyPossibleBuyObject, GameUtils.ActivityPlaceId), 0);
            }
        }

        public bool CheckPossibleBuyObject
        {
            get
            {
                return PossibleBuyObject > 0;
            }
        }

        public void SetPossibleBuyObject()
        {
            if (PossibleBuyObject > 0)
                return;

            var openConditionCtr = ObjectOpenConditionContainer.Instance;
            if (openConditionCtr == null)
                return;

            if (!openConditionCtr.CheckPossibleBuy(out int id))
                return;

            SavePossibleBuyObject(id);
        }

        public void SetPossibleBuyObject(int id)
        {
            if (PossibleBuyObject > 0)
                return;

            var openConditionCtr = ObjectOpenConditionContainer.Instance;
            if (openConditionCtr == null)
                return;

            if (!openConditionCtr.CheckPossibleBuy(id))
                return;

            SavePossibleBuyObject(id);
        }

        public void ResetPossibleBuyObject()
        {
            if (PossibleBuyObject <= 0)
                return;

            SavePossibleBuyObject(0);
        }

        private void SavePossibleBuyObject(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyPossibleBuyObject, GameUtils.ActivityPlaceId), id);

            Game.Notification.Get?.Notify(Game.Notification.EType.PossibleBuyObject);
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

        #region Add Story
        public int AddStoryId
        {
            get
            {
                return PlayerPrefs.GetInt(string.Format(KeyAddStory, GameUtils.ActivityPlaceId));
            }
        }

        public void SetAddStory(int id)
        {
            PlayerPrefs.SetInt(string.Format(KeyAddStory, GameUtils.ActivityPlaceId), id);

            if (id > 0)
            {
                Game.Notification.Get?.Notify(Game.Notification.EType.AddStory);
            }
        }

        public void ResetAddStory()
        {
            if (AddStoryId <= 0)
                return;

            PlayerPrefs.SetInt(string.Format(KeyAddStory, GameUtils.ActivityPlaceId), 0);

            Game.Notification.Get?.Notify(Game.Notification.EType.AddStory);
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
        #endregion

        #region Complete Tutorial
        public bool IsCompleteTutorial
        {
            get
            {
                bool isComplete = false;
                System.Boolean.TryParse(PlayerPrefs.GetString(KeyCompleteTutorial), out isComplete);

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

