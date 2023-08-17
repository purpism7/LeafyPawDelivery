using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using Game.Creature;
using Data;
using GameData;
using GameSystem;

public class MainGameManager : Singleton<MainGameManager>
{
    #region Inspector
    public Game.PlaceManager placeMgr = null;
    #endregion

    public Game.ObjectManager ObjectMgr { get; private set; } = null;
    public Game.AnimalManager AnimalMgr { get; private set; } = null;
    public Game.StoryManager StoryMgr { get; private set; } = null; 
    
    public Transform ObjectRootTm { get { return placeMgr?.ActivityPlace?.ObjectRootTm; } }

    public Camera GameCamera { get; private set; } = null;

    public Game.State.Base GameState { get; private set; } = null;

    private System.Action<Game.BaseElement> _startEditAction = null;
    
    protected override void Initialize()
    {
        ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();
        AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
    }

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        var activityPlaceId = Game.Data.Const.StartPlaceId;//placeMgr.ActivityPlaceId; 

        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInit(
                new Game.PlaceManager.Data()
                {
                    StartPlaceId = activityPlaceId,
                }));
        }

        yield return StartCoroutine(ObjectMgr?.CoInit(new Game.ObjectManager.Data
        {
            PlaceId = activityPlaceId,
        }));
        
        yield return StartCoroutine(AnimalMgr?.CoInit(new Game.AnimalManager.Data
        {
            PlaceId = activityPlaceId,
        }));

        placeMgr?.ActivityPlace?.Activate();

        StoryMgr = iProvider.Get<Game.StoryManager>();
        //OpenCondition = iProvider.Get<Game.Manager.OpenCondition>();

        GameCamera = iProvider.Get<InputManager>()?.GameCameraCtr?.GameCamera;

        SetGameState<Game.State.Game>();

        LocalizationSettings.SelectedLocale = UnityEngine.Localization.Locale.CreateLocale("ko");

        yield return new WaitForEndOfFrame();
    }

    public void Starter()
    {
        Debug.Log("Starter");
        AnimalMgr?.Check();
    }

    private void Update()
    {
        placeMgr?.ChainUpdate();
    }

    #region GameState
    public void SetGameState<T>() where T : Game.State.Base
    {
        if (GameState != null &&
            GameState.Type.Equals(typeof(T)))
            return;

        GameState = System.Activator.CreateInstance<T>();
        GameState?.Initialize(this);
    }
    #endregion

    public void SetStartEditAction(System.Action<Game.BaseElement> action)
    {
        _startEditAction = action;
    }

    public void AddInfo(Type.EElement EElement, int id)
    {
        if (EElement == Type.EElement.Animal)
        {
            AnimalMgr?.AddAnimal(id);
        }
        else if (EElement == Type.EElement.Object)
        {
            ObjectMgr?.AddObject(id);
        }
    }

    public bool CheckExist(Type.EElement EElement, int id)
    {
        if (EElement == Type.EElement.Animal)
        {
            return AnimalMgr.CheckExist(id);
        }
        else if (EElement == Type.EElement.Object)
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

    public void Remove(Type.EElement EElement, int id, int uId)
    {
        switch (EElement)
        {
            case Type.EElement.Animal:
                {
                    placeMgr?.ActivityPlace?.RemoveAnimal(id);
                    AnimalMgr?.RemoveAnimal(id);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Type.EElement.Object:
                {
                    placeMgr?.ActivityPlace?.RemoveObject(uId);
                    ObjectMgr?.RemoveObject(id, uId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }

    public void Arrange(Type.EElement EElement, int id, Vector3 pos)
    {
        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        switch(EElement)
        {
            case Type.EElement.Animal:
                {
                    AnimalMgr?.ArrangeAnimal(id, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Type.EElement.Object:
                {

                    ObjectMgr?.ArrangeObject(id, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }
}   

