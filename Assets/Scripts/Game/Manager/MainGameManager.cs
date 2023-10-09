using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using Game.Creature;
using Data;
using GameData;
using GameSystem;
using Info;

public class MainGameManager : Singleton<MainGameManager>
{
    #region Inspector
    public Game.PlaceManager placeMgr = null;
    #endregion

    public Game.ObjectManager ObjectMgr { get; private set; } = null;
    public Game.AnimalManager AnimalMgr { get; private set; } = null;
    public Game.StoryManager StoryMgr { get; private set; } = null;
    public Game.Manager.Acquire AcquireMgr { get; private set; } = null;

    public GameCameraController GameCameraCtr { get; private set; } = null;

    public Game.State.Base GameState { get; private set; } = null;

    private System.Action<Game.Base, bool> _startEditAction = null;
    private IUpdater _iUpdateInputMgr = null;
    private IUpdater _iUpdateGameCameraCtr = null;
    
    
    protected override void Initialize()
    {
        ObjectMgr = gameObject.GetOrAddComponent<Game.ObjectManager>();
        AnimalMgr = gameObject.GetOrAddComponent<Game.AnimalManager>();
        AcquireMgr = gameObject.GetOrAddComponent<Game.Manager.Acquire>();
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

        var inputMgr = iProvider.Get<InputManager>();
        GameCameraCtr = inputMgr?.GameCameraCtr;

        _iUpdateInputMgr = inputMgr;
        _iUpdateGameCameraCtr = GameCameraCtr;

        SetGameState<Game.State.Game>();       

        yield return new WaitForEndOfFrame();
    }

    public void Starter()
    {
        Debug.Log("Starter");
        AnimalMgr?.Check();
    }

    private void Update()
    {
        _iUpdateInputMgr?.ChainUpdate();
        _iUpdateGameCameraCtr?.ChainUpdate();

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

    public void SetStartEditAction(System.Action<Game.Base, bool> action)
    {
        _startEditAction = action;
    }

    public void AddInfo(Game.Type.EElement EElement, int id)
    {
        if (EElement == Game.Type.EElement.Animal)
        {
            AnimalMgr?.AddAnimal(id);
        }
        else if (EElement == Game.Type.EElement.Object)
        {
            ObjectMgr?.AddObject(id);
        }
    }

    public bool CheckExist(Game.Type.EElement EElement, int id)
    {
        if (EElement == Game.Type.EElement.Animal)
        {
            return AnimalMgr.CheckExist(id);
        }
        else if (EElement == Game.Type.EElement.Object)
        {
            return ObjectMgr.CheckExist(id);
        }

        return false;
    }

    #region Animal
    public void AddAnimalToPlace(int id)
    {
        var animalInfo = AnimalMgr?.GetAnimalInfo(id);
        if (animalInfo == null)
            return;

        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        var animal = activityPlace.AddAnimal(id, animalInfo.SkinId, pos);
        if (animal == null)
            return;

        _startEditAction?.Invoke(animal, GameState.CheckState<Game.State.Edit>());
    }

    public void ChangeAnimalSkinToPlace(int id, int skinId)
    {
        if (AnimalMgr == null)
            return;

        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        int currSkinId = AnimalMgr.GetCurrenctSkinId(id);

        if(activityPlace.ChangeAnimalSkin(id, skinId, pos, currSkinId))
        {
            AnimalMgr?.ApplySkin(id, skinId);
        }
    }
    #endregion

    #region Object
    public void AddObjectToPlace(int id)
    {
        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        var editObject = ObjectMgr?.GetAddEditObject(id);

        var obj = activityPlace.AddObject(id, pos, editObject.UId);
        if (obj == null)
            return;

        _startEditAction?.Invoke(obj, GameState.CheckState<Game.State.Edit>());
    }
    #endregion

    public void Remove(Game.Type.EElement EElement, int id, int uId)
    {
        switch (EElement)
        {
            case Game.Type.EElement.Animal:
                {
                    placeMgr?.ActivityPlace?.RemoveAnimal(id);
                    AnimalMgr?.RemoveAnimal(id);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Game.Type.EElement.Object:
                {
                    placeMgr?.ActivityPlace?.RemoveObject(id, uId);
                    ObjectMgr?.RemoveObject(id, uId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }

    public void Arrange(Game.BaseElement gameBaseElement, Vector3 pos)
    {
        if (gameBaseElement == null ||
            gameBaseElement.ElementData == null)
            return;

        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        switch(gameBaseElement.ElementData.EElement)
        {
            case Game.Type.EElement.Animal:
                {
                    AnimalMgr?.ArrangeAnimal(gameBaseElement.Id, pos);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Game.Type.EElement.Object:
                {

                    ObjectMgr?.ArrangeObject(gameBaseElement.Id, gameBaseElement.UId, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList();

                    break;
                }
        }
    }
}   

