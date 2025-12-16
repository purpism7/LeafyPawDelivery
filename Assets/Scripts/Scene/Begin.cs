using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem.Load;
using Game.Manager;
using UI;

namespace Scene
{
    public class Begin : Base, ContinueGame.IListener, NickName.IListener, Letter.IListener
    {
        private const string KeyPrologue = "Prologue";

        [SerializeField] private RectTransform uiRootRectTm = null;
        [SerializeField] private LoadData loadData = null;
        [SerializeField]
        private GameObject continueGameGameObj = null;
        [SerializeField] private GameObject prologueGameObj = null;
        [SerializeField] private GameObject nickNameGameObj = null;
        [SerializeField] private GameObject letterGameObj = null;

        private NickName _nickName = null;
        private Letter _letter = null;

        private bool _end = false;
        private Auth _auth = null;

        private async void Start()
        {
            InitializeScreenSetting();

            Info.Setting.Get?.InitializeLocale();

            _auth = new Auth();
            await _auth.AsyncInitializeAsync();
            await Plugin.Native.Instance.CoInit();

            string saveValue = string.Empty;

            if(!Application.isEditor)
            {
                bool end = false;

                if(!string.IsNullOrEmpty(GameSystem.Auth.ID))
                {
                    Plugin.Native.Instance?.GetString(Application.platform == RuntimePlatform.Android ? typeof(Info.User).Name : GameSystem.Auth.ID,
                   (success, value) =>
                   {
                       saveValue = value;

                       end = true;
                   });

                    await UniTask.WaitUntil(() => end);
                }
            }

            if (!string.IsNullOrEmpty(saveValue))
            {
                if(Info.UserManager.IsFirst)
                {
                    await ContinueGameAsync();
                }
            }

            await PlayPrologueAsync();
            await CreateNickNameAsync();
            CreateLetter();

            await UniTask.WaitUntil(() => _end);

            SceneLoader.LoadWithLoading(loadData);

            var firebaseMgr = FirebaseManager.Instance;
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);

            SceneLoader.LoadWithLoading(loadData);
        }

        private void InitializeScreenSetting()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //Screen.orientation = ScreenOrientation.AutoRotation;
            //Screen.autorotateToPortraitUpsideDown = true;
        }

        private async UniTask ContinueGameAsync()
        {
            _end = false;

            UI.ContinueGame continueGame = null;
            Sequencer.EnqueueTask(
                 () =>
                 {
                     var gameObj = Instantiate(continueGameGameObj, uiRootRectTm) as GameObject;
                     if (gameObj)
                     {                         
                         gameObj.transform.SetAsLastSibling();

                         continueGame = gameObj.GetComponent<ContinueGame>();
                         continueGame?.Initialize(this);
                     }

                     return continueGame;
                 });

            await UniTask.WaitUntil(() => _end);
        }

        private async UniTask PlayPrologueAsync()
        {
            bool already = false;
            //if(!Application.isEditor)
            {
                Boolean.TryParse(PlayerPrefs.GetString(KeyPrologue, false.ToString()), out already);
            }

            bool endPrologue = false;

            if (!already)
            {
                var cutscene = Cutscene.Create(
                    new Cutscene.Data()
                    {
                        orthographicSize = 1500f,
                        TargetGameObj = prologueGameObj,
                        EndAction = () =>
                        {
                            endPrologue = true;
                        },
                        IsConversation = false,
                    });

                PlayerPrefs.SetString(KeyPrologue, true.ToString());

                await UniTask.WaitUntil(() => endPrologue);
            }

            endPrologue = true;
        }

        private async UniTask CreateNickNameAsync()
        {
            if (!string.IsNullOrEmpty(Auth.NickName))
            {
                _end = true;

                return;
            }

            _end = false;

            Sequencer.EnqueueTask(
                () =>
                {
                    var gameObj = Instantiate(nickNameGameObj, uiRootRectTm) as GameObject;
                    if (gameObj)
                    {
                        gameObj.transform.SetAsLastSibling();

                        _nickName = gameObj.GetComponent<NickName>();
                        _nickName?.Initialize(this);
                    }

                    return _nickName;
                });

            await UniTask.Yield();
        }

        private void CreateLetter()
        {
            if (_end)
                return;

            Sequencer.EnqueueTask(
              () =>
              {
                  var gameObj = Instantiate(letterGameObj, uiRootRectTm) as GameObject;
                  if (gameObj)
                  {
                      gameObj.transform.SetAsLastSibling();

                      _letter = gameObj.GetComponent<Letter>();
                      _letter?.Initialize(this);
                  }

                  return _letter;
              });
        }

        #region ContinueGame.IListener
        void ContinueGame.IListener.New()
        {
            _auth?.SetGameType(Auth.EGameType.New);

            _end = true;
        }

        void ContinueGame.IListener.Continue()
        {
            _auth?.SetGameType(Auth.EGameType.Continue);

            _end = true;
        }
        #endregion

        void NickName.IListener.Confirm(string nickName)
        {
            _auth?.SetNickName(nickName);
        }

        void Letter.IListener.End()
        {
            _end = true;
        }
    }
}

