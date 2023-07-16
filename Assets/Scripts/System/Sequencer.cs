using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : Game.Common
{
    #region Static

    private static Sequencer Sequencer_ = null;
    public static Sequencer Create()
    {
        if (Sequencer_ == null)
        {
            Sequencer_ = GameSystem.ResourceManager.Instance.InstantiateGame<Sequencer>(null);
        }

        if(Sequencer_ != null)
        {
            DontDestroyOnLoad(Sequencer_);
        }

        return Sequencer_;
    }

    public static bool Validate
    {
        get { return Sequencer_ != null; }
    }

    public static void EnqueueTask(System.Func<ITask> func)
    {
        if (Sequencer_ == null)
            return;

        if (Sequencer_._taskFuncQueue == null)
        {
            Sequencer_._taskFuncQueue = new();
        }

        Sequencer_._taskFuncQueue?.Enqueue(func);
        Sequencer_.Progress();
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

        Progress,
    }

    Queue<System.Func<ITask>> _taskFuncQueue = new();

    private void Progress()
    {
        StartCoroutine(CoProgressTask());
    }

    IEnumerator CoProgressTask()
    {
        if (_taskFuncQueue.Count <= 0)
            yield break;

        var taskFunc = _taskFuncQueue.Dequeue();
        if (taskFunc == null)
        {
            Progress();

            yield break;
        }
            

        var iTask = taskFunc();
        if(iTask == null)
        {
            Progress();

            yield break;
        }

        Debug.Log("Sequence Begin Task = " + iTask.GetType().FullName);
        iTask.Begin();

        yield return new WaitUntil(() => iTask.End);

        Debug.Log("Sequence End Task");

        Progress();
    }
}
