using PermissionSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using PM = PermissionSystem.PermissionManager;
[CustomEditor(typeof(PM))]
public class PermissionManagerEditor : Editor
{
    SerializedProperty DataURL;

    GUIStyle MainHeader;

    public void OnEnable()
    {
        DataURL = serializedObject.FindProperty("DataUrl");   
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MainHeader = new GUIStyle(EditorStyles.toolbar);
        MainHeader.alignment = TextAnchor.MiddleCenter;
        MainHeader.fixedHeight = 32f;
        MainHeader.fontSize = 20;

        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("PermissionManager - V0.1.0", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.PropertyField(DataURL);

        if (GUILayout.Button("Setup"))
        {
            List<GameObject> objectsInScene = GetAllObjectsInScene();
            GameObject gameObject = objectsInScene.Find(x => x.GetComponent<PM>() != null);


            gameObject.transform.SetParent(null);
            gameObject.transform.position = new Vector3(0f, 10f, 0f);
            gameObject.name = "PermissionSystem";
            gameObject.layer = 2;

        }
        serializedObject.ApplyModifiedProperties();
    }
    static List<GameObject> GetAllObjectsInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var gameObject in go.GetComponentsInChildren<Transform>(true))
            {
                objectsInScene.Add(gameObject.gameObject);
            }
        }

        return objectsInScene;
    }
}
