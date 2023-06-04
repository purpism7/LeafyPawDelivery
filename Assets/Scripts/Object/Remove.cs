using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private int _objectId = 0;
        private int _objectUId = 0;

        public static void Execute(int objectId, int objectUId)
        {
            new Remove(objectId, objectUId)?.Execute();
        }

        public Remove(int objectId, int objectUId)
        {
            _objectId = objectId;
            _objectUId = objectUId;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.RemoveObject(_objectId, _objectUId);
        }
    }
}