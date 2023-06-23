using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace UI.Component
{
    public class AddCurrency : Base<AddCurrency.Data>
    {
        [SerializeField] private TextMeshProUGUI currencyTMP;
        
        public class Data : BaseData
        {
            public Vector3 StartPos = Vector3.zero;
            public int Currency = 0;
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
            currencyTMP?.SetText("+" + _data.Currency);
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
                .Append(rectTm.DOLocalMoveY(70f, 1f).SetEase(Ease.OutCirc))
                .Append(currencyTMP.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    Deactivate();

                    currencyTMP.DOFade(1, 0);
                });
            sequence.Restart();
        }
    }
}
