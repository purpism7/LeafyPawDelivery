using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class Cell : MonoBehaviour
    {
        public enum EState
        {
            None,


        }

        public class Data
        {
            public IListener IListener = null;
            public int Row = 0;
            public int Column = 0;
            public float CellSize = 0;
        }

        public interface IListener
        {
            void Overlap(List<GameObject> list);
        }

        [SerializeField]
        private BoxCollider boxCollider = null;

        private List<GameObject> _overlapList = new List<GameObject>();
        private Vector3 _halfExtnents = Vector3.one * 0.5f;

        public Data Data_ { get; private set; } = null;

        public void Init(Data data)
        {
            if (data == null)
                return;

            Data_ = data;

            name = "[" + data.Row + ", " + data.Column + "]"; 

            SetColliderSize();
            SetLocalPosition();
        }

        private void SetColliderSize()
        {
            if(boxCollider == null)
                return;

            boxCollider.size = new Vector3(Data_.CellSize, Data_.CellSize, 0);

            _halfExtnents = boxCollider.size * 0.5f;
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(Data_.Row, Data_.Column, 0) * Data_.CellSize;
        }

        private bool IsOverlap
        {
            get
            {
                if (boxCollider == null)
                    return false;

                _overlapList.Clear();

                var colliders = Physics.OverlapBox(boxCollider.center + transform.position, _halfExtnents, Quaternion.identity);
                //Debug.Log("colliders = "  + colliders.Length);
                foreach (var collider in colliders)
                {
                    if (collider == null)
                        continue;

                    // cell 들은 제외.
                    var cell = collider.GetComponentInParent<Cell>();
                    if (cell != null)
                        continue;

                    var animal = collider.GetComponentInParent<Game.Creature.Animal>();
                    if (animal != null)
                        continue;

                    //_overlapList.Add(collider.gameObject);
                    return true;
                }

                return false;
            }
           

            //Data_?.IListener?.Overlap(_overlapList);
        }

        public bool CheckOverlap
        {
            get
            {
                return IsOverlap;
            }
        }

        public void ChainUpdate()
        {
            //Overlap();
        }
    }
}

