using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private Type.EMain _eMain = Type.EMain.None;
        private int _id = 0;
        private Vector3 _pos = Vector3.zero;

        public static void Execute(Type.EMain eMain, int id, Vector3 pos)
        {
            new Arrange(eMain, id, pos)?.Execute();
        }

        public Arrange(Type.EMain eMain, int id, Vector3 pos)
        {
            _eMain = eMain;
            _id = id;
            _pos = pos;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Arrange(_eMain, _id, _pos);
        }
    }
}

