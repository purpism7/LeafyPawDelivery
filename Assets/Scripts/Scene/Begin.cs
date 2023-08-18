using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using GameSystem.Load;
using Game.Manager;
using UI;

namespace Scene
{
    public class Begin : Base, NickName.IListener, Letter.IListener
    {
        [SerializeField] private RectTransform uiRootRectTm = null;
        [SerializeField] private LoadData loadData = null;
        [SerializeField] private GameObject prologueGameObj = null;
        [SerializeField] private GameObject nickNameGameObj = null;
        [SerializeField] private GameObject letterGameObj = null;

        private NickName _nickName = null;
        private Letter _letter = null;
        private bool _endPrologue = false;

        private IEnumerator Start()
        {
            LocalizationSettings.SelectedLocale = UnityEngine.Localization.Locale.CreateLocale("ko");

            var firebaseMgr = FirebaseManager.Instance;

            //yield return StartCoroutine(firebaseMgr.CoInit());

            //_iListener?.EndLoad();

            var nickName = PlayerPrefs.GetString("KeyNickName");

            _endPrologue = false;
            //if(firebaseMgr.Auth.IsFirst)
            {
                var cutscene = Cutscene.Create(new Cutscene.Data()
                {
                    TargetGameObj = prologueGameObj,
                    EndAction = () =>
                    {
                        CreateNickName();
                        CreateLetter();
                    },
                    IsConversation = false,
                });

                while (!_endPrologue)
                {
                    yield return null;
                }
            }

            //if (firebaseMgr.Auth.IsValid)
            {
                SceneLoader.LoadWithLoading(loadData);

                yield break;
            }

            //Init(null);
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);

            SceneLoader.LoadWithLoading(loadData);
        }

        private void CreateNickName()
        {
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
        }

        private void CreateLetter()
        {
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

        void NickName.IListener.Confirm()
        {
            
        }

        void Letter.IListener.End()
        {
            _endPrologue = true;
        }
    }
}

