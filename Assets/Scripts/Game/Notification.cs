using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Notification : Statics<Notification>
    {
        public enum EType
        {
            None,

            OpenPlace,

            AddAnimal,
            AddObject,
            AddStory,

            CompleteDailyMission,
            CompleteAchievement,
        }

        public interface IListener
        {
            void Notify();
        }

        private Dictionary<EType, List<IListener>> _iListenerDic = new();

        private void Awake()
        {
            _iListenerDic?.Clear();
        }

        public void AddListener(EType eType, IListener iListener)
        {
            List<IListener> iListenerList = null;
            if (_iListenerDic.TryGetValue(eType, out iListenerList))
            {
                if (iListenerList == null)
                    return;

                if (!iListenerList.Contains(iListener))
                {
                    iListenerList.Add(iListener);
                }
            }
            else
            {
                iListenerList = new();
                iListenerList?.Clear();

                iListenerList?.Add(iListener);

                _iListenerDic.TryAdd(eType, iListenerList);
            }
        }

        public void Notify(EType eType)
        {
            if (_iListenerDic == null)
                return;

            if (_iListenerDic.TryGetValue(eType, out List<IListener> iListenerList))
            {
                if (iListenerList == null)
                    return;

                foreach(var iListener in iListenerList)
                {
                    iListener?.Notify();
                }
            }
        }

        public void AllNotify()
        {
            foreach(EType eType in System.Enum.GetValues(typeof(Notification.EType)))
            {
                Notify(eType);
            }
        }
    }
}

