using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Loading : BasePopup<Loading.Data>
    {
        public class Data : BaseData
        {
            public int PlaceId = 0;
        }

        [SerializeField]
        private RectTransform[] rootRectTms = null;
        [SerializeField]
        private Transform[] targetTms = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            AllDeactiveLoading();
        }

        public override void Activate()
        {
            base.Activate();

            ActiveLoading();
            MovePlace();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            AllDeactiveLoading();
        }

        private void ActiveLoading()
        {
            if (_data == null)
                return;

            int index = _data.PlaceId - 1;
            if (rootRectTms != null &&
                rootRectTms.Length > index)
            {
                rootRectTms[index].SetActive(true);
            }

            if(targetTms != null &&
               targetTms.Length > index)
            {
                targetTms[index].SetActive(true);
            }
        }

        private void AllDeactiveLoading()
        {
            foreach(RectTransform rectTm in rootRectTms)
            {
                rectTm.SetActive(false);
            }

            foreach (Transform tm in targetTms)
            {
                tm.SetActive(false);
            }
        }

        private void MovePlace()
        {
            if (_data == null)
                return;

            MainGameManager.Instance?.MovePlace(_data.PlaceId,
              () =>
              {
                  Deactivate();

                  _endTask = true;
              });
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }
    }
}

