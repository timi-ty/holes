using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StoryScript : ScriptableObject
{
    public List<string> storyBlocksTitles = new List<string>();
    public List<string> storyBlockBodies = new List<string>();
    public int storyBlockCount => storyBlockBodies.Count;

    public string[] GetStoryBlock(int i)
    {
        string[] block = { storyBlocksTitles[i], storyBlockBodies[i] };
        return block;
    }

    public void AddStoryBlock()
    {
        storyBlocksTitles.Add("");
        storyBlockBodies.Add("");
    }

    public void DeleteStoryBlock(int i)
    {
        storyBlocksTitles.RemoveAt(i);
        storyBlockBodies.RemoveAt(i);
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Story")]
    public static void CreateObstacleData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Story", "New Story", "Asset", "Save Story", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<StoryScript>(), path);
    }
#endif
}