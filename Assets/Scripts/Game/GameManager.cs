using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;

namespace GameSystem
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Transform objectRootTm;

        public Game.AnimalManager AnimalMgr { get; private set; } = null;
        public Data.Container DataContainer { get; private set; } = null;

        public Transform ObjectRootTm { get { return objectRootTm; } }

        public Game.State.IState GameState { get; private set; } = new Game.State.Game();

        public override IEnumerator CoInit()
        {
            DontDestroyOnLoad(this);

            AnimalMgr = new();

            DataContainer = FindObjectOfType<Data.Container>();

            yield return null;
        }

        private void Update()
        {
            AnimalMgr?.ChainUpdate();
        }

        #region GameState
        public void SetGameState<T>() where T : Game.State.Base
        {
            if (GameState.Type.Equals(typeof(T)))
            {
                return;
            }

            GameState = System.Activator.CreateInstance<T>();
        }
        #endregion

        public Animal AddAnimal(Animal animal)
        {
            if(animal == null)
            {
                return null;
            }

            var animalData = DataContainer.GetAnimal(animal.Id);
            if(animalData == null)
            {
                Debug.LogError(name + " = No Animal Data");
                return null;
            }

            if (!AnimalMgr.AddAnimal(animal))
            {
                return null;
            }

            return animal;
        }
    }
}

