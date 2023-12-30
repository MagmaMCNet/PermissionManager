using PermissionSystem;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PermissiveCollider))]
public class PermissiveColliderEditor : Editor
{

    SerializedProperty _PermissionManager;
    SerializedProperty Colliders;
    SerializedProperty Destructive;
    SerializedProperty LoopCheck;
    SerializedProperty Reverse;
    SerializedProperty AuthorizedPermissions;

    GUIStyle MainHeader;

    public void OnEnable()
    {
        _PermissionManager = serializedObject.FindProperty("PermissionManager");
        Colliders = serializedObject.FindProperty("Colliders");
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
        GUILayout.Label("PermissiveCollider", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(15);
        EditorGUILayout.PropertyField(_PermissionManager);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(Colliders);
        EditorGUILayout.PropertyField(Destructive);
        if (!Destructive.boolValue)
            EditorGUILayout.PropertyField(LoopCheck);

        EditorGUILayout.PropertyField(Reverse);
        EditorGUILayout.PropertyField(AuthorizedPermissions, true);

        serializedObject.ApplyModifiedProperties();

    }
}
