using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private Type.EMain _eMain = Type.EMain.None;
        private int _id = 0;
        private int _uId = 0;

        public static void Execute(Type.EMain eMain, int id, int uId = 0)
        {
            new Remove(eMain, id, uId)?.Execute();
        }

        public Remove(Type.EMain eMain, int id, int uId)
        {
            _eMain = eMain;
            _id = id;
            _uId = uId;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Remove(_eMain, _id, _uId);
        }
    }
}