using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ElementCollision : MonoBehaviour
    {
        private int _id = 0;
        private int _uId = 0;
        private Game.Type.EElement _element = Type.EElement.None;
        private bool _isOverlapTarget = false;
        private bool _goPass = true;

        public bool IsOverlap { get; private set; } = false;

        private List<(int, int)> _overlapList = new(); 

        public void Initialize(Game.BaseElement gameBaseElement)
        {
            if (gameBaseElement == null)
                return;

            _id = gameBaseElement.Id;
            _uId = gameBaseElement.UId;

            var elementData = gameBaseElement.ElementData;
            if(elementData != null)
            {
                _element = elementData.EElement;
            }

            if (_element == Type.EElement.Object)
            {
                var obj = gameBaseElement as Game.Object;
                _isOverlapTarget = obj.IsOverlap;
                _goPass = obj.GoPass;
            }

            _overlapList?.Clear();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_id <= 0)
                return;

            if(CheckIsOverlap(collision, out int id, out int uId))
            {
                int index = GetIndex(id, uId);
                if (index >= 0)
                    return;

                _overlapList?.Add((id, uId));
            }

            IsOverlap = _overlapList?.Count > 0;            
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_id <= 0)
                return;

            if (CheckIsOverlap(collision, out int id, out int uId))
            {
                int index = GetIndex(id, uId);
                if (index < 0)
                    return;

                _overlapList?.RemoveAt(index);
            }

            IsOverlap = _overlapList?.Count > 0;
        }

        //private void OnCollisionStay2D(Collision2D collision)
        //{
        //    if (_id <= 0)
        //        return;

        //    //Debug.Log("OnCollisionStay2D = " + _id);
        //    //IsOverlap = CheckIsOverlap(collision);
        //}

        private int GetIndex(int id, int uId)
        {
            for (int i = 0; i < _overlapList.Count; ++i)
            {
                var items = _overlapList[i];

                if (items.Item1 != id)
                    continue;

                if (items.Item2 != uId)
                    continue;

                return i;
            }

            return -1;
        }

        private bool CheckIsOverlap(Collision2D collision, out int id, out int uId)
        {
            id = 0;
            uId = 0;

            var gameObj = collision?.gameObject;
            if (!gameObj)
                return false;

            var obj = gameObj.GetComponentInParent<Game.Object>();
            if (obj != null)
            {
                if (obj.Id != _id ||
                    obj.UId != _uId)
                {
                    if (_isOverlapTarget)
                    {
                        id = obj.Id;
                        uId = obj.UId;

                        return true;
                    }
                    else
                    {
                        if (obj.IsOverlap ||
                           (_element == Type.EElement.Animal && !obj.GoPass) ||
                           (!_goPass && !obj.GoPass))
                        {
                            id = obj.Id;
                            uId = obj.UId;

                            return true;
                        }
                    }
                }
            }

            var animal = gameObj.GetComponentInParent<Game.Creature.Animal>();
            if (animal != null)
            {
                if (_isOverlapTarget)
                    return true;

                if (_element == Game.Type.EElement.Animal)
                {
                    if (animal.Id != _id)
                    {
                        id = animal.Id;

                        return true;
                    }
                }
            }

            return false;
        }

        public void Reset()
        {
            _overlapList?.Clear();
        }
    }
}

