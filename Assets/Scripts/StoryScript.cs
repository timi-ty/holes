using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StoryScript : ScriptableObject
{
    public List<string> storyBlocks = new List<string>();
    
    public string GetStoryBlock(int i)
    {
        return storyBlocks[i];
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Story")]
    public static void CreateObstacleData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Story", "New Story", "Asset", "Save Story", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Background>(), path);
    }
#endif
}