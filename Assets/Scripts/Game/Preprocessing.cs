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
        private void Awake()
        {
            Initialize();
        }

        protected abstract void Initialize();
        public abstract IEnumerator CoProcess(IPreprocessingProvider iProvider);
    }

    public interface IPreprocessingProvider
    {
        T Get<T>() where T : Process;
    }

    public class Preprocessing : MonoBehaviour, IPreprocessingProvider
    {
        public interface IListener
        {
            void End();
        }
        
        public List<Process> ProcessList = new List<Process>();

        private Queue<Process> _processQueue = new();
        private Processing _processing = null;
        private IListener _iListener = null;

        public void Init(IListener iListener)
        {
            _iListener = iListener;
            
            SetProcessQueue();

            StartCoroutine(CoProcess());
        }

        private void SetProcessQueue()
        {
            var start = gameObject.GetOrAddComponent<Start>();
            if (start == null)
                return;

            _processQueue.Enqueue(start);

            if (ProcessList != null)
            {
                foreach (var process in ProcessList)
                {
                    if (process == null)
                        continue;

                    _processQueue.Enqueue(process);
                }
            }

            var end = gameObject.GetOrAddComponent<End>();
            if(end != null)
            {
                _processQueue.Enqueue(end);
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

                _iListener?.End();

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


