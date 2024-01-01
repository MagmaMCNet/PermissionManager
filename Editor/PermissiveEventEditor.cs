using MagmaMc.Utils;
using PermissionSystem;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PermissiveEvents))]
public class PermissiveEventsEditor : Editor
{
    SerializedProperty _PermissionManager;

    SerializedProperty AuthorizedReceivers;
    SerializedProperty AuthorizedEvents;
    SerializedProperty DeniedReceivers;
    SerializedProperty DeniedEvents;

    SerializedProperty LoopCheck;

    SerializedProperty Networked;
    SerializedProperty Targets;

    SerializedProperty AuthorizedPermissions;

    GUIStyle MainHeader;

    public void OnEnable()
    {
        _PermissionManager = serializedObject.FindProperty("PermissionManager");

        AuthorizedReceivers = serializedObject.FindProperty("AuthorizedReceivers");
        AuthorizedEvents = serializedObject.FindProperty("AuthorizedEvents");

        DeniedReceivers = serializedObject.FindProperty("DeniedReceivers");
        DeniedEvents = serializedObject.FindProperty("DeniedEvents");

        LoopCheck = serializedObject.FindProperty("LoopCheck");

        Networked = serializedObject.FindProperty("Networked");
        Targets = serializedObject.FindProperty("Targets");

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
        GUILayout.Label("PermissiveEvents", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(_PermissionManager);

        bool a = false;
        EditorUtilities.DrawEventReceiverArray(AuthorizedReceivers, AuthorizedEvents, "Authorized", ref a);

        EditorUtilities.DrawEventReceiverArray(DeniedReceivers, DeniedEvents, "Denied", ref a);

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(LoopCheck);
        EditorGUILayout.PropertyField(Networked);
        if (Networked.boolValue)
            EditorGUILayout.PropertyField(Targets);

        EditorGUILayout.PropertyField(AuthorizedPermissions, true);

        serializedObject.ApplyModifiedProperties();

    }
}
