using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UI.Component;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

[CustomEditor(typeof(ConversationClip))]
public class ConversationClipInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var conversationClip = target as ConversationClip;
        if (conversationClip == null)
            return;

        //EditorGUILayout.BeginVertical();
        ////serializedObject.Update();
        ////var serializedObject = new SerializedObject(conversationClip);
        //serializedObject.Update();
        //var behaviourProperty = serializedObject.FindProperty("Behaviour");
        //var localDataListProperty = behaviourProperty.FindPropertyRelative("LocalDatas");
        //EditorGUILayout.PropertyField(localDataListProperty);
        //serializedObject.ApplyModifiedProperties();

        //EditorGUILayout.EndVertical();
    }
}
