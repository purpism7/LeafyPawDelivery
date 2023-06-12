using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using Data;
using GameData;
using GameSystem;

public class MainGameManager : Singleton<MainGameManager>
{
    public interface ICondition
    {
        bool Check(int placeId);
    }
    
    public Game.PlaceManager placeMgr = null;

    public Game.ObjectManager ObjectMgr { get; private set; } = null;
    public Game.AnimalManager AnimalMgr { get; private set; } = null;

    // public Game.Manager.Story StoryMgr { get; private set; } = null;
    // public GameSystem.OpenConditionManager OpenConditionMgr { get; private set; } = null;

    public Transform ObjectRootTm { get { return placeMgr?.ActivityPlace?.ObjectRootTm; } }

    public Game.State.Base GameState { get; private set; } = new Game.State.Game();

    private System.Action<Game.Base> _startEditAction = null;

    private ICondition _story = null;
    private ICondition _openCondition = null;

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();
        yield return StartCoroutine(ObjectMgr?.CoInit(new Game.ObjectManager.Data
        {
            PlaceId = 1,
        }));

        AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
        yield return StartCoroutine(AnimalMgr?.CoInit(new Game.AnimalManager.Data
        {
            PlaceId = 1,
        }));

        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInit(null));
        }

        _story = iProvider.Get<Game.Manager.Story>();
        _openCondition = iProvider.Get<Game.Manager.OpenCondition>();
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

    public void AddInfo(Type.EOpen eOpenType, int id)
    {
        if (eOpenType == Type.EOpen.Animal)
        {
            AnimalMgr?.AddAnimalInfo(id);
        }
        else if (eOpenType == Type.EOpen.Object)
        {
            ObjectMgr?.AddObjectInfo(id);
        }

        _story?.Check(placeMgr.ActivityPlace.Id);
    }

    public bool CheckExist(Type.EOpen eOpenType, int id)
    {
        if (eOpenType == Type.EOpen.Animal)
        {
            return AnimalMgr.CheckExist(id);
        }
        else if (eOpenType == Type.EOpen.Object)
        {
            return ObjectMgr.CheckExist(id);
        }

        return false;
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

    public void CheckOpenCondition()
    {
        _openCondition?.Check(placeMgr.ActivityPlace.Id);
    }
}   

