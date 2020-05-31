//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;

//[CustomEditor(typeof(Background))]
//[CanEditMultipleObjects]
//public class BackgroudEditor : Editor
//{
//    Background background;

//    private void OnEnable()
//    {
//        background = (Background)target;
//    }
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        for (int i = background.layerSpeedRatio.Count; i < background.layers.Count; i++)
//        {
//            background.layerSpeedRatio.Add(i + 1);
//        }
//    }
//}