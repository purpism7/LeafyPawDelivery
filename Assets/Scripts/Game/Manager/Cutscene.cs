using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class Cutscene : Game.Base
    {
        #region Static

        private static Cutscene Cutscene_ = null;

        // story cutscene 일 경우 Id 필요.
        public static Cutscene Create(Func<bool> endFunc, int id = 0)
        {
            if (Cutscene_ == null)
            {
                Cutscene_ = GameSystem.ResourceManager.Instance.InstantiateGame<Cutscene>(null);
            }
            
            if (Cutscene_ != null)
            {
                Cutscene_.transform.position = new Vector3(-3000f, 0, 0);
                Cutscene_.Initialize(endFunc);
            }

            return Cutscene_;
        }

        public static bool Validate
        {
            get { return Cutscene_ != null; }
        }

        #endregion

        [SerializeField] private Transform timelineRootTm = null;
        [SerializeField] private RectTransform uiRootRectTm = null;

        private PlayableDirector _playableDirector = null;
        private Func<bool> _endFunc = null;

        private void Initialize(Func<bool> endFunc)
        {
            Deactivate();

            if (!timelineRootTm)
            {
                // 종료시키자.
                End();

                return;
            }

            DestoryAllChild();

            if (InitPlayableDirector())
            {
                _endFunc = endFunc;

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
                End();

                return false;
            }

            _playableDirector.extrapolationMode = DirectorWrapMode.None;
            _playableDirector.playOnAwake = false;

            _playableDirector.stopped += End;

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

        private void End(PlayableDirector playableDirector)
        {
            StartCoroutine(CoEnd());
        }

        private IEnumerator CoEnd()
        {
            if (_endFunc != null)
            {
                yield return new WaitUntil(() => _endFunc.Invoke());
            }

            End();
        }

        private void End()
        {
            Fade.Create.Out(() =>
            {
                Deactivate();
                
                Fade.Create.In(() => { Destroy(gameObject); });
            });
        }
    }
}

