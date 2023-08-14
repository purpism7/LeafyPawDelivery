using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(UI.Component.SpeechBubble))]
[TrackClipType(typeof(SpeechBubbleClip))]
public class SpeechBubbleTrack : TrackAsset
{
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        var playable = base.CreatePlayable(graph, gameObject, clip);

        return playable;
    }
}
