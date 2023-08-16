using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;

[System.Serializable]
public class SpeechBubbleBehaviour : PlayableBehaviour
{
    [System.Serializable]
    public class LocalData : BaseLocalData
    {
        public float KeepDelay = 1.5f;
    }

    public LocalData[] LocalDatas = null;

    private UI.Component.SpeechBubble _speechBubble = null;

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);   
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        InitializeSpeechBubble();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);

        if(_speechBubble == null)
        {
            _speechBubble = playerData as UI.Component.SpeechBubble;

            InitializeSpeechBubble();
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
    }

    public void SetSpeechBubble(UI.Component.SpeechBubble speechBubble)
    {
        _speechBubble = speechBubble;
    }

    private void InitializeSpeechBubble()
    {
        if (_speechBubble == null)
            return;

        var locale = LocalizationSettings.SelectedLocale;
        if(locale == null)
        {
            locale = new UnityEngine.Localization.Locale()
            {
                LocaleName = "en",
            };
        }

        foreach (var localData in LocalDatas)
        {
            if (localData == null)
                continue;

            var sentence = LocalizationSettings.StringDatabase.GetLocalizedString(localData.TableName, localData.Key, locale);
            
            _speechBubble.Enqueue(new UI.Component.SpeechBubble.Constituent()
            {
                Sentence = sentence,
                KeepDelay = localData.KeepDelay,
            });
        }

        _speechBubble.Begin();
    }
}
