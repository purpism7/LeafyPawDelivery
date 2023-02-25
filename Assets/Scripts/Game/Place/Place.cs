using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class Place : Base, ActivityArea.IListener
    {
        public Transform ActivityAreaRootTm;

        private Dictionary<int, ActivityArea> _activityAreaDic = new();
        private System.Action<int> _placeActivityAnimalAction = null;
        private int _selectedAnimalId = 0;

        public override void Init(params object[] objs)
        {
            if(objs != null &&
               objs.Length > 0)
            {
                _placeActivityAnimalAction = objs[0] as System.Action<int>;
            }

            InitActivityAreaDic();
        }

        public override void ChainUpdate()
        {
            return;
        }

        private void InitActivityAreaDic()
        {
            _activityAreaDic.Clear();

            var activityAreas = GetComponentsInChildren<ActivityArea>();
            if (activityAreas == null)
            {
                return;
            }

            foreach (var activityArea in activityAreas)
            {
                if (activityArea == null)
                {
                    continue;
                }

                activityArea.Init(this);

                _activityAreaDic.TryAdd(activityArea.Id, activityArea);
            }
        }

        public void EnableActivityArea(int animalId)
        {
            _selectedAnimalId = animalId;

            EnalbeAllActivityArea(true);
        }

        private void EnalbeAllActivityArea(bool enable)
        {
            if (_activityAreaDic == null)
            {
                return;
            }

            foreach (var activityArea in _activityAreaDic.Values)
            {
                if (activityArea == null)
                {
                    continue;
                }

                if(enable &&
                   activityArea.PlayingAnimal)
                {
                    continue;
                }

                activityArea.Enable(enable);
            }
        }

        #region ActivityArea.IListener
        void ActivityArea.IListener.PlaceAnimal(ActivityArea activityArea)
        {
            if(activityArea == null)
            {
                return;
            }

            if(activityArea.PlaceAnimal(_selectedAnimalId))
            {
                EnalbeAllActivityArea(false);

                _placeActivityAnimalAction?.Invoke(_selectedAnimalId);

                _selectedAnimalId = 0;
            }
        }
        #endregion
    }
}

