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
            //void Overlap(List<GameObject> list);
        }

        [SerializeField]
        private BoxCollider boxCollider = null;

        //private List<GameObject> _overlapList = new List<GameObject>();
        private Vector3 _halfExtnents = Vector3.one * 0.5f;

        public Data Data_ { get; private set; } = null;
        public bool IsOverlap { get; private set; } = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = IsOverlap ? new Color(1, 0, 0, 1f) : new Color(0, 0, 1, 1f);
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }

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

            boxCollider.size = new Vector3(Data_.CellSize, Data_.CellSize, 10f);

            _halfExtnents = boxCollider.size * 0.5f;
        }

        private void SetLocalPosition()
        {
            gameObject.transform.localPosition = new Vector3(Data_.Row, Data_.Column, 0) * Data_.CellSize;
        }

        public void SetOverlap()
        {
            if (boxCollider == null)
                return;

            IsOverlap = false;

            var colliders = Physics.OverlapBox(boxCollider.center + transform.position, _halfExtnents, Quaternion.identity);

            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

                // cell 들은 제외.
                var cell = collider.GetComponentInParent<Cell>();
                if (cell != null)
                    continue;

                var gameBaseElement = collider.GetComponentInParent<Game.BaseElement>();
                if (gameBaseElement != null)
                {
                    if (!gameBaseElement.IsOverlap)
                        continue;
                }

                IsOverlap = true;

                break;
            }
        }

        public void ChainUpdate()
        {
            //Overlap();
        }
    }
}

