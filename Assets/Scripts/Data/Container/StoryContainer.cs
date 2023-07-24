using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryContainer : BaseContainer<StoryContainer, Story>
{
    private Dictionary<int, List<Story>> _storyListDic = null;

    protected override void InternalInitialize()
    {
        base.InternalInitialize();

        if (_storyListDic == null)
        {
            _storyListDic = new Dictionary<int, List<Story>>();
            _storyListDic.Clear();
        }

        foreach(var storyData in _datas)
        {
            if (storyData == null)
                continue;

            List<Story> storyList = null;
            if (_storyListDic.TryGetValue(storyData.PlaceId, out storyList))
            {
                if (storyList == null)
                    return;

                bool existStory = false;
                foreach(var story in storyList)
                {
                    if (story == null)
                        continue;

                    if(story.Id == storyData.Id)
                    {
                        existStory = true;

                        break;
                    }
                }

                if (!existStory)
                {
                    storyList.Add(storyData);
                }
            }
            else
            {
                storyList = new List<Story>();
                storyList.Clear();
                storyList.Add(storyData);

                _storyListDic.TryAdd(storyData.PlaceId, storyList);
            }
        }
    }

    public List<Story> GetStoryList(int placeId)
    {
        if (_storyListDic.TryGetValue(placeId, out List<Story> storyList))
        {
            return storyList;
        }

        return null;
    }
}
