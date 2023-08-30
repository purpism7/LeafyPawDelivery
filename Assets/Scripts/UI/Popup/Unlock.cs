using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using static ConversationBehaviour;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class Unlock : BasePopup<Unlock.Data>
    {
        [SerializeField] private Image iconImg = null;
        [SerializeField] private RectTransform renderTextureRootRectTm = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;

        public class Data : BaseData
        {
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public int Id = 0;
            public Action ClickAction = null;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            SetNameTMP();

            UIUtils.SetActive(iconImg?.gameObject, false);
            UIUtils.SetActive(renderTextureRootRectTm, false);

            if (data == null)
                yield break;

            if(data.EElement == Game.Type.EElement.Animal)
            {
                SetRenderTexture();
            }
            else if(data.EElement == Game.Type.EElement.Object)
            {
                SetImg();
            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            if (_data != null)
            {
                _data?.ClickAction?.Invoke();
            }

            _endTask = true;
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }

        public override bool End
        {
            get
            {
                return _endTask;
            }
        }

        private void SetRenderTexture()
        {
            if (_data == null)
                return;

            Game.RenderTextureElement.Create(
                new Game.RenderTextureElement.Data()
                {
                    Id = _data.Id,
                    EElement = _data.EElement,
                });

            UIUtils.SetActive(renderTextureRootRectTm, true);
        }

        private void SetImg()
        {
            if (iconImg == null)
                return;

            var sprite = GameUtils.GetLargeIconSprite(_data.EElement, _data.Id);
            iconImg.sprite = sprite;

            UIUtils.SetActive(iconImg?.gameObject, true);
        }

        private void SetNameTMP()
        {
            if (_data == null)
                return;

            nameTMP?.SetText(GameUtils.GetName(_data.EElement, _data.Id));
        }

        public void OnClick()
        {
            Deactivate();

            Game.RenderTextureElement.Destroy();
        }
    }
}

