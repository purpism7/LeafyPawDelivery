using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EncryptFileWindow : EditorWindow
{
    [MenuItem("LeafyPawDelivery/Encrypt File Window")]
    private static void Open()
    {
        var window = GetWindow<EncryptFileWindow>();
    }

    private string _fileName = string.Empty;
    private string _filePath = string.Empty;

    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        _fileName = EditorGUILayout.TextField("File Name", _fileName);
        _filePath = EditorGUILayout.TextField("File Path", _filePath);

        //if (_openCondition != null)
        //{
        //    EditorGUILayout.BeginVertical("Box");

        //    //_openCondition.Starter = EditorGUILayout.Toggle("Starter", _openCondition.Starter);
        //    //if (!_openCondition.Starter)
        //    //{
        //    //    _openCondition.ReqLeaf = EditorGUILayout.IntField("Req Leaf", _openCondition.ReqLeaf);

        //    //    var serializedObject = new SerializedObject(_openCondition);
        //    //    serializedObject.Update();
        //    //    var dataListProperty = serializedObject.FindProperty("Datas");
        //    //    EditorGUILayout.PropertyField(dataListProperty);
        //    //    serializedObject.ApplyModifiedProperties();
        //    //}

        //    EditorGUILayout.EndVertical();
        //}

        if (GUILayout.Button("Encrypt"))
        {
            Encrypt();
        }
    }

    private void Encrypt()
    {
        var fullPath = Path.Combine(_filePath, _fileName);
        if (!System.IO.File.Exists(fullPath))
        {
            return;
        }

        if (fullPath.Contains(".txt"))
            return;

        var jsonStr = System.IO.File.ReadAllText(fullPath);
        var encrytStr = jsonStr.Encrypt(Games.Data.SecretKey);

        var fileName = _fileName.Replace("json", "txt");

        var resFullPath = Path.Combine(_filePath, fileName);
        Debug.Log("resFullPath = " + resFullPath);
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.WriteAllText(fullPath, encrytStr);
            System.IO.File.Move(fullPath, resFullPath);
        }
        else
        {
            System.IO.File.WriteAllText(resFullPath, encrytStr);
            System.IO.File.Create(resFullPath);
        }

        EditorApplication.update();
        Debug.Log("jsonStr = " + jsonStr);
        Debug.Log("encrytStr = " + encrytStr);
    }
}
