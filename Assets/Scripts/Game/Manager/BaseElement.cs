using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class BaseElement : Base
    {
        
        
       
    }

    public abstract class BaseElement<T> : BaseElement
    {
        private void Awake()
        {
            Initialize();
        }

        public virtual void ChainUpdate()
        {

        }

        protected bool CheckIsAll(List<OpenConditionData> openConditionDataList)
        {
            if (openConditionDataList == null)
                return false;

            foreach (var data in openConditionDataList)
            {
                if (data == null)
                    continue;

                if (!CheckExist(data.Id))
                    return false;
            }

            return true;
        }

        protected abstract void Initialize();
        public abstract IEnumerator CoInitialize(T data);

        public abstract void Add(int id);
        public abstract void Remove(int id, int uId);
        public abstract bool CheckExist(int id);
    }
}
