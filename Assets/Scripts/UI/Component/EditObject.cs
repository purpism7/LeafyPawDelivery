using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Game;

namespace UI.Component
{
    public class EditObject : UI.Base<EditObject.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public int ObjectId = 0;
            public int Count = 0;
            public int RemainCount = 0;
            public bool isTutorial = false;
        }

        public interface IListener
        {
            void Select(int id);
        }

        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI countTMP = null;
        [SerializeField]
        private Image guideLineImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            SetIconImg();
            SetCount();
            SetTutorial();
        }

        public override void Activate()
        {
            base.Activate();

            UIUtils.SetActive(gameObject, true);
        }

        public override void Deactivate()
        {
            base.Deactivate();

            UIUtils.SetActive(gameObject, false);
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(Game.Type.EElement.Object, _data.ObjectId);
        }

        private void SetCount()
        {
            if (_data == null)
                return;

            countTMP?.SetText(_data.RemainCount + "/" + _data.Count);
        }

        private void SetTutorial()
        {
            if (_data == null)
                return;

            UIUtils.SetActive(guideLineImg?.gameObject, _data.isTutorial);

            if (_data.isTutorial)
            {
                guideLineImg?.StartBlink();

                ObjectManager.Event?.RemoveListener(OnChangedObject);
                ObjectManager.Event?.AddListener(OnChangedObject);
            }
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            _data.iListener?.Select(_data.ObjectId);
        }

        private void OnChangedObject(Game.Event.ObjectData objectData)
        {
            switch (objectData)
            {
                case Game.Event.ArrangeObjectData arrangeObjectData:
                    {
                        if (arrangeObjectData.id == Game.Data.Const.TutorialObjectId)
                        {
                            _data.isTutorial = false;

                            guideLineImg?.StopBlink();
                            UIUtils.SetActive(guideLineImg?.gameObject, false);

                            ObjectManager.Event?.RemoveListener(OnChangedObject);
                        }

                        break;
                    }
            }
        }
    }
}

