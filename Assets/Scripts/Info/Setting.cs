using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace Info
{
    public class Setting
    {
        private readonly string KeyBGM = string.Empty;
        private readonly string KeyEffect = string.Empty;
        private readonly string KEYLOCALEINDEX = string.Empty;

        public static UnityEvent<Game.Event.SettingData> Event { get; private set; } = new();

        private static Setting _instance = null;

        public static Setting Get
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }

        public bool OnBGM
        {
            get
            {
                string onBGM = PlayerPrefs.GetString(KeyBGM, true.ToString());
                if (Boolean.TryParse(onBGM, out bool on))
                {
                    return on;
                }

                return true;
            }
        }

        public bool OnEffect
        {
            get
            {
                string onEffect = PlayerPrefs.GetString(KeyEffect, true.ToString());
                if (Boolean.TryParse(onEffect, out bool on))
                {
                    return on;
                }

                return true;
            }
        }
        
        public int LocaleIndex
        {
            get
            {
                return PlayerPrefs.GetInt(KEYLOCALEINDEX, 0);
            }
        }

        public Setting()
        {
            KeyBGM = GetType().Name + "_BGM";
            KeyEffect = GetType().Name + "_Effect";
            KEYLOCALEINDEX = GetType().Name + "_LocaleIndex";
        }

        public void SaveBGM(bool on)
        {
            PlayerPrefs.SetString(KeyBGM, on.ToString());

            Event?.Invoke(
                new Game.Event.BGMData()
                {
                    on = on,
                });
        }

        public void SaveEffect(bool on)
        {
            PlayerPrefs.SetString(KeyEffect, on.ToString());
        }

        public void SaveLocaleIndex(int index)
        {
            Debug.Log("SaveLocaleIndex = " + index);
            PlayerPrefs.SetInt(KEYLOCALEINDEX, index);
        }

        public void InitializeLocale()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (locales == null)
                return;

            int index = LocaleIndex;
            if (locales.Count <= index)
                return;

            Debug.Log("InitializeLocale = " + locales[index].LocaleName);
            LocalizationSettings.SelectedLocale = locales[index];
        }
    }
}

