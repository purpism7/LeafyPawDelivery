using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;

[CreateAssetMenu(menuName = "Game/ScriptableObject/Const")]
public class Const : ScriptableObject
{
    //[System.Serializable]
    //public class PlaceData
    //{
    //    public Game.Type.EPlaceName ePlaceName = Game.Type.EPlaceName.None;

    //    public Game.Type.EAnimalCurrency Animal = Game.Type.EAnimalCurrency.None;
    //    public Game.Type.EObjectCurrency Object = Game.Type.EObjectCurrency.None;
    //    public Info.User.Currency StartValue = null;
    //    public float animalCurrencyRate = 1f;
    //    public float objectCurrencyRate = 1f;

    //    public string AnimalSpriteName { get { return Animal.ToString().ToLower(); } }
    //    public string ObjectSpriteName { get { return Object.ToString().ToLower(); } }

    //    public int PlaceId
    //    {
    //        get
    //        {
    //            return StartValue != null ? StartValue.PlaceId : 0;
    //        }
    //    }
    //}

    [SerializeField]
    private string worldTimeUri = string.Empty;

    [SerializeField]
    private int animalBaseSkinId = 1;
    [SerializeField]
    private int startPlaceId = 1;
    [SerializeField]
    private int tutorialAnimalId = 1;
    [SerializeField]
    private int tutorialObjectId = 1;

    [SerializeField]
    private int maxDropAnimalCurrencyCount = 30;
    [SerializeField]
    private int maxDropLetterCount = 20;

    [SerializeField]
    private int dailyMissionRewardCount = 5;
    [SerializeField]
    private int achievementRewardCount = 5;

    public string WorldTimeURI { get { return worldTimeUri; } }

    public int AnimalBaseSkinId { get { return animalBaseSkinId; } }
    public int StartPlaceId { get { return startPlaceId; } }
    public int TutorialAnimalId { get { return tutorialAnimalId; } }
    public int TutorialObjectId { get { return tutorialObjectId; } }

    public int MaxDropAnimalCurrencyCount { get { return maxDropAnimalCurrencyCount; } }
    public int MaxDropLetterCount { get { return maxDropLetterCount; } }

    public int DailyMissionRewardCount { get { return dailyMissionRewardCount; } }
    public int AchievementRewardCount { get { return achievementRewardCount; } }
}
