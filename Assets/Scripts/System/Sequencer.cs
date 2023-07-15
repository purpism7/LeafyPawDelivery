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

    public static void EnqueueTask(ITask iTask)
    {
        if (Sequencer_ == null)
            return;

        if (Sequencer_._taskQueue == null)
        {
            Sequencer_._taskQueue = new();
        }

        
        Sequencer_._taskQueue?.Enqueue(iTask);
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

    Queue<ITask> _taskQueue = new();

    private void Progress()
    {
        StartCoroutine(CoProgressTask());
    }

    IEnumerator CoProgressTask()
    {
        if (_taskQueue.Count <= 0)
            yield break;

        var iTask = _taskQueue.Dequeue();
        if (iTask == null)
            yield break;

        Debug.Log("Sequence Begin Task = " + iTask.GetType().FullName);
        iTask.Begin();

        yield return new WaitUntil(() => iTask.End);

        Debug.Log("Sequence End Task");

        Progress();
    }
}
