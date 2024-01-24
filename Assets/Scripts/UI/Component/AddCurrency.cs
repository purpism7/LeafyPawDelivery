using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using TMPro;


namespace UI.Component
{
    public class AddCurrency : Base<AddCurrency.Data>
    {
        [SerializeField] private TextMeshProUGUI currencyTMP;
        
        public class Data : BaseData
        {
            public Vector3 StartPos = Vector3.zero;
            public Game.Type.EElement EElement = Game.Type.EElement.None;
            public int Currency = 0;
            public Color color = Color.white;
            public System.Action CompleteAction = null;
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Deactivate();
            
            Initialize();
            
            Add();
        }

        private void Initialize()
        {
            SetText();
        }

        private void SetText()
        {
            currencyTMP?.SetText(string.Empty);

            if (_data == null)
                return;

            if (currencyTMP == null)
                return;

            currencyTMP.color = _data.color;
            currencyTMP.SetText("+" + _data.Currency);
        }

        private void Add()
        {
            var rectTm = GetComponent<RectTransform>();
            if (!rectTm)
                return;
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                .Append(rectTm.DOLocalMoveY(60f, 1.2f).SetEase(Ease.OutCirc))
                .Append(currencyTMP.DOFade(0, 0.3f))
                .OnComplete(() =>
                {
                    Deactivate();

                    currencyTMP.DOFade(1, 0);
                });
            sequence.Restart();
        }
    }
}
