using System.Collections;
using System.Collections.Generic;
using Game.Creature;
using Game.Manager;
using UnityEngine;

namespace Game
{
    public class RenderTextureElement : Game.Common
    {
        #region Static
        private static RenderTextureElement _instance = null;

        public static RenderTextureElement Create(Data data)
        {
            if (data == null)
                return null;

            if (_instance == null)
            {
                _instance = GameSystem.ResourceManager.Instance?.InstantiateGame<RenderTextureElement>(null);
            }

            if (_instance != null)
            {
                _instance.transform.position = new Vector3(0, -5000f, 0);
                _instance.Initialize(data);
            }

            return _instance;
        }

        public static void Destroy()
        {
            if (_instance == null)
                return;

            _instance.Deactivate();
        }

        public static bool Validate
        {
            get { return _instance != null; }
        }
        #endregion

        public class Data
        {
            public int Id = 0;
            public int SkinId = 0;
            public Type.EElement EElement = Type.EElement.None;
        }

        #region Inspector
        [SerializeField] private Transform targetRootTm = null;
        #endregion

        private Dictionary<Data, BaseElement> _cachedElementDic = new();
        private Data _data = null;

        private void Initialize(Data data)
        {
            if (data == null)
            {
                Destroy(gameObject);

                return;
            }

            _data = data;

            CreateTarget();
            Activate();
        }

        private void CreateTarget()
        {
            if (_data == null)
                return;

            if(_cachedElementDic != null)
            {
                foreach(KeyValuePair<Data, BaseElement> pair in _cachedElementDic)
                {
                    var data = pair.Key;
                    if (data == null)
                        continue;

                    if (_data.Id != data.Id)
                        continue;

                    if (_data.SkinId != data.SkinId)
                        continue;

                    pair.Value?.Activate();

                    return;
                }
            }

            if(_data.EElement == Type.EElement.Animal)
            {
                var animal = new GameSystem.AnimalCreator()
                    .SetAnimalId(_data.Id)
                    .SetSkinId(_data.SkinId)
                    .SetRootTm(targetRootTm)
                    .SetIsEdit(false)
                    .Create();

                _cachedElementDic.TryAdd(_data, animal);
            }
            else if(_data.EElement == Type.EElement.Object)
            {
                var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                    .SetId(_data.Id)
                    .SetRootTm(targetRootTm)
                    // .SetIsEdit(false)
                    .Create();

                _cachedElementDic.TryAdd(_data, obj);
            }
        }

        public override void Deactivate()
        {
            DeactivateTarget();

            base.Deactivate();
        }

        private void DeactivateTarget()
        {
            if (!targetRootTm)
                return;

            foreach(BaseElement baseElement in _cachedElementDic.Values)
            {
                baseElement?.Deactivate();
            }
        }

        public static Creature.Animal GetAnimal(int id, int skinId)
        {
            var dic = _instance?._cachedElementDic;
            if (dic == null)
                return null;
            
            foreach(KeyValuePair<Data, BaseElement> pair in dic)
            {
                var data = pair.Key;
                if (data == null)
                    continue;

                if (data.Id != id)
                    continue;

                if (data.SkinId != skinId)
                    continue;

                return pair.Value as Creature.Animal;
            }

            return null;
        }
    }
}

