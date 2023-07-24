using System.Collections;
using System.Collections.Generic;
using GameSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace  GameSystem.Load
{
    public class LoadData : Base
    {
        [SerializeField] private Type.EScene loadSceneType = Type.EScene.None;
        [SerializeField] private bool activeLoading = true;

        private void Start()
        {
            MainCamera = GetComponentInChildren<Camera>();
        }

        public override string SceneName
        {
            get
            {
                return loadSceneType.ToString();
            }
        }
        
        public override bool ActiveLoading
        {
            get
            {
                return activeLoading;
            }
        }
    }
}

