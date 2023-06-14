using System.Collections;
using System.Collections.Generic;
using GameData;
using UI;
using UnityEngine;

namespace Game
{
    public class UIManager : Singleton<UIManager>
    {
        public UI.Top Top;
        public UI.Bottom Bottom;
        public UI.Popup Popup;

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(base.CoInit(iProvider));
            
            Top?.Initialize(new UI.Top.Data()
            {

            });

            Bottom?.Initialize(new UI.Bottom.Data()
            {
                PopupRootRectTm = Popup.popupRootRectTm,
            });
            
            var openConditionMgr = iProvider.Get<Game.Manager.OpenCondition>();

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
    }
}

