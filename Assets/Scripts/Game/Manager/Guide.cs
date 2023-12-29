using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Game.Manager
{
    public class Guide : Game.Common
    {
        private string KeyGuide { get { return "Key" + typeof(Guide).Name; } }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            AnimalManager.Event?.AddListener(OnChangedAnimal);
            ObjectManager.Event?.AddListener(OnChangedObject);
            PlaceEventController.Event?.AddListener(OnChangedPlaceEvnet);
        }

        private void Show(Queue<string> sentenceQueue)
        {
            Sequencer.EnqueueTask(
                () =>
                {
                    var guide = new GameSystem.PopupCreator<UI.Guide, UI.Guide.Data>()
                        .SetReInitialize(true)
                        .SetData(new UI.Guide.Data()
                        {
                            sentenceQueue = sentenceQueue,
                        })
                        .Create();

                    return guide;
                });
        }

        private void CheckOpenPlace()
        {
            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            if (!mainGameMgr.CheckIsAll)
                return;

            var sentence = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "guide_open_place", LocalizationSettings.SelectedLocale);

            var sentenceQueue = new Queue<string>();
            sentenceQueue.Clear();
            sentenceQueue.Enqueue(sentence);

            Show(sentenceQueue);
        }

        private void OnChangedAnimal(Game.Event.AnimalData animalData)
        {
            switch (animalData)
            {
                case Game.Event.AddAnimalData addAnimalData:
                    {
                        CheckOpenPlace();

                        break;
                    }


                case Game.Event.ArrangeAnimalData arrangeAnimalData:
                    {
                        Debug.Log("Guide = " + arrangeAnimalData.id);
                        if (Boolean.TryParse(PlayerPrefs.GetString(KeyGuide + "_Animal", false.ToString()), out bool already))
                        {
                            if (already)
                                return;
                        }

                        if (arrangeAnimalData.id == 1)
                        {
                            var sentenceQueue = new Queue<string>();
                            sentenceQueue.Clear();

                            var key = "guide_object_{0}";
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 1), LocalizationSettings.SelectedLocale));
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 2), LocalizationSettings.SelectedLocale));
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 3), LocalizationSettings.SelectedLocale));

                            Show(sentenceQueue);
                        }

                        break;
                    }
            }
        }

        private void OnChangedObject(Game.Event.ObjectData objectData)
        {
            switch (objectData)
            {
                case Event.AddObjectData addObjectData:
                    {
                        //Debug.Log("Guide = " + addObjectData.id);
                        //if (Boolean.TryParse(PlayerPrefs.GetString(KeyGuide + "_Object", false.ToString()), out bool already))
                        //{
                        //    if (already)
                        //        return;
                        //}

                        if (addObjectData.id == 1)
                        {
                            var sentenceQueue = new Queue<string>();
                            sentenceQueue.Clear();

                            var key = "guide_animal_{0}";
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 1), LocalizationSettings.SelectedLocale));
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 2), LocalizationSettings.SelectedLocale));
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", string.Format(key, 3), LocalizationSettings.SelectedLocale));

                            Show(sentenceQueue);

                            return;
                        }

                        CheckOpenPlace();

                        break;
                    }
            }
        }

        private void OnChangedPlaceEvnet(Game.PlaceEvent.BaseData baseData)
        {
            Debug.Log("OnChangedPlaceEvnet");

            switch (baseData)
            {
                case Game.PlaceEvent.DropItemData dropItemData:
                    {
                        if (dropItemData == null)
                            return;

                        if (Boolean.TryParse(PlayerPrefs.GetString(KeyGuide + "FirstDropLetter", false.ToString()), out bool already))
                        {
                            if (already)
                                return;
                        }
                        
                        if (dropItemData.currCnt == 1)
                        {
                            var sentenceQueue = new Queue<string>();
                            sentenceQueue.Clear();

                            var key = "guide_drop_letter";
                            sentenceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", key, LocalizationSettings.SelectedLocale));

                            Show(sentenceQueue);
                        }

                        break;
                    }
            }
        }
    }
}

