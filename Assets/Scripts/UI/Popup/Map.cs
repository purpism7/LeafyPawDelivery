using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

using GameSystem;
using UI.Component;

namespace UI
{
    public class Map : BasePopup<Map.Data>, MapIcon.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private MapIcon[] mapIcons = null;
        [SerializeField]
        private RectTransform myLocationRectTm = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InternalInitialize();
        }
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InternalInitialize();
            
            yield break;
        }

        private void InternalInitialize()
        {
            SetMapIcons();

            InitializeChildComponent();

            Sequence sequence = DOTween.Sequence()
              .SetAutoKill(false)
              .Append(myLocationRectTm.DOJumpAnchorPos(Vector2.up * 2f, 15f, 1, 0.7f))
              .AppendInterval(0.1f);
            sequence.Restart();
            sequence.SetLoops(-1);
        }

        public override void Activate()
        {
            base.Activate();

            ActivateChildComponent(typeof(MapIcon));
        }

        private void SetMapIcons()
        {
            if (mapIcons == null)
                return;

            foreach(var mapIcon in mapIcons)
            {
                if (mapIcon == null)
                    continue;

                mapIcon?.Initialize(
                    new MapIcon.Data()
                    {
                        IListener = this,
                    });
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        // public void OnChanged(string tabType)
        // {
        //     if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
        //     {
        //         if(_currETabType == eTabType)
        //         {
        //             return;
        //         }
        //
        //         _currETabType = eTabType;
        //
        //         ActiveContents();
        //     }
        // }

        public void OnClickClose()
        {
            Deactivate();
        }

        #region MapIcon.IListener
        void MapIcon.IListener.SelectPlace(int placeId, System.Action<RectTransform> action)
        {
            if (GameUtils.ActivityPlaceId == placeId)
                return;

            Deactivate();

            action?.Invoke(myLocationRectTm);

            Sequencer.EnqueueTask(
                () =>
                {
                    var loading = new PopupCreator<Loading, Loading.Data>()
                       .SetReInitialize(true)
                       .SetAnimActivate(false)
                       .SetData(new Loading.Data()
                       {
                           PlaceId = placeId,
                       })
                       .Create();

                    return loading;
                }); 
        }

        void MapIcon.IListener.SetMyLocation(int placeId, System.Action<RectTransform> action)
        {
            if (GameUtils.ActivityPlaceId != placeId)
                return;

            action?.Invoke(myLocationRectTm);
        }
        #endregion
    }
}