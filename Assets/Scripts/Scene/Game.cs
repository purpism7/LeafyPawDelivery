using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

using GameSystem;

using Type = Game.Type;

namespace Scene
{
    public class Game : Base, Preprocessing.IListener
    {
        [SerializeField] private Preprocessing _preprocessing = null;

        private async void Start()
        {
            if(Application.isEditor)
            {
                // GameScene 에서 바로 실행 시, 동작.
                // Loading 거쳐서 들어올 경우 에는, ActiveScene 이 LoadingScene 임.
                if (Enum.TryParse(SceneManager.GetActiveScene().name.Replace("Scene", ""), out Type.EScene eSceneType))
                {
                    var auth = new Auth();
                    await auth.AsyncInitialize();

                    if (eSceneType == Type.EScene.Game)
                    {
                        Info.Setting.Get?.InitializeLocale();

                        _preprocessing?.Init(this);
                    }
                }
            }
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            _preprocessing?.Init(this);
        }

        void Preprocessing.IListener.Progress(int processIndex, float progress)
        {
            _iListener?.Progress(progress, processIndex == 1 ? 0.3f : 0.04f);
        }

        void Preprocessing.IListener.End()
        {
            _iListener?.EndLoad();

            MainGameManager.Instance?.EndLoadAsync(true).Forget();
        }
    }
}