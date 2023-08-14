using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;

[System.Serializable]
public class ConversationBehaviour : PlayableBehaviour
{
    [System.Serializable]
    public class LocalData
    {
        public string Table = string.Empty;
        public string Key = string.Empty;
    }

    public LocalData[] LocalDatas = null;

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);

        var conversation = Game.Manager.Cutscene.Instance?.Conversation;
        if (conversation != null)
        {
            conversation?.Clear();
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        var conversation = Game.Manager.Cutscene.Instance?.Conversation;
        if (conversation == null)
            return;

        if (LocalDatas == null ||
            LocalDatas.Length <= 0)
            return;

        foreach(var localData in LocalDatas)
        {
            if (localData == null)
                continue;

            conversation.Enqueue(new UI.Conversation.Constituent()
            {
                Sentence = LocalizationSettings.StringDatabase.GetLocalizedString(localData.Table, localData.Key, LocalizationSettings.SelectedLocale),
            });
        }

        conversation.Activate();
        conversation.StartTyping();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);

        Debug.Log("ConversationBehaviour OnBehaviourPause");
        Game.Manager.Cutscene.Instance?.Conversation?.Deactivate();
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);

        Debug.Log("ConversationBehaviour OnGraphStop");
    }
}
