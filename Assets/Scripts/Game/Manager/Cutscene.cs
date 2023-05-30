using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class Cutscene : MonoBehaviour
    {
        public static Cutscene Create()
        {
            return GameSystem.ResourceManager.Instance.InstantiateGame<Cutscene>(null);
        }
        private void Awake()
        {
            
        }
    }
}

