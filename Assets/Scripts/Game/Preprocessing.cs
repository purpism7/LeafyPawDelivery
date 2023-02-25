using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameSystem
{
    [Serializable]
    public class Process : MonoBehaviour
    {

    }

    public abstract class Processing : Process
    {
        public abstract IEnumerator CoProcess(IPreprocessingProvider iProvider);
    }

    public interface IPreprocessingProvider
    {
        T Get<T>() where T : Process;
    }

    public class Preprocessing : MonoBehaviour, IPreprocessingProvider
    {
        public List<Process> ProcessList = new List<Process>();

        private Queue<Process> _processQueue = new();
        private Processing _processing = null;

        private void Awake()
        {
            SetProcessQueue();

            StartCoroutine(CoProcess());
        }

        private void SetProcessQueue()
        {
            var init = gameObject.GetOrAddComponent<Init>();
            if (init == null)
            {
                return;
            }

            _processQueue.Enqueue(init);

            if (ProcessList != null)
            {
                foreach (var process in ProcessList)
                {
                    if (process == null)
                    {
                        continue;
                    }

                    _processQueue.Enqueue(process);
                }
            }
        }

        private IEnumerator CoProcess()
        {
            if (_processQueue == null)
            {
                yield break;
            }

            if (_processQueue.TryDequeue(out Process process))
            {
                _processing = process as Processing;
                if (_processing != null)
                {
                    Debug.Log("Start Processing = " + _processing.GetType());
                    yield return StartCoroutine(_processing.CoProcess(this));
                }
            }

            if(_processQueue.Count > 0)
            {
                StartCoroutine(CoProcess());

                yield break;
            }
            else
            {
                Debug.Log("End Preprocessing");

                var openConditionMgr = Get<OpenConditionManager>();
                if(openConditionMgr != null)
                {
                    yield return null;

                    openConditionMgr.UpdaeCheckConditionList();
                }

                yield break;
            }
        }

        #region IPreprocessingProvider
        public T Get<T>() where T : Process
        {
            if (ProcessList == null)
            {
                return default(T);
            }

            foreach (var process in ProcessList)
            {
                if (process is T)
                {
                    return process as T;
                }
            }

            return default(T);
        }
        #endregion
    }
}


