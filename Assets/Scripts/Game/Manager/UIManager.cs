using System.Collections;
using System.Collections.Generic;
using GameData;
using UI;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private RectTransform uiRootRectTm = null;
        public UI.Top Top;
        public UI.Bottom Bottom;
        public UI.Popup Popup;

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(base.CoInit(iProvider));

            int activityPlaceId = 1;
            var placeMgr = MainGameManager.Instance?.placeMgr;
            if (placeMgr != null)
            {
                activityPlaceId = placeMgr.ActivityPlaceId;
            }

            //MainGameManager.Instance?.placeMgr.ActivityPlaceId;
            
            Top?.Initialize(new UI.Top.Data()
            {
                PlaceId = activityPlaceId,
            });

            Bottom?.Initialize(new UI.Bottom.Data()
            {
                PopupRootRectTm = Popup.popupRootRectTm,
            });

            yield return null;
        }

        public T Instantiate<T>(RectTransform rootRectTm)
        {
            return GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootRectTm);
        }

        private void OpenCountListener(int count)
        {
            Debug.Log("OpenCount " + count);
        }

        public void EnalbeUIRoot(bool enable)
        {
            UIUtils.SetActive(uiRootRectTm, enable);
        }
    }
}

