using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Game.Manager;

public class Sequencer : Game.Common
{
    #region Static
    private static Sequencer _instance = null;
    public static Sequencer Create()
    {
        if (_instance == null)
        {
            var gameObj = Instantiate(Resources.Load("Sequencer")) as GameObject;
            if (!gameObj)
                return null;

            //gameObj.transform.SetParent(MainGameManager.Instance?.transform);

            _instance = gameObj.GetOrAddComponent<Sequencer>();
        }

        if(_instance != null)
        {
            DontDestroyOnLoad(_instance);
        }

        return _instance;
    }

    public static bool Validate
    {
        get { return _instance != null; }
    }

    public static void EnqueueTask(System.Func<ITask> func)
    {
        if (_instance == null)
        {
            Create();
        }

        _instance.Enqueue(func);
    }
    #endregion

    public interface ITask
    {
        void Begin();
        bool End { get; }
    }

    public enum ETaskState
    {
        None,

        Begin,
        Progress,
        End,
    }

    private Queue<System.Func<ITask>> _taskFuncQueue = new();
    private ETaskState _eTaskState = ETaskState.None;

    private void Enqueue(System.Func<ITask> func)
    {
        if (_taskFuncQueue == null)
        {
            _taskFuncQueue = new();
        }

        _taskFuncQueue.Enqueue(func);

        if (_eTaskState != ETaskState.None)
            return;
        
        Progress();
    }

    private void Progress()
    {
        _eTaskState = ETaskState.Progress;

        //StartCoroutine(CoProgressTask());

        AsyncProgressTask().Forget();
    }

    private async UniTask AsyncProgressTask()
    {
        if (_taskFuncQueue.Count <= 0)
        {
            _eTaskState = ETaskState.None;

            return;
        }

        var taskFunc = _taskFuncQueue.Dequeue();
        if (taskFunc == null)
        {
            Progress();

            return;
        }

        var iTask = taskFunc();
        if (iTask == null)
        {
            Progress();

            return;
        }

        _eTaskState = ETaskState.Begin;
        
        iTask.Begin();

        await UniTask.WaitUntil(() => iTask.End);

        _eTaskState = ETaskState.End;

        //await UniTask.WaitForSeconds(0.1f);

        Progress();
    }

    //private IEnumerator CoProgressTask()
    //{
    //    if (_taskFuncQueue.Count <= 0)
    //    {
    //        _eTaskState = ETaskState.None;

    //        yield break;
    //    }

    //    var taskFunc = _taskFuncQueue.Dequeue();
    //    if (taskFunc == null)
    //    {
    //        Progress();

    //        yield break;
    //    }
            

    //    var iTask = taskFunc();
    //    if(iTask == null)
    //    {
    //        Progress();

    //        yield break;
    //    }

    //    _eTaskState = ETaskState.Begin;
    //    Debug.Log("Sequence Begin Task = " + iTask.GetType().FullName);
    //    iTask.Begin();

    //    yield return new WaitUntil(() => iTask.End);

    //    _eTaskState = ETaskState.End;
    //    Debug.Log("Sequence End Task");

    //    Progress();
    //}
}
