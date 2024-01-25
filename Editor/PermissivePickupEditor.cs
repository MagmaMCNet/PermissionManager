using PermissionSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PermissivePickup))]
public class PermissivePickupEditor : Editor
{
    SerializedProperty _PermissionManager;
    SerializedProperty Items;
    SerializedProperty Destructive;
    SerializedProperty LoopCheck;
    SerializedProperty Reverse;
    SerializedProperty AuthorizedPermissions;

    GUIStyle MainHeader;

    public void OnEnable()
    {
        _PermissionManager = serializedObject.FindProperty("PermissionManager");
        Items = serializedObject.FindProperty("Items");
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

        EditorGUILayout.PropertyField(Items);
        EditorGUILayout.PropertyField(Destructive);
        EditorGUILayout.PropertyField(LoopCheck);

        EditorGUILayout.PropertyField(Reverse);
        EditorGUILayout.PropertyField(AuthorizedPermissions, true);

        serializedObject.ApplyModifiedProperties();

    }
}