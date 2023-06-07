using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using Data;
using GameData;
using GameSystem;
using Animal = Info.Animal;

public class MainGameManager : Singleton<MainGameManager>
{
    public Game.PlaceManager placeMgr = null;

    public Game.ObjectManager ObjectMgr { get; private set; } = null;
    public Game.AnimalManager AnimalMgr { get; private set; } = null;

    public GameSystem.OpenConditionManager OpenConditionManager { get; private set; } = null;

    public Transform ObjectRootTm { get { return placeMgr?.ActivityPlace?.ObjectRootTm; } }

    public Game.State.Base GameState { get; private set; } = new Game.State.Game();

    private System.Action<Game.Base> _startEditAction = null;

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();
        yield return StartCoroutine(ObjectMgr?.CoInit(new Game.ObjectManager.Data
        {
            PlaceId = 1,
        }));

        AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
        yield return StartCoroutine(AnimalMgr?.CoInit(null));

        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInit(null));
        }

        OpenConditionManager = iProvider.Get<OpenConditionManager>();
        Debug.Log(OpenConditionManager?.CheckOpenCondition);
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

    public void SetStartEditAction(System.Action<Game.Base> action)
    {
        _startEditAction = action;
    }

    #region Object
    public void AddObjectToPlace(Game.Object obj)
    {
        if (obj == null)
            return;

        placeMgr?.ActivityPlace?.AddObject(obj);

        _startEditAction?.Invoke(obj);
    }

    public void RemoveObject(int objectId, int objectUId)
    {
        placeMgr?.RemoveObject(objectUId);
        ObjectMgr?.RemoveObject(objectId, objectUId);

        Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();
    }

    public void ArrangeObject(int objectUId, Vector3 pos)
    {
        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        ObjectMgr?.ArrangeObject(objectUId, pos, placeId);

        Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();
    }
    #endregion
}   

