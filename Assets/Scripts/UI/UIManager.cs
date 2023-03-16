using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace GameSystem
{
    public class UIManager : Singleton<UIManager>
    {
        public UI.Top Top;
        public UI.Bottom Bottom;
        public UI.Popup Popup;
        public UI.Fade Fade;

        public override IEnumerator CoInit()
        {
            Top?.Init(new UI.Top.Data()
            {

            });

            Popup?.Init(new UI.Popup.Data()
            {

            });

            Bottom?.Init(new UI.Bottom.Data()
            {
                PopupRootRectTm = Popup.RootRectTm,
            });

            Fade?.Init(null);

            yield return null;
        }

        public T Instantiate<T>(RectTransform rootRectTm)
        {
            return ResourceManager.Instance.InstantiateUI<T>(rootRectTm);
        }

        public T InstantiatePopup<T>()
        {
            return ResourceManager.Instance.InstantiateUI<T>(Popup?.RootRectTm);
        }
    }
}

