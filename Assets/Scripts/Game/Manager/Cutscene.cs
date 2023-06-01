using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class Cutscene : Game.Base
    {
        private static Cutscene Cutscene_ = null;
        
        public static Cutscene Create()
        {
            Cutscene_ = GameSystem.ResourceManager.Instance.InstantiateGame<Cutscene>(null);
            Cutscene_?.Initialize();
            
            return Cutscene_;
        }

        public static bool Validate
        {
            get
            {
                return Cutscene_ != null;
            }
        }
        
        private void Initialize()
        {
            Debug.Log("Cutscene Initialize");
        }
    }
}

