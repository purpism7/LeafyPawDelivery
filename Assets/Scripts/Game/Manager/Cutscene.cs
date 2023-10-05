using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    [ExecuteInEditMode]
    public class Cutscene : Game.Common, Sequencer.ITask, Conversation.IListener
    {
        #region Static
        private static Cutscene _instance = null;

        public static Cutscene Instance
        {
            get
            {
                if (Application.isEditor)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<Cutscene>();

                        if(_instance == null)
                        {
                            InstantiateCutscene();
                        }
                    }
                }

                return _instance;
            }

            private set
            {
                _instance = value;
            } 
        }

        private static void InstantiateCutscene()
        {
            var gameObj = Instantiate(Resources.Load("Cutscene")) as GameObject;
            if (!gameObj)
                return;

            _instance = gameObj.GetComponent<Cutscene>();
        }

        public static Cutscene Create(Data data)
        {
            if (data == null)
                return null;
            
            if (_instance == null)
            {
                InstantiateCutscene();
            }
            
            if (_instance != null)
            {
                _instance.transform.position = new Vector3(0, 5000f, 0);
                _instance.Initialize(data);
            }

            return _instance;
        }

        public static bool Validate
        {
            get { return Instance != null; }
        }

        #endregion

        public class Data
        {
            public GameObject TargetGameObj = null;            
            public Func<bool> EndFunc = null;
            public Action EndAction = null;
            public bool IsConversation = true;
        }

        [SerializeField] private Transform timelineRootTm = null;
        [SerializeField] private RectTransform uiRootRectTm = null;

        private Data _data = null;
        private PlayableDirector _playableDirector = null;
        private bool _end = false;

        public Conversation Conversation { get; private set; } = null;

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

            DestoryAllChild();

            GameObject.Instantiate(_data.TargetGameObj, timelineRootTm);

            if (InitPlayableDirector())
            {
                DeactiveCameras();

                Fade.Create.Out(
                    () =>
                    {
                        StartCoroutine(CoStart());
                    });
            }
        }

        private void DestoryAllChild()
        {
            foreach (Transform tm in timelineRootTm)
            {
                if(!tm)
                    continue;
                
                GameObject.DestroyImmediate(tm.gameObject);
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

            if(_data.IsConversation)
            {
                CreateConvesation();
            }

            yield return null;

            _playableDirector.Play();

            Fade.Create.In(
                () =>
                {
                    UIManager.Instance?.EnalbeUIRoot(false);
                });
        }

        private void CreateConvesation()
        {
            Conversation = new UICreator<Conversation, Conversation.Data>()
                .SetData(new Conversation.Data()
                {
                    IListener = this,
                })
               .SetRootRectTm(uiRootRectTm)
               .Create();


            UIUtils.SetActive(Conversation?.rootRectTm, false);
            //Conversation?.Deactivate();
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
                UIManager.Instance?.EnalbeUIRoot(true);

                Deactivate();

                _end = true;

                _data.EndAction();

                Fade.Create.In(() =>
                {
                    Destroy(gameObject);
                });
            });
        }

        public override void Begin()
        {
            base.Begin();

            _end = false;
        }

        public override bool End
        {
            get
            {
                return _end;
            }
        }

        #region Conversation.IListener
        void Conversation.IListener.FinishTyping(int remainCnt)
        {

        }
        #endregion
    }
}

