using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class Cutscene : Game.Common
    {
        #region Static

        private static Cutscene Cutscene_ = null;
        public static Cutscene Create(Data data)
        {
            if (data == null)
                return null;
            
            if (Cutscene_ == null)
            {
                Cutscene_ = GameSystem.ResourceManager.Instance.InstantiateGame<Cutscene>(null);
            }
            
            if (Cutscene_ != null)
            {
                Cutscene_.transform.position = new Vector3(-3000f, 0, 0);
                Cutscene_.Initialize(data);
            }

            return Cutscene_;
        }

        public static bool Validate
        {
            get { return Cutscene_ != null; }
        }

        #endregion

        public class Data
        {
            public GameObject TargetGameObj = null;
            public Func<bool> EndFunc = null;
            public Action EndAction = null;
        }

        [SerializeField] private Transform timelineRootTm = null;
        [SerializeField] private RectTransform uiRootRectTm = null;

        private Data _data = null;
        private PlayableDirector _playableDirector = null;

        private void Initialize(Data data)
        {
            _data = data;
                
            Deactivate();

            if (!timelineRootTm)
            {
                // 종료시키자.
                Finish();

                return;
            }

            GameObject.Instantiate(_data.TargetGameObj, timelineRootTm);

            DestoryAllChild();

            if (InitPlayableDirector())
            {
                DeactiveCameras();

                Fade.Create.Out(() => { StartCoroutine(CoStart()); });
            }
        }

        private void DestoryAllChild()
        {
            foreach (Transform tm in timelineRootTm)
            {
                if(!tm)
                    continue;
                
                GameObject.Destroy(tm);
            }
        }

        private bool InitPlayableDirector()
        {
            _playableDirector = timelineRootTm.GetComponentInChildren<PlayableDirector>();
            if (_playableDirector == null)
            {
                Finish();

                return false;
            }

            _playableDirector.extrapolationMode = DirectorWrapMode.None;
            _playableDirector.playOnAwake = false;

            _playableDirector.stopped += Finish;

            return true;
        }

        private void DeactiveCameras()
        {
            var cameras = timelineRootTm.GetComponentsInChildren<Camera>();
            foreach (var camera in cameras)
            {
                camera?.gameObject.SetActive(false);
            }
        }

        private IEnumerator CoStart()
        {
            Activate();

            yield return null;

            Fade.Create.In(() => { _playableDirector.Play(); });
        }

        private void Finish(PlayableDirector playableDirector)
        {
            StartCoroutine(CoFinish());
        }

        private IEnumerator CoFinish()
        {
            if (_data?.EndFunc != null)
            {
                yield return new WaitUntil(() => _data.EndFunc.Invoke());
            }

            Finish();
        }

        private void Finish()
        {
            Fade.Create.Out(() =>
            {
                Deactivate();
                
                Fade.Create.In(() =>
                {
                    _data.EndAction();
                    
                    Destroy(gameObject);
                });
            });
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override bool End
        {
            get
            {
                return false;
            }
        }
    }
}

