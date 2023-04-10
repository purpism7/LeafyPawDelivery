using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using UnityEditor.Experimental.GraphView;

namespace GameSystem
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Transform objectRootTm;
        [SerializeField]
        private Game.PlaceManager placeMgr;

        public Game.AnimalManager AnimalMgr { get; private set; } = null;
        public Data.Container DataContainer { get; private set; } = null;

        public Game.PlaceManager PlaceMgr { get { return placeMgr; } }
        public Transform ObjectRootTm { get { return objectRootTm; } }

        public Game.State.IState GameState { get; private set; } = new Game.State.Game();

        public override IEnumerator CoInit()
        {
            DontDestroyOnLoad(this);

            AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
            yield return StartCoroutine(AnimalMgr?.CoInit());

            yield return StartCoroutine(PlaceMgr?.CoInit());

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

        public Creature.Animal AddAnimal(Creature.Animal animal)
        {
            if(animal == null)
            {
                return null;
            }

            //var animalData = DataContainer.GetAnimal(animal.Id);
            //if(animalData == null)
            //{
            //    Debug.LogError(name + " = No Animal Data");
            //    return null;
            //}

            //if (!AnimalMgr.AddAnimal(animal))
            //{
            //    return null;
            //}

            return animal;
        }
    }
}

