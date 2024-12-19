using System.Collections;
using System.Collections.Generic;
using Game;
using GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI
{
    public class OpenContent : BasePopup<OpenContent.Data>
    {
        public class Data : BaseData
        {
            public Type.EContent EContent = Type.EContent.None;
        }

        [SerializeField] 
        private TextMeshProUGUI descTMP = null;
        
        [SerializeField] 
        private Button okBtn = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            okBtn?.onClick?.RemoveAllListeners();
            okBtn?.onClick?.AddListener(OnClickClose);
        }

        public override void Activate(Data data)
        {
            base.Activate(data);
            
            SetDescTMP();
        }

        private void SetDescTMP()
        {
            descTMP?.SetText(string.Empty);
            
            if (_data == null)
                return;

            var localKey = $"desc_open_content_{_data.EContent}".ToLower();
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);
            
            descTMP?.SetText(local);
        }

        private void OnClickClose()
        {
            Deactivate();
            
            var gameTip = new PopupCreator<GameTip, GameTip.Data>()
                .Create();
        }
    }
}

