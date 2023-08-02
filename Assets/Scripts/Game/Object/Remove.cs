using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private Type.EElement _eElement = Type.EElement.None;
        private int _id = 0;
        private int _uId = 0;

        public static void Execute(Type.EElement eElement, int id, int uId = 0)
        {
            new Remove(eElement, id, uId)?.Execute();
        }

        public Remove(Type.EElement eElement, int id, int uId)
        {
            _eElement = eElement;
            _id = id;
            _uId = uId;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Remove(_eElement, _id, _uId);
        }
    }
}