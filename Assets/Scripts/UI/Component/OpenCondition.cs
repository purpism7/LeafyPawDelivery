using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace UI.Component
{
    public class OpenCondition : Base<OpenCondition.Data>
    {
        public class Data : BaseData
        {
            public string Text = string.Empty;
        }

        #region Inspector
        [SerializeField]
        private TextMeshProUGUI textTMP = null;
        #endregion

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetText(data.Text);
        }

        public override void Activate()
        {
            base.Activate();
        }

        private void SetText(string text)
        {
            textTMP?.SetText(text);
        }
    }
}

