using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public interface IPoolable
    {
        GameObject PrefabKey { get; }

        Transform Transform { get; }
        bool IsActivate { get; }
    }

    public class ObjectPooler : Singleton<ObjectPooler>
    {
        private List<IPoolable> _iPoolableList = new();


        protected override void Initialize()
        {
            
        }

        public void Add(IPoolable iPoolable)
        {
            if (iPoolable == null)
                return;

            _iPoolableList?.Add(iPoolable);
        }

        public T Get<T>(GameObject prefab = null)
        {
            if (_iPoolableList == null ||
                _iPoolableList.Count <= 0)
                return default;

            for (int i = 0; i < _iPoolableList.Count; ++i)
            {
                var iPoolable = _iPoolableList[i];
                if (iPoolable == null)
                    continue;

                if (iPoolable == null ||
                    (iPoolable is UnityEngine.Object obj && obj == null))
                {
                    _iPoolableList.RemoveAt(i);
                    --i;
                    continue;
                }

                if (iPoolable.Transform &&
                    iPoolable.Transform.gameObject.activeSelf)
                    continue;

                if (prefab)
                {
                    if (iPoolable.PrefabKey != prefab)
                        continue;
                }

                if (iPoolable is T t)
                {
                    iPoolable.Transform.gameObject.SetActive(true);
                    return t;
                }
            }

            return default;
        }

        public void Activate(IPoolable iPoolable)
        {
            if (iPoolable == null ||
                !iPoolable.Transform)
                return;

            if (iPoolable.Transform.gameObject.activeSelf)
                return;

            iPoolable.Transform.gameObject.SetActive(true);
        }

        public void Return(IPoolable iPoolable)
        {
            if (iPoolable == null)
                return;

            iPoolable.Transform.SetActive(false);
        }
    }
}

