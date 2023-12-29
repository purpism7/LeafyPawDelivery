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
        private Toggle[] langToggles = null;

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

            bool on = true;
            if (setting != null)
            {
                on = setting.OnBGM;
            }

            UIUtils.SetActive(bgmCheckImg?.gameObject, on);
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
        }

        public void OnClickEffect()
        {
            SetEffect();
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

            if (langToggles == null)
                return;

            if (langToggles.Length <= index)
                return;

            langToggles[index]?.SetIsOnWithoutNotify(true);
        }

        public void OnChangedLanguage(int index)
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

            if(langToggles != null)
            {
                for (int i = 0; i < langToggles.Length; ++i)
                {
                    langToggles[i]?.SetIsOnWithoutNotify(i == index);
                }
            }
        }
        #endregion
    }
}

