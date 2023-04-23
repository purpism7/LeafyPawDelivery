using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private int _objectUId = 0;
        private Vector3 _pos = Vector3.zero;

        public Arrange(int objectUId, Vector3 pos)
        {
            _objectUId = objectUId;
            _pos = pos;
        }

        public override void Execute()
        {
            GameSystem.GameManager.Instance.ArrangeObject(_objectUId, _pos);
        }
    }
}

