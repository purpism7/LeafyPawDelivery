using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Game.Manager
{
    public class Guide : Base<Guide.Data>
    {
        public class Data : BaseData
        {

        }

        protected override void Initialize()
        {
            AnimalManager.Event?.AddListener(OnChangedAnimalInfo);
        }

        public override IEnumerator CoInitialize(Data data)
        {
            yield return null;
        }

        public void Show()
        {
            Sequencer.EnqueueTask(
                () =>
                {
                    var senteceQueue = new Queue<string>();
                    senteceQueue.Clear();
                    senteceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", "guide_" + "1_1", LocalizationSettings.SelectedLocale));
                    senteceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", "guide_" + "1_2", LocalizationSettings.SelectedLocale));
                    senteceQueue.Enqueue(LocalizationSettings.StringDatabase.GetLocalizedString("UI", "guide_" + "1_3", LocalizationSettings.SelectedLocale));

                    var guide = new GameSystem.PopupCreator<UI.Guide, UI.Guide.Data>()
                        .SetReInitialize(true)
                        .SetData(new UI.Guide.Data()
                        {
                            sentenceQueue = senteceQueue,
                        })
                        .Create();

                    return guide;
                });
        }

        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {

        }
    }
}

