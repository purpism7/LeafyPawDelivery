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
        [Tooltip("Localization Tables 에 언어 순서랑 동일해야 됨.")]
        [SerializeField]
        private Toggle[] langToggles = null;

        private Info.Setting _setting = new();

        public override void Initialize(BaseData data)
        {
            base.Initialize(data);

            SetLanguage();
            SetSound();
        }

        public override void Activate()
        {
            base.Activate();
        }

        #region Sound
        private void SetSound()
        {
            bool on = true;
            if (_setting != null)
            {
                on = _setting.OnBGM;
            }

            UIUtils.SetActive(bgmCheckImg?.gameObject, on);
        }

        private void SetBGM()
        {
            if (_setting == null)
                return;

            bool on = !_setting.OnBGM;

            UIUtils.SetActive(bgmCheckImg?.gameObject, on);

            _setting?.SaveBGM(on);
        }

        public void OnClickBGM()
        {
            SetBGM();
        }

        public void OnClickEffect()
        {

        }
        #endregion

        private void SetLanguage()
        {
            int index = 0;
            if(_setting != null)
            {
                index = _setting.LocaleIndex;
            }

            if (langToggles == null)
                return;

            if (langToggles.Length <= index)
                return;

            langToggles[index].SetIsOnWithoutNotify(true);
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

            _setting?.SaveLocaleIndex(index);
        }
    }
}

