using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private int _objectUId = 0;
        private Vector3 _pos = Vector3.zero;

        public static void Execute(int objectUId, Vector3 pos)
        {
            new Arrange(objectUId, pos)?.Execute();
        }

        public Arrange(int objectUId, Vector3 pos)
        {
            _objectUId = objectUId;
            _pos = pos;
        }

        public override void Execute()
        {
            MainGameManager.Instance.ArrangeObject(_objectUId, _pos);
        }
    }
}

