using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private int _objectUId = 0;

        public static void Execute(int objectUId)
        {
            new Remove(objectUId)?.Execute();
        }

        public Remove(int objectUId)
        {
            _objectUId = objectUId;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.RemoveObject(_objectUId);
        }
    }
}