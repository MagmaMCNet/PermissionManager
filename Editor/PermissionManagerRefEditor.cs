using PermissionSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PermissionManagerRef))]
public class PermissionManagerRefEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle MainHeader = new GUIStyle(EditorStyles.toolbar);
        MainHeader.alignment = TextAnchor.MiddleCenter;
        MainHeader.fixedHeight = 24f;
        MainHeader.fontSize = 14;
        MainHeader.normal.textColor = Color.red;


        GUILayout.BeginHorizontal(MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("This Script Should Not Be Used By Its Self", MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
}