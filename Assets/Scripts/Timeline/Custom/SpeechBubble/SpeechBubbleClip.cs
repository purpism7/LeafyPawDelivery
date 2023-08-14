using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SpeechBubbleClip : PlayableAsset, ITimelineClipAsset
{
    public SpeechBubbleBehaviour Behaviour = new();

    public ClipCaps clipCaps
    {
        get
        {
            return ClipCaps.All;
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SpeechBubbleBehaviour>.Create(graph, Behaviour);

        Behaviour.Clone();

        return playable;
    }
}
