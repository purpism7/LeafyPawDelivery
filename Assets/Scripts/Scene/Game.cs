using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class Game : Base, Preprocessing.IListener
    {
        [SerializeField] private Preprocessing _preprocessing = null;

        private void Start()
        {
            // GameScene 에서 바로 실행 시, 동작.
            // Loading 거쳐서 들어올 경우 에는, ActiveScene 이 LoadingScene 임.
            if (Enum.TryParse(SceneManager.GetActiveScene().name.Replace("Scene", ""), out Type.EScene eSceneType))
            {
                if (eSceneType == Type.EScene.Game)
                {
                    _preprocessing?.Init(this);
                }
            }
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            _preprocessing?.Init(this);
        }

        void Preprocessing.IListener.End()
        {
            _iListener?.EndLoad();
        }
    }
}