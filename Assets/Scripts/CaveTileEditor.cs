//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(CaveTile))]
//[CanEditMultipleObjects]
//public class CaveTileEditor : Editor
//{
//    SerializedProperty m_Sprites;
//    SerializedProperty m_Preview;
//    SerializedProperty enableCeilingMods;
//    SerializedProperty leftCornerMod, midMod, rightCornerMod;
//    SerializedProperty leftTrigger, rightTrigger;

//    private void OnEnable()
//    {
//        m_Sprites = serializedObject.FindProperty("m_Sprites");
//        m_Preview = serializedObject.FindProperty("m_Preview");
//        enableCeilingMods = serializedObject.FindProperty("enableCeilingMods");
//        leftCornerMod = serializedObject.FindProperty("leftCornerMod");
//        midMod = serializedObject.FindProperty("midMod");
//        rightCornerMod = serializedObject.FindProperty("rightCornerMod");
//        leftTrigger = serializedObject.FindProperty("leftTrigger");
//        rightTrigger = serializedObject.FindProperty("rightTrigger");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(m_Sprites);
//        EditorGUILayout.PropertyField(m_Preview);
//        EditorGUILayout.PropertyField(enableCeilingMods);

//        if (enableCeilingMods.boolValue)
//        {
//            EditorGUILayout.PropertyField(leftCornerMod);
//            EditorGUILayout.PropertyField(midMod);
//            EditorGUILayout.PropertyField(rightCornerMod);
//            EditorGUILayout.PropertyField(leftTrigger);
//            EditorGUILayout.PropertyField(rightTrigger);
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
