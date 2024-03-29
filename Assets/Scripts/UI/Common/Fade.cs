using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class Fade : Base<BaseData>
    {
        #region Static
        private static Fade _instance = null;
        public static Fade Create
        {
           get
           {
               if (_instance == null)
               {
                    var gameObj = Instantiate(Resources.Load("Fade")) as GameObject;
                    if (gameObj)
                    {
                        _instance = gameObj.GetComponent<Fade>();
                    }

                    //_instance = GameSystem.ResourceManager.Instance?.InstantiateUI<Fade>(null);
               }
              
               if (_instance != null)
               {
                    _instance.transform.position = new Vector3(-3000f, -1000f);
                    _instance.transform.SetAsLastSibling();
                    _instance.Initialize(null);
               }

               return _instance;
           }
        }
        #endregion
        
        readonly float _duration = 1f;

        public Image img = null;
        [SerializeField] private Button btn = null;

        public override void Initialize(BaseData data)
        {
            base.Initialize(data);
        }

        private void SetColorAlpha(float alpha)
        {
            if (img == null)
                return;
            
            var tmpColor = img.color;
            tmpColor.a = alpha;
            img.color = tmpColor;
        }

        public Fade Out(System.Action completeAction)
        {
            SetInteratable(true);
            
            SetColorAlpha(0);
            
            Activate();

            FadeInOut(1f, _duration,
                () =>
                {
                    completeAction?.Invoke();

                    return;
                });

            return this;
        }

        public Fade In(System.Action completeAction)
        {
            SetColorAlpha(1f);
            
            FadeInOut(0, _duration,
                () =>
                {
                    // UIUtils.SetActive(Img.transform, false);
                    completeAction?.Invoke();
                    
                    Deactivate();
                    SetInteratable(false);
                });

            return this;
        }

        private void FadeInOut(float alpha, float duration, System.Action completeAction)
        {
            if (img == null)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(img.DOFade(alpha, duration))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });

            sequence.Restart();
        }

        private void SetInteratable(bool interatable)
        {
            if (btn == null)
                return;
            
            btn.interactable = interatable;
        }
    }
}

