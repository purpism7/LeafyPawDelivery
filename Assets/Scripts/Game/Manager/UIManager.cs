using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Game
{
    public class UIManager : Singleton<UIManager>
    {
        public UI.Top Top;
        public UI.Bottom Bottom;
        public UI.Popup Popup;

        public override IEnumerator CoInit()
        {
            Top?.Initialize(new UI.Top.Data()
            {

            });

            Bottom?.Initialize(new UI.Bottom.Data()
            {
                PopupRootRectTm = Popup.RootRectTm,
            });

            yield return null;
        }

        public T Instantiate<T>(RectTransform rootRectTm)
        {
            return GameSystem.ResourceManager.Instance.InstantiateUI<T>(rootRectTm);
        }
    }
}

