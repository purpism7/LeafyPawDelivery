using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using GameSystem.Load;
using Game.Manager;
using UI;

namespace Scene
{
    public class Begin : Base, NickName.IListener, Letter.IListener
    {
        private const string KeyPrologue = "Prologue";

        [SerializeField] private RectTransform uiRootRectTm = null;
        [SerializeField] private LoadData loadData = null;
        [SerializeField] private GameObject prologueGameObj = null;
        [SerializeField] private GameObject nickNameGameObj = null;
        [SerializeField] private GameObject letterGameObj = null;

        private NickName _nickName = null;
        private Letter _letter = null;

        private bool _end = false;
        private Auth _auth = null;

        private async void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Info.Setting.Get?.InitializeLocale();

            var firebaseMgr = FirebaseManager.Instance;

            _auth = new Auth();
            await _auth.AsyncInitialize();

            await Plugin.Native.Instance.CoInit();

            Debug.Log("end Native Initialize 222");

            //await firebaseMgr.CoInit();

            //yield return new WaitForSeconds(2f);

            //var nickName = PlayerPrefs.GetString("KeyNickName");

            //bool already = false;
            //Boolean.TryParse(PlayerPrefs.GetString(KeyPrologue, false.ToString()), out already);

            //bool endPrologue = false;

            //if (!already)
            //{
            //    var cutscene = Cutscene.Create(new Cutscene.Data()
            //    {
            //        TargetGameObj = prologueGameObj,
            //        EndAction = () =>
            //        {
            //            endPrologue = true;
            //            CreateNickName();
            //            CreateLetter();
            //        },
            //        IsConversation = false,
            //    });

            //    await UniTask.WaitUntil(() => endPrologue);
            //}

            await UniTask.WaitForSeconds(2f);

            await PlayPrologueAsync();
            await CreateNickNameAsync();
            CreateLetter();

            await UniTask.WaitUntil(() => _end);

            SceneLoader.LoadWithLoading(loadData);
            //PlayerPrefs.GetString(KeyPrologue)
            //if(firebaseMgr.Auth.IsFirst)
            //{
            //    var cutscene = Cutscene.Create(new Cutscene.Data()
            //    {
            //        TargetGameObj = prologueGameObj,
            //        EndAction = () =>
            //        {
            //            //CreateNickName();
            //            //CreateLetter();


            //        },
            //        IsConversation = false,
            //    });

            //    //while (!_endPrologue)
            //    //{
            //    //    yield return null;
            //    //}
            //}

            //await UniTask.WaitUntil(() => _endPrologue);

            //if (firebaseMgr.Auth.IsValid)
            //{
            //    SceneLoader.LoadWithLoading(loadData);

            //    yield break;
            //}
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);

            SceneLoader.LoadWithLoading(loadData);
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
                var cutscene = Cutscene.Create(new Cutscene.Data()
                {
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

