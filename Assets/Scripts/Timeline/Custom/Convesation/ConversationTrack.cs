using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(ConversationClip))]
public class ConversationTrack : TrackAsset
{
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        return base.CreatePlayable(graph, gameObject, clip);
    }
}
