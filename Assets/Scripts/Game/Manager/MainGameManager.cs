using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.Creature;
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

    public Camera GameCamera { get; private set; } = null;

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

        GameCamera = iProvider.Get<InputManager>()?.GameCameraCtr?.GameCamera;
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

    public void SetStartEditAction(System.Action<Game.Base> action)
    {
        _startEditAction = action;
    }

    public void AddInfo(Type.EMain eMain, int id)
    {
        if (eMain == Type.EMain.Animal)
        {
            AnimalMgr?.AddAnimal(id);
        }
        else if (eMain == Type.EMain.Object)
        {
            ObjectMgr?.AddObject(id);
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

    #region Animal
    public void AddAnimalToPlace(Game.Creature.Animal animal)
    {
        if (animal == null)
            return;

        placeMgr?.ActivityPlace?.AddAnimal(animal);

        _startEditAction?.Invoke(animal);
    }
    #endregion

    #region Object
    public void AddObjectToPlace(Game.Object obj)
    {
        if (obj == null)
            return;

        placeMgr?.ActivityPlace?.AddObject(obj);

        _startEditAction?.Invoke(obj);
    }
    #endregion

    public void Remove(Type.EMain eMain, int id, int uId)
    {
        switch (eMain)
        {
            case Type.EMain.Animal:
                {
                    placeMgr?.ActivityPlace?.RemoveAnimal(id);
                    AnimalMgr?.RemoveAnimal(id);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Type.EMain.Object:
                {
                    placeMgr?.ActivityPlace?.RemoveObject(uId);
                    ObjectMgr?.RemoveObject(id, uId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }

    public void Arrange(Type.EMain eMain, int id, Vector3 pos)
    {
        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        switch(eMain)
        {
            case Type.EMain.Animal:
                {
                    AnimalMgr?.ArrangeAnimal(id, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Type.EMain.Object:
                {

                    ObjectMgr?.ArrangeObject(id, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }
}   

