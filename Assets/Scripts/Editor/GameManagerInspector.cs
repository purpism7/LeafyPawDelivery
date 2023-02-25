using UnityEngine;
using System.Collections;
using UnityEditor;

using GameSystem;

[CustomEditor(typeof(GameManager))]
public class GameManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gameMgr = target as GameManager;
        if(gameMgr == null)
        {
            return;
        }

        //gameMgr.EditorFoldout = EditorGUILayout.Foldout(gameMgr.EditorFoldout, "Editor");
        //if(gameMgr.EditorFoldout)
        //{
        //    EditorGUILayout.BeginHorizontal("");
        //    gameMgr.CreateActivityAnimalId = EditorGUILayout.IntField(gameMgr.CreateActivityAnimalId);
        //    if (GUILayout.Button("Create Activity Animal"))
        //    {
        //        gameMgr.ActivityAnimalManager.CreateActivityAnimal(gameMgr.CreateActivityAnimalId);
        //    }
        //    EditorGUILayout.EndHorizontal();
        //} 
    }
}

