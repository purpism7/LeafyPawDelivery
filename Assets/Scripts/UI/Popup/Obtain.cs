using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.Localization.Settings;

using GameSystem;

namespace UI
{
    public class Obtain : BasePopup<Obtain.Data>
    {
        public class Data : BaseData
        {
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public int Id = 0;
            public int skinId = Game.Data.Const.AnimalBaseSkinId;
            public Action ClickAction = null;

            public bool keepRenderTexture = false;
        }

        [SerializeField] private Image iconImg = null;
        [SerializeField] private RectTransform renderTextureRootRectTm = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;

        private int _skinId = 0;

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
                _skinId = data.skinId;

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

        private void SetRenderTexture()
        {
            Game.RenderTextureElement.Destroy();

            if (_data == null)
                return;

            Game.RenderTextureElement.Create(
                new Game.RenderTextureElement.Data()
                {
                    Id = _data.Id,
                    SkinId = _skinId,
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

            nameTMP?.SetText(GameUtils.GetName(_data.EElement, _data.Id, _data.skinId));
        }

        public void OnClick()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            Deactivate();

            if(!_data.keepRenderTexture)
            {
                Game.RenderTextureElement.Destroy();
            }
        }
    }
}

