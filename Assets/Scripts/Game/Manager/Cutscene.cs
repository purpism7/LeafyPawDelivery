using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class Cutscene : Game.Base
    {
        #region Static
        private static Cutscene Cutscene_ = null;
        
        public static Cutscene Create(Func<bool> endFunc)
        {
            Cutscene_ = GameSystem.ResourceManager.Instance.InstantiateGame<Cutscene>(null);
            if (Cutscene_ != null)
            {
                Cutscene_.transform.position = new Vector3(-3000f, 0, 0);
                Cutscene_.Initialize(endFunc);
            }

            return Cutscene_;
        }

        public static bool Validate
        {
            get
            {
                return Cutscene_ != null;
            }
        }
        #endregion
        
        [SerializeField] private Transform timelineRootTm = null;
        [SerializeField] private RectTransform uiRootRectTm = null;
        
        private PlayableDirector _playableDirector = null;
        private Func<bool> _endFunc = null;

        private void Initialize(Func<bool> endFunc)
        {
            Debug.Log("Cutscene Initialize");
            
            Deactivate();
            
            if (!timelineRootTm)
            {
                // 종료시키자.
                End();
                
                return;
            }

            if (InitPlayableDirector())
            {
                _endFunc = endFunc;
                
                var cameras = timelineRootTm.GetComponentsInChildren<Camera>();
                foreach (var camera in cameras)
                {
                    camera?.gameObject.SetActive(false);
                }
                
                StartCoroutine(CoStart());
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

        private IEnumerator CoStart()
        {
            yield return new WaitForSeconds(1f);
            
            Activate();
            
            _playableDirector.Play();
        }

        private void End(PlayableDirector playableDirector)
        {
            End();
            StaticCoroutine.Start(CoEnd());
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
            Destroy(gameObject);
        }
    }
}

