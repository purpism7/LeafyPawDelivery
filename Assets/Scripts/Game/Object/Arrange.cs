using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private Type.EElement _eElement = Type.EElement.None;
        private int _id = 0;
        private Vector3 _pos = Vector3.zero;

        public static void Execute(Type.EElement eElement, int id, Vector3 pos)
        {
            new Arrange(eElement, id, pos)?.Execute();
        }

        public Arrange(Type.EElement eElement, int id, Vector3 pos)
        {
            _eElement = eElement;
            _id = id;
            _pos = pos;
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Arrange(_EElement, _id, _pos);
        }
    }
}

