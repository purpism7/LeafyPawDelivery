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
        public abstract void Initialize();
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
            void Progress(int processIndex, float progress);
            void End();
        }
        
        public List<Process> ProcessList = new List<Process>();

        private Queue<Process> _processQueue = new();
        private Processing _processing = null;
        private IListener _iListener = null;

        private int _process = 0;
        private int _totalProcessCnt = 0;

        public void Init(IListener iListener)
        {
            _iListener = iListener;
            
            SetProcessQueue();

            StartCoroutine(CoProcess());
        }

        private void SetProcessQueue()
        {
            InitializeProcess(gameObject.GetOrAddComponent<Start>());

            if (ProcessList != null)
            {
                foreach (Processing processing in ProcessList)
                {
                    InitializeProcess(processing);
                }
            }

            InitializeProcess(gameObject.GetOrAddComponent<End>());

            _totalProcessCnt = _processQueue.Count;
        }

        void InitializeProcess(Processing processing)
        {
            if (processing == null)
                return;
            
            processing.Initialize();
            _processQueue.Enqueue(processing);
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
                    float progress = (float)++_process / (float)_totalProcessCnt;
             
                    _iListener?.Progress(_process, progress);

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


