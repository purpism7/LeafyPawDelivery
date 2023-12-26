using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

using GameSystem;
using Info;

namespace Game
{
    public class StoryManager : Manager.Base<StoryManager.Data>
    {
        public class Data : Manager.BaseData
        {
            
        }

        private List<GameObject> _storyPrefabList = new();
        private int _placeId = 0;

        public static UnityEvent<Event.StoryData> Listener = new();

        protected override void Initialize()
        {
            _storyPrefabList.Clear();
            
            AnimalManager.Event?.AddListener(OnChangedAnimalInfo);
            ObjectManager.Event?.AddListener(OnChangedObjectInfo);
            Game.PlaceManager.Event?.AddListener(OnChangedPlace);
        }

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(CoLoadStory());
        }

        private IEnumerator CoLoadStory()
        {
            var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
            if(addressableAssetLoader == null)
                yield break;

            bool endLoad = false;

            yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<GameObject>(
                addressableAssetLoader.AssetLabelStory,
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if(result == null)
                        return;

                    _storyPrefabList.Add(result);

                    endLoad = true;
                }));
        
            yield return new WaitUntil(() => endLoad);
        }

        private bool Check(out Story currStory)
        {
            currStory = null;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;

            var storyList = StoryContainer.Instance.GetStoryList(_placeId);
            if (storyList == null)
                return false;

            var user = UserManager.Instance?.User;
            if (user == null)
                return false;

            int lastStoryId = user.GetLastStoryId(_placeId);
            
            foreach (var story in storyList)
            {
                if (story == null)
                    continue;

                if (story.Order <= lastStoryId)
                    continue;

                currStory = story;

                break;
            }

            if (currStory == null)
                return false;

            var storyOpenCondition =  StoryOpenConditionContainer.Instance.GetData(currStory.Id);
            if(storyOpenCondition == null)
                return false;

            if (!CheckReqIds(mainGameMgr, Type.EElement.Animal, storyOpenCondition.ReqAnimalIds))
                return false;

            if (!CheckReqIds(mainGameMgr, Type.EElement.Object, storyOpenCondition.ReqObjectIds))
                return false;

            return true;
        }

        private bool CheckReqIds(MainGameManager mainGameMgr, Type.EElement eElement, int[] ids)
        {
            if (ids != null)
            {
                foreach (int id in ids)
                {
                    if (id <= 0)
                        continue;

                    if (!mainGameMgr.CheckExist(eElement, id))
                        return false;
                }
            }

            return true;
        }

        public Game.Manager.Cutscene PlayStory(Story story)
        {
            if (story == null)
                return null;

            return Game.Manager.Cutscene.Create(new Game.Manager.Cutscene.Data()
            {
                TargetGameObj = GetStoryGameObj(story.PrefabName),
                EndAction = () =>
                {
                    EndStory(story);
                },
            });
        }

        private void StartStory(Story story)
        {
            if (story == null)
                return;

            Sequencer.EnqueueTask(
                () =>
                {
                    var cutscene = PlayStory(story);

                    Listener?.Invoke(new Event.StoryData()
                    {
                        Id = story.Id,
                        EState = Event.EState.Begin,
                    });

                    MainGameManager.Instance?.AddAcquire(Type.EAcquire.Story, Type.EAcquireAction.Obtain, 1);

                    return cutscene;
                });

            UserManager.Instance?.SaveStory(story.Id);
        }

        private void EndStory(Story story)
        {
            if (story == null)
                return;
            
            Listener?.Invoke(new Event.StoryData()
            {
                Id = story.Id,
                EState = Event.EState.Begin,
            });

            //storyData.Completed = true;
        }

        //public bool CheckCompleted(int storyId)
        //{
        //    //if (_storyDic.TryGetValue(_placeId, out GameData.Story story))
        //    //{
        //        //if (story == null)
        //        //    return false;

        //        //if (story.Datas == null)
        //        //    return false;

        //        //int index = storyId - 1;
        //        //if (story.Datas.Length <= index ||
        //        //    0 < index)
        //        //    return false;

        //        //var storyData = story.Datas[index];
        //        //if (storyData == null)
        //        //    return false;
                
        //        //return storyData.Completed;
        //    //}

        //    return false;
        //}

        private GameObject GetStoryGameObj(string prefabName)
        {
            if (_storyPrefabList == null)
                return null;

            foreach(GameObject storyGameObj in _storyPrefabList)
            {
                if (!storyGameObj)
                    continue;

                if (!storyGameObj.name.Equals(prefabName))
                    continue;

                return storyGameObj;
            }

            return null;
        }

        #region Listener
        private void OnChangedAnimalInfo(Game.Event.AnimalData animalData)
        {
            if (Check(out Story story))
            {
                StartStory(story);
            }
        }

        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {
            if (Check(out Story story))
            {
                StartStory(story);
            }
        }

        private void OnChangedPlace(int placeId)
        {
            _placeId = placeId;

            //SetStoryList();
        }
        #endregion
    }
}

