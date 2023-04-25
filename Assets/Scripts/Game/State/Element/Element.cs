using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public abstract class Element
    {
        public abstract bool Check { get; }
    }
}


//public class Remove : GameElementState
//{
//    public override bool Check
//    {
//        get
//        {
//            return true;
//        }
//    }
//}

//public class Arrange : GameElementState
//{
//    public override bool Check
//    {
//        get
//        {
//            return true;
//        }
//    }
//}