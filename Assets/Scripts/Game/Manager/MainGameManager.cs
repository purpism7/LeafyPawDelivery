using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;
using Data;
using GameData;
using GameSystem;

public class MainGameManager : Singleton<MainGameManager>
{
    public Game.PlaceManager placeMgr = null;

    public Game.ObjectManager ObjectMgr { get; private set; } = null;
    public Game.AnimalManager AnimalMgr { get; private set; } = null;

    public Game.Manager.Story Story { get; private set; } = null;
    public Game.Manager.OpenCondition OpenCondition { get; private set; } = null;
    
    public Transform ObjectRootTm { get { return placeMgr?.ActivityPlace?.ObjectRootTm; } }

    public Game.State.Base GameState { get; private set; } = new Game.State.Game();

    private System.Action<Game.Base> _startEditAction = null;
    
    protected override void Initialize()
    {
        ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();
        AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
    }

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        var activityPlaceId = Game.Data.Const.StartPlaceId;//placeMgr.ActivityPlaceId; 
        
        yield return StartCoroutine(ObjectMgr?.CoInit(new Game.ObjectManager.Data
        {
            PlaceId = activityPlaceId,
        }));
        
        yield return StartCoroutine(AnimalMgr?.CoInit(new Game.AnimalManager.Data
        {
            PlaceId = activityPlaceId,
        }));

        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInit(null));
        }
        
        Story = iProvider.Get<Game.Manager.Story>();
        OpenCondition = iProvider.Get<Game.Manager.OpenCondition>();
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

    public void AddInfo(Type.EMain eMain, int id)
    {
        if (eMain == Type.EMain.Animal)
        {
            AnimalMgr?.AddAnimalInfo(id);
        }
        else if (eMain == Type.EMain.Object)
        {
            ObjectMgr?.AddObjectInfo(id);
        }
    }

    public bool CheckExist(Type.EMain eMain, int id)
    {
        if (eMain == Type.EMain.Animal)
        {
            return AnimalMgr.CheckExist(id);
        }
        else if (eMain == Type.EMain.Object)
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
}   

