using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class HeartCell : BaseComponent<HeartCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public int SkinId = 0;

            public int AddFriendShipPoint = 0;
            
            public Vector3 StartPos = Vector3.zero;
            public Vector3 EndPos = Vector3.zero;
            public System.Action EndAction = null;
        }

        [SerializeField] 
        private TextMeshProUGUI pointTMP = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }
        
        public override void Activate(Data data)
        {
            base.Activate(data);
            
            pointTMP?.SetText(string.Empty);
            
            if (data.AddFriendShipPoint > 0)
            {
                AddFriendShipPoint();
            }
            else
            {
                GiveGift();
            }
        }

        private void AddFriendShipPoint()
        {
            if (_data == null)
                return;
            
            var rectTm = GetComponent<RectTransform>();
            if (!rectTm)
                return;
            
            GameUtils.SetActive(rootRectTm, false);
            
            pointTMP?.SetText($"+{_data.AddFriendShipPoint}");
           
            rectTm.anchoredPosition = Vector2.zero;
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .AppendCallback(() => { GameUtils.SetActive(rootRectTm, true); })
                .Append(DOTween.To(() => rectTm.anchoredPosition, pos => rectTm.anchoredPosition = pos, new Vector2(rectTm.anchoredPosition.x, rectTm.anchoredPosition.y + 60f), 1.5f))
                .OnComplete(() =>
                {
                    Deactivate();
                });
            sequence.Restart();
        }
        
        private void GiveGift()
        {
            if (_data == null)
                return;

            var startPos = _data.StartPos;
            var endPos = _data.EndPos;
            
            var wayPoint = (endPos - startPos) / 2f;
            wayPoint += startPos;
            wayPoint.x += UnityEngine.Random.Range(-150f, 150f);
            
            startPos.z = 50f;
            wayPoint.z = startPos.z;
            endPos.z = startPos.z;
            
            var wayPoints = new[] { startPos, wayPoint, endPos };
            var duration = 0.5f;
            
            // var animal = Game.RenderTextureElement.GetAnimal(_data.Id, _data.SkinId);
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(transform.DOMove(_data.StartPos, 0))
                .AppendCallback(() => { Activate(); })
                .Append(transform.DOPath(wayPoints, duration, PathType.CatmullRom).SetEase(Ease.Linear))
                .AppendInterval(0.3f)
                .OnComplete(() =>
                {
                    Deactivate();
                    
                    _data?.EndAction?.Invoke();
                    //
                    // _data?.CollectEndAction();
                });
            sequence.Restart();
        }
    }
}

