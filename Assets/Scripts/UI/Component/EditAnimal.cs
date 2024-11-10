using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class EditAnimal : UI.Base<EditAnimal.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public Animal AnimalData = null;
            public bool isTutorial = false;
        }

        public interface IListener
        {
            void Select(int id);
        }

        [SerializeField] 
        private Image iconImg = null;
        [SerializeField]
        private Image guideLineImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetIconImg();
            SetTutorial();
        }

        public override void Activate()
        {
            base.Activate();

            GameUtils.SetActive(transform, true);
        }

        public override void Deactivate()
        {
            base.Deactivate();

            GameUtils.SetActive(transform, false);
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            if (_data.AnimalData == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(Game.Type.EElement.Animal, _data.AnimalData.Id);
        }

        private void SetTutorial()
        {
            if (_data == null)
                return;

            guideLineImg?.SetActive(_data.isTutorial);
            if (_data.isTutorial)
            { 
                guideLineImg?.StartBlink();
            }
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            if (_data.AnimalData == null)
                return;

            _data?.iListener?.Select(_data.AnimalData.Id);
        }
    }
}