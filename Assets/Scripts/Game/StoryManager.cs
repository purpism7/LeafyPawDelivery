﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

using GameSystem;
using Info;

namespace Game
{
    public class StoryManager : GameSystem.Processing
    {
        public enum EState
        {
            None,
            
            Begin,
            End,
        }

        public class Data
        {
            public int Id = 0;
            public EState EState = EState.None;
        }

        private List<GameObject> _storyPrefabList = new();
        private List<Story> _storyList = new();
        private int _placeId = 0;

        public UnityEvent<Data> Listener = new();

        public override void Initialize()
        {
            _storyPrefabList.Clear();
            
            var mainGameMgr = MainGameManager.Instance;
            mainGameMgr?.AnimalMgr?.Listener.AddListener(OnChangedAnimalInfo);
            mainGameMgr?.placeMgr?.Listener?.AddListener(OnChangedPlace);
        }

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
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

        private void SetStoryList()
        {
            if(_storyList == null)
            {
                _storyList = new();
            }

            _storyList.Clear();
            _storyList = StoryContainer.Instance.GetStoryList(_placeId);
            if(_storyList != null)
            {
                _storyList.OrderByDescending(story => story.Id);
            }
        }
        
        private bool Check(out Story currStory)
        {
            currStory = null;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return false;

            if (_storyList == null)
                return false;

            int lastStoryId = UserManager.Instance.GetLastStoryId(_placeId);
            
            foreach (var story in _storyList)
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

        private void StartStory(Story story)
        {
            if (story == null)
                return;
                        
            Sequencer.EnqueueTask(
                () =>
                {
                    var cutscene = Game.Manager.Cutscene.Create(new Game.Manager.Cutscene.Data()
                    {
                        TargetGameObj = GetStoryGameObj(story.PrefabName),
                        EndAction = () =>
                        {
                            EndStory(story);
                        },
                    });

                    Listener?.Invoke(new Data()
                    {
                        Id = story.Id,
                        EState = EState.Begin,
                    });

                    return cutscene;
                });
        }

        private void EndStory(Story story)
        {
            if (story == null)
                return;
            
            Listener?.Invoke(new Data()
            {
                Id = story.Id,
                EState = EState.End,
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
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            Debug.Log("Start Story Animal = " + animalInfo.Id);
            //Check();
            if (Check(out Story story))
            {
                StartStory(story);
            }
        }
        
        private void OnChangedObjectInfo(Info.Object objectInfo)
        {
            if(Check(out Story story))
            {
                StartStory(story);
            }
        }
        
        private void OnChangedPlace(int placeId)
        {
            _placeId = placeId;

            SetStoryList();
        }
        #endregion
    }
}

