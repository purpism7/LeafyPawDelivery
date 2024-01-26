using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Localization.Settings;

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

        public override void Initialize(BaseData data)
        {
            base.Initialize(data);

            SetSound();
            SetLanguage();
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

            UIUtils.SetActive(bgmCheckImg?.gameObject, onBGM);
            UIUtils.SetActive(effectCheckImg?.gameObject, onEffect);
        }

        private void SetBGM()
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            bool on = !setting.OnBGM;

            UIUtils.SetActive(bgmCheckImg?.gameObject, on);

            setting.SaveBGM(on);
        }

        private void SetEffect()
        {
            var setting = Info.Setting.Get;
            if (setting == null)
                return;

            bool on = !setting.OnEffect;

            UIUtils.SetActive(effectCheckImg?.gameObject, on);

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
            if(setting != null)
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
                UIUtils.SetActive(unselectedRootRectTms[i], i != index);
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
    }
}

