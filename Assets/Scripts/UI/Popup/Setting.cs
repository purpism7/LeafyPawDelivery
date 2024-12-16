using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Localization.Settings;
using TMPro;

using GameSystem;

namespace UI
{
    public class Setting : BasePopup<BaseData>
    {
        [Header("Sound")]
        [SerializeField]
        private Image bgmCheckImg = null;
        [SerializeField]
        private Image effectCheckImg = null;
        [Header("Language")]
        [SerializeField]
        private RectTransform[] unselectedRootRectTms = null;
        [SerializeField]
        private TextMeshProUGUI buildVersionTMP = null;

        public override void Initialize(BaseData data)
        {
            base.Initialize(data);

            SetSound();
            SetLanguage();

            buildVersionTMP?.SetText(string.Format("ver. {0}", Application.version));
        }

        public override void Activate()
        {
            base.Activate();
        }

        #region Sound
        private void SetSound()
        {
            var setting = Info.Setting.Get;

            bool onBGM = true;
            bool onEffect = true;
            if (setting != null)
            {
                onBGM = setting.OnBGM;
                onEffect = setting.OnEffect;
            }
            
            bgmCheckImg?.SetActive(onBGM);
            effectCheckImg?.SetActive(onEffect);
        }

        private void SetBGM()
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            bool on = !setting.OnBGM;
            bgmCheckImg?.SetActive(on);

            setting.SaveBGM(on);
        }

        private void SetEffect()
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            bool on = !setting.OnEffect;
            effectCheckImg?.SetActive(on);

            setting.SaveEffect(on);
        }

        public void OnClickBGM()
        {
            SetBGM();

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }

        public void OnClickEffect()
        {
            SetEffect();

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }
        #endregion

        #region Language
        private void SetLanguage()
        {
            var setting = Info.Setting.Get;

            int index = 0;
            if (setting != null)
            {
                index = setting.LocaleIndex;
            }

            SetButtonState(index);
        }

        private void SetButtonState(int index)
        {
            if (unselectedRootRectTms == null)
                return;

            for (int i = 0; i < unselectedRootRectTms.Length; ++i)
            {
                GameUtils.SetActive(unselectedRootRectTms[i], i != index);
            }
        }
        public void OnClickLanguage(int index)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (locales == null)
                return;

            if (locales.Count <= index)
                return;

            var selectedLocale = locales[index];
            if (LocalizationSettings.SelectedLocale.Equals(selectedLocale))
                return;

            LocalizationSettings.SelectedLocale = selectedLocale;

            Info.Setting.Get?.SaveLocaleIndex(index);

            SetButtonState(index);

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }
        #endregion

        public void OnClickSNS()
        {
            Application.OpenURL("http://www.instagram.com/leafyparcels/");
        }

        public void OnClickGameTip()
        {
            var gameTip = new PopupCreator<GameTip, GameTip.Data>()
                .Create();
            gameTip?.Activate();
        }

        public void OnclickPrivacyPolish()
        {
            //Application.OpenURL("https://leafypawdelivery.blogspot.com/2024/02/blogger-30.html");
            Application.OpenURL("https://sites.google.com/view/leafy-parcels-privacy-policy/%ED%99%88");
        }
    }
}

