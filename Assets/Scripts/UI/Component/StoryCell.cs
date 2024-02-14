using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

namespace UI.Component
{
    public class StoryCell : BaseComponent<StoryCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Story Story = null;
            public int PlaceId = 0;
        }

        public interface IListener
        {
            void Select(Story story);
        }

        [SerializeField]
        private TextMeshProUGUI storyNameTMP = null;
        [SerializeField]
        private Button btn = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            //SetStoryName();
        }

        public override void Activate()
        {
            base.Activate();

            SetLock();
            SetStoryName();
        }

        private void SetStoryName()
        {
            if (_data == null)
                return;

            var key = _data.Story.PlaceId + "_" + _data.Story.Id + "_name";
            var localStoryName = LocalizationSettings.StringDatabase.GetLocalizedString("Story", key);

            storyNameTMP?.SetText(localStoryName);
        }

        private void SetLock()
        {
            bool isLock = true;
            var user = Info.UserManager.Instance.User;
            int lastStoryId = 0;
            if (user != null)
            {
                lastStoryId  = user.GetLastStoryId(GameUtils.ActivityPlaceId);
            }

            if(_data.Story.Id <= lastStoryId)
            {
                isLock = false;
            }

            btn.interactable = !isLock;
        }

        public void OnClick()
        {
            if (_data == null)
                return;

            _data.IListener?.Select(_data.Story);
        }
    }
}

