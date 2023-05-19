using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Data;
using GameData;

public class CreateAnimalDataWindow : EditorWindow
{
    private string _fileName = string.Empty;
    private int _animalId = 0;
    //private Data.Animal.EGrade _eGrade = Data.Animal.EGrade.Normal;

    private GameData.OpenCondition _openCondition = new();

    [MenuItem("Animals/Create Animal Data Window")]
    static void Open()
    {
        var window = GetWindow<CreateAnimalDataWindow>();
    }

    private void OnGUI()
    {
        _fileName = EditorGUILayout.TextField("File Name", _fileName);
        _animalId = EditorGUILayout.IntField("Animal Id", _animalId);
        //_eGrade = (Data.Animal.EGrade)EditorGUILayout.EnumPopup("Animal Grade", _eGrade);

        if(_openCondition != null)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Open Condition");
            _openCondition.ReqLv = EditorGUILayout.IntField("Require Lv", _openCondition.ReqLv);
            _openCondition.ReqLeaf = EditorGUILayout.LongField("Require Leaf", _openCondition.ReqLeaf);
            _openCondition.ReqBerry = EditorGUILayout.LongField("Require Berry", _openCondition.ReqBerry);
            EditorGUILayout.EndVertical();
        }

        if(GUILayout.Button("Create"))
        {
            Create();
        }
    }

    private void Create()
    {
        var filePath = "Assets/Data/Animal/" + _fileName + ".asset";
        if (File.Exists(filePath))
        {
            return;
        }

        //var animalData = ScriptableObject.CreateInstance<Data.Animal>();

        //AssetDatabase.CreateAsset(animalData, filePath);
        //AssetDatabase.SaveAssets();

        //animalData.name = _fileName;
        //animalData.Id = _animalId;
        //animalData.OpenCondition = _openCondition;

        //Save(animalData);
    }

    //private void Save(Data.Animal animalData)
    //{
    //    if(animalData == null)
    //    {
    //        return;
    //    }

    //    var containerPrefabPath = "Assets/Prefabs/DataContainer.prefab";
    //    var gameObj = PrefabUtility.LoadPrefabContents(containerPrefabPath);
    //    if (!gameObj)
    //    {
    //        return;
    //    }

    //    var dataContainer = gameObj.GetComponent<Data.Container>();
    //    dataContainer?.AddAnimaData(animalData);

    //    PrefabUtility.SaveAsPrefabAssetAndConnect(gameObj, containerPrefabPath, InteractionMode.AutomatedAction);
    //}
}
