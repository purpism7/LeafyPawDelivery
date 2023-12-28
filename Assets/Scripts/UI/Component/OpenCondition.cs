using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class OpenCondition : Base<OpenCondition.Data>
    {
        public class Data : BaseData
        {
            public Sprite ImgSprite = null;
            public string Text = string.Empty;
            public System.Func<bool> PossibleFunc = null;
        }

        #region Inspector
        [SerializeField]
        private Image img = null;
        [SerializeField]
        private TextMeshProUGUI textTMP = null;
        [SerializeField]
        private ContentSizeFitter textContentSizeFitter = null;
        #endregion

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetImg();
            SetText(data.Text);
            SetColor();   
        }

        public override void Activate()
        {
            base.Activate();

            SetColor();

            LayoutRebuilder.ForceRebuildLayoutImmediate(textContentSizeFitter?.GetComponent<RectTransform>());
        }

        private void SetImg()
        {
            if (img == null)
                return;

            var sprite = _data?.ImgSprite;
            if (sprite == null)
                return;

            img.sprite = sprite;
        }

        private void SetText(string text)
        {
            textTMP?.SetText(text);
        }

        private void SetColor()
        {
            if (_data == null)
                return;

            if (_data.PossibleFunc == null)
                return;

            textTMP.color = _data.PossibleFunc() ? Color.black : Color.red;
        }
    }
}

