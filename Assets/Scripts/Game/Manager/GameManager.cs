using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.PlayerSettings;

namespace GameSystem
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Game.PlaceManager placeMgr;

        public Data.Container DataContainer { get; private set; } = null;

        public Transform ObjectRootTm { get { return placeMgr?.ActivityPlace?.ObjectRootTm; } }

        public Game.State.Base GameState { get; private set; } = new Game.State.Game();

        private Game.AnimalManager _animalMgr = null;
        public Game.ObjectManager ObjectMgr { get; private set; } = null;

        public override IEnumerator CoInit()
        {
            DontDestroyOnLoad(this);

            _animalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
            yield return StartCoroutine(_animalMgr?.CoInit(null));

            ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();

            yield return StartCoroutine(ObjectMgr?.CoInit(null));

            yield return StartCoroutine(placeMgr?.CoInit(null));

            DataContainer = FindObjectOfType<Data.Container>();

            yield break;
        }

        private void Update()
        {
            _animalMgr?.ChainUpdate();
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

        #region Object
        public void RemoveObject(int objectUId)
        {
            placeMgr?.RemoveObject(objectUId);
            ObjectMgr?.RemoveObject(objectUId);
        }

        public void ArrangeObject(int objectUId, Vector3 pos)
        {
            int placeId = 0;
            if (placeMgr != null)
            {
                placeId = placeMgr.ActivityPlace.Id;
            }

            ObjectMgr?.ArrangeObject(objectUId, pos, placeId);
        }
        #endregion
    }
}

