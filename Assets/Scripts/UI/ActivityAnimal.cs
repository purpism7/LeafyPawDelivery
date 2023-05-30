using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using GameSystem;
using UnityEngine.UI;

namespace UI
{
    public class ActivityAnimal : Base<ActivityAnimal.Data>
    {
        public SpriteRenderer animalSpr;
        public Image animalImg;

        public class Data : BaseData
        {
            public int AnimalId = 0;
            public string AnimalName = string.Empty;
            //public System.Action<int> EnableActivityAreaAction = null;
            public ActivityAnimalManager.SelectActivityAnimalDelegate SelectActivityAnimalDel = null;
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
            if(atlasLoader != null)
            {
                //animalSpr.SetSpritie(atlasLoader?.GetSprite(atlasLoader.KeyAnimalIcon, data.AnimalName));

                animalImg.sprite = atlasLoader?.GetSprite(atlasLoader.KeyAnimalIcon, data.AnimalName);
            }
        }

        public void OnClick()
        {
            if(_data == null)
            {
                return;
            }

            //_data.EnableActivityAreaAction?.Invoke(_data.AnimalId);
            _data.SelectActivityAnimalDel(_data.AnimalId);
        }
    }
}