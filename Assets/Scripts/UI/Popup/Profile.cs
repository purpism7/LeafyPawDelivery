using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class Profile : BasePopup<Profile.Data>
    {
        public class Data : BaseData
        {
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public int Id = 0;
        }

        [SerializeField] private Image iconImg = null;
        [SerializeField] private RectTransform renderTextureRootRectTm = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] private TextMeshProUGUI descTMP = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            UIUtils.SetActive(iconImg?.gameObject, false);
            UIUtils.SetActive(renderTextureRootRectTm, false);

            if (data == null)
                return;

            SetNameTMP();
            SetDescTMP();

            if (data.EElement == Game.Type.EElement.Animal)
            {
                SetRenderTexture();
            }
            else if (data.EElement == Game.Type.EElement.Object)
            {
                SetImg();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Game.RenderTextureElement.Destroy();
        }

        private void SetNameTMP()
        {
            if (_data == null)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id);

            nameTMP?.SetText(localName);
        }

        private void SetDescTMP()
        {
            var text = string.Empty;
            if (_data.EElement == Game.Type.EElement.Animal)
            {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(_data.EElement.ToString(), "description_" + _data.Id, LocalizationSettings.SelectedLocale);
            }

            descTMP?.SetText(text);
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
    }
}

