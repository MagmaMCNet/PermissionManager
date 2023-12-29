using PermissionSystem;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PermissiveObject))]
public class PermissiveObjectEditor : Editor
{
    SerializedProperty _PermissionManager;
    SerializedProperty gameobject;
    SerializedProperty Destructive;
    SerializedProperty LoopCheck;
    SerializedProperty Reverse;
    SerializedProperty AuthorizedPermissions;

    GUIStyle MainHeader;

    public void OnEnable()
    {
        _PermissionManager = serializedObject.FindProperty("PermissionManager");
        gameobject = serializedObject.FindProperty("gameobject");
        Destructive = serializedObject.FindProperty("Destructive");
        LoopCheck = serializedObject.FindProperty("LoopCheck");
        Reverse = serializedObject.FindProperty("Reverse");
        AuthorizedPermissions = serializedObject.FindProperty("AuthorizedPermissions");   
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MainHeader = new GUIStyle(EditorStyles.toolbar);
        MainHeader.alignment = TextAnchor.MiddleCenter;
        MainHeader.fixedHeight = 32f;
        MainHeader.fontSize = 20;

        if (_PermissionManager.objectReferenceValue == null)
        {
            EditorGUILayout.PropertyField(_PermissionManager);
            serializedObject.ApplyModifiedProperties();
            return;
        }


        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("PermissiveObject", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);
        EditorGUILayout.PropertyField(_PermissionManager);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(gameobject);
        EditorGUILayout.PropertyField(Destructive);
        if (!Destructive.boolValue)
            EditorGUILayout.PropertyField(LoopCheck);

        EditorGUILayout.PropertyField(Reverse);
        EditorGUILayout.PropertyField(AuthorizedPermissions, true);

        serializedObject.ApplyModifiedProperties();

    }
}
