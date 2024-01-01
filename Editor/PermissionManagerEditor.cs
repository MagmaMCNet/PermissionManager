using PermissionSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using PM = PermissionSystem.PermissionManager;
using UnityEditor.Build;
using UnityEditorInternal;
[CustomEditor(typeof(PM))]
public class PermissionManagerEditor : Editor
{
    SerializedProperty DataURL;

    GUIStyle MainHeader;
    List<GameObject> GameObjects = new List<GameObject>();
    GameObject Self;
    bool issetup = false;
    public void OnEnable()
    {

        DataURL = serializedObject.FindProperty("DataUrl");
        GameObjects = GetAllObjectsInScene();

        GameObject gameObject = GameObjects.Find(x => x.GetComponent<PM>() != null);

        gameObject.GetComponent<PM>()._Editor_Self = gameObject;
        Self = gameObject;
        if (Self.name != "PermissionSystem")
            return;
        if (Self.tag != "PermissionSystem")
            return;
        if (Self.transform.parent != null)
            return;
        issetup = true;
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
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MainHeader = new GUIStyle(EditorStyles.toolbar);
        MainHeader.alignment = TextAnchor.MiddleCenter;
        MainHeader.fixedHeight = 32f;
        MainHeader.fontSize = 20;
#if MAGMAMC_PERMISSIONMANAGER
#else
        GUIStyle Text = new GUIStyle(EditorStyles.toolbar);
        Text.alignment = TextAnchor.MiddleCenter;
        Text.fixedHeight = 32f;
        Text.fontSize = 10;
        MainHeader.fontSize = 14;
        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Define Symbol Not Found", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Please Add MAGMAMC_PERMISSIONMANAGER");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);
#endif
#if MAGMAMC_PERMISSIONMANAGER
        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("PermissionManager - V1.0.0", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.PropertyField(DataURL);

        if (!issetup)
        {
            if (GUILayout.Button("Setup"))
            {
                if (!InternalEditorUtility.tags.Contains("PermissionSystem"))
                    InternalEditorUtility.AddTag("PermissionSystem");
                GameObject gameObject = GameObjects.Find(x => x.GetComponent<PM>() != null);

                gameObject.GetComponent<PM>()._Editor_Self = gameObject;
                Self = gameObject;
                gameObject.transform.SetParent(null);
                gameObject.transform.position = new Vector3(0f, 10f, 0f);
                gameObject.name = "PermissionSystem";
                gameObject.tag = "PermissionSystem";
                gameObject.layer = 0;
                
                issetup = true;
            }
        }
#endif
        serializedObject.ApplyModifiedProperties();
    }
}
