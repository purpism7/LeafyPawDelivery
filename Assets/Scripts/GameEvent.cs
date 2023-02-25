using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvent<T>
{
    private HashSet<Action<T>> _eventHash = new();

    public void AddListener(Action<T> action)
    {
        if(action == null)
        {
            return;
        }

        if(_eventHash.Contains(action))
        {
            return;
        }

        _eventHash.Add(action);
    }

    public void RemoveListener(Action<T> action)
    {
        if (action == null)
        {
            return;
        }

        if (!_eventHash.Contains(action))
        {
            return;
        }

        _eventHash.Remove(action);
    }

    public void Invoke(T t)
    {
        if(_eventHash == null)
        {
            return;
        }

        int count = _eventHash.Count;
        var itr = _eventHash.GetEnumerator();

        while(itr.MoveNext())
        {
            itr.Current?.Invoke(t);

            if(count != _eventHash.Count)
            {
                itr = _eventHash.GetEnumerator();
                count = _eventHash.Count;
            }
        }
    }
}
