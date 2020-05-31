//using UnityEngine;
//using UnityEditor;
//using UnityEngine.UIElements;

//[CustomEditor(typeof(StoryScript))]
//[CanEditMultipleObjects]
//public class StoryScriptEditor : Editor
//{
//    StoryScript storyScript;

//    private void OnEnable()
//    {
//        storyScript = (StoryScript)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        GUILayoutOption[] layoutOptions = { GUILayout.ExpandWidth(true) };

//        for(int i = 0; i < storyScript.storyBlockCount; i++)
//        {
//            GUIStyle style = new GUIStyle();
//            style.normal.textColor = Color.white;
//            style.fontStyle = FontStyle.Bold;
            
//            GUILayoutOption[] titleLayoutOptions = {  };
//            GUILayoutOption[] bodyLayoutOptions = { GUILayout.MinHeight(40), GUILayout.MaxHeight(120), GUILayout.ExpandHeight(true) };

//            //GUILayout.BeginVertical();

//            GUILayout.Label("Story Block: " + (i + 1), style, layoutOptions);

//            GUILayout.Space(3);

//            GUILayout.Label("Title");
//            storyScript.storyBlocksTitles[i] = GUILayout.TextArea(storyScript.storyBlocksTitles[i], 30, titleLayoutOptions);

//            GUILayout.Space(2);

//            GUILayout.Label("Body");
//            storyScript.storyBlockBodies[i] = GUILayout.TextArea(storyScript.storyBlockBodies[i], 200, bodyLayoutOptions);

//            if (GUILayout.Button("Delete Story Block"))
//            {
//                storyScript.DeleteStoryBlock(i);
//            }

//            GUILayout.Space(15);
//        }

//        if (GUILayout.Button("Add Story Block"))
//        {
//            storyScript.AddStoryBlock();
//        }
//    }
//}