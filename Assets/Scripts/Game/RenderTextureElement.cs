using System.Collections;
using System.Collections.Generic;
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
            public Type.EElement EElement = Type.EElement.None;
        }

        #region Inspector
        [SerializeField] private Transform targetRootTm = null;
        #endregion

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
        }

        private void CreateTarget()
        {
            if(_data.EElement == Type.EElement.Animal)
            {
                var animal = new GameSystem.AnimalCreator()
                    .SetAnimalId(_data.Id)
                    .SetRootTm(targetRootTm)
                    .SetIsEdit(false)
                    .Create();
            }
            else if(_data.EElement == Type.EElement.Object)
            {
                var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                    .SetId(_data.Id)
                    .SetRootTm(targetRootTm)
                    .SetIsEdit(false)
                    .Create();
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

            foreach (Transform childTm in targetRootTm)
            {
                if (!childTm)
                    continue;

                childTm.gameObject.SetActive(false);
            }
        }
    }
}

