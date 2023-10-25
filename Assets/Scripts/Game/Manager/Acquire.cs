using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class Acquire : Base<Acquire.Data>
    {
        public class Data : BaseData
        {

        }

        private Info.AcquireHolder _acquireHolder = new();

        

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit(Data data)
        {
            _acquireHolder?.LoadInfo();

            yield break;
        }

        public void Add(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction, int value)
        {
            _acquireHolder?.Add(eAcquire, eAcquireAction, value);
        }

        public void SetNextStep(int id)
        {
            _acquireHolder?.SetNextStep(id);
        }

        public Info.Acquire.DailyMission GetDailyMission(int id)
        {
            return _acquireHolder?.GetDailyMission(id);
        }

        public Info.Acquire.Achievement GetAchievement(int id)
        {
            return _acquireHolder?.GetAchievement(id);
        }
    }
}

 