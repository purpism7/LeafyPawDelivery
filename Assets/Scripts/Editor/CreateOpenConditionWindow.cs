using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Data;
using GameData;
using Unity.VisualScripting;

public class CreateOpenConditionWindow : EditorWindow
{
    private Type.EOpen _eOpenType = Type.EOpen.None;
    private string _fileName = string.Empty;
    private int _id = 0;
    private OpenConditionData _openCondition = null;
    
    [MenuItem("Animals/Create Open Condition Window")]
    private static void Open()
    {
        var window = GetWindow<CreateOpenConditionWindow>();
    }

    private void OnEnable()
    {
        if (_openCondition == null)
        {
            _openCondition = new();
        }
    }

    private void OnGUI()
    {
        _eOpenType = (Type.EOpen)EditorGUILayout.EnumPopup("Type", _eOpenType);
        _fileName = EditorGUILayout.TextField("File Name", _fileName);
        _id = EditorGUILayout.IntField("Id", _id);

        if(_openCondition != null)
        {
            EditorGUILayout.BeginVertical("Box");

            //_openCondition.Starter = EditorGUILayout.Toggle("Starter", _openCondition.Starter);
            //if (!_openCondition.Starter)
            //{
            //    _openCondition.ReqLeaf = EditorGUILayout.IntField("Req Leaf", _openCondition.ReqLeaf);
            
            //    var serializedObject = new SerializedObject(_openCondition);
            //    serializedObject.Update();
            //    var dataListProperty = serializedObject.FindProperty("Datas");
            //    EditorGUILayout.PropertyField(dataListProperty);
            //    serializedObject.ApplyModifiedProperties();
            //}
            
            EditorGUILayout.EndVertical();
        }
        
        if(GUILayout.Button("Create"))
        {
            Create();
        }
    }

    private void Create()
    {
        if (_eOpenType == Type.EOpen.None)
            return;
        
        var filePath = "Assets/ScriptableObject/OpenCondition/" + _eOpenType + "/" + _fileName + ".asset";
        if (File.Exists(filePath))
            return;

        //var openCondition = ScriptableObject.CreateInstance<GameData.OpenCondition>();

        //openCondition.Starter = _openCondition.Starter;
        //if (!openCondition.Starter)
        //{
        //    openCondition.ReqLeaf = _openCondition.ReqLeaf;
        //    openCondition.ReqDatas = _openCondition.ReqDatas;
        //}

        //AssetDatabase.CreateAsset(openCondition, filePath);
        //AssetDatabase.SaveAssets();

        // animalData.OpenCondition = _openCondition;

        // Save(openCondition);
    }

    // private void Save(Data.Animal animalData)
    // {
    //     if(animalData == null)
    //     {
    //         return;
    //     }
    //
    //     var containerPrefabPath = "Assets/Prefabs/DataContainer.prefab";
    //     var gameObj = PrefabUtility.LoadPrefabContents(containerPrefabPath);
    //     if (!gameObj)
    //         return;
    //
    //     var dataContainer = gameObj.GetComponent<Data.Container>();
    //     dataContainer?.AddAnimaData(animalData);
    //
    //     PrefabUtility.SaveAsPrefabAssetAndConnect(gameObj, containerPrefabPath, InteractionMode.AutomatedAction);
    // }
}
