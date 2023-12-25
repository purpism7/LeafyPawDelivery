using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Toast : MonoBehaviour
    {
        private static Toast _instance = null;

        public static Toast Get
        {
            get
            {
                return _instance;
            }
        }

        [SerializeField]
        private RectTransform rootRectTm = null;

        private List<UI.Component.Toast> _toastList = new();
        private int cnt = 0;

        private void Awake()
        {
            _instance = this;
        }

        public void Show(string localKey)
        {
            var toast = Create(new UI.Component.Toast.Data()
            {
                localKey = localKey + (++cnt),
            });
        }

        private UI.Component.Toast Create(UI.Component.Toast.Data toastData)
        {
            if (toastData == null)
                return null;

            UI.Component.Toast toast = null;

            if (_toastList != null)
            {
                for(int i = 0; i < _toastList.Count; ++i)
                {
                    toast = _toastList[i];
                    if (toast == null)
                        continue;

                    if (toast.IsActivate)
                        continue;

                    toast.Initialize(toastData);

                    toast.transform.SetAsLastSibling();

                    return toast;
                }
            }

            toast = new GameSystem.ComponentCreator<UI.Component.Toast, UI.Component.Toast.Data>()
                .SetData(toastData)
                .SetRootRectTm(rootRectTm)
                .Create();

            toast?.transform.SetAsLastSibling();

            _toastList?.Add(toast);

            return toast;
        }
    }
}

