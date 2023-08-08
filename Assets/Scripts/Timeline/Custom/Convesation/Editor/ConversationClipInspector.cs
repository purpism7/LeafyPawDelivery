using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;

[CustomEditor(typeof(ConversationClip))]
public class ConversationClipInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var conversationClip = target as ConversationClip;
        if (conversationClip == null)
            return;

        var behaviour = conversationClip.Behaviour;
        if (behaviour == null)
            return;


    }
}
