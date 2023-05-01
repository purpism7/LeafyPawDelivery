using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private int _objectUId = 0;
        
        public Remove(int objectUId)
        {
            _objectUId = objectUId;
        }

        public override void Execute()
        {
            GameSystem.GameManager.Instance?.RemoveObject(_objectUId);
        }
    }
}