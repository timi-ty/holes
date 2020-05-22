using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public StoryScript storyScript;
    public TextMeshPro storyBoard;

    public void NextStoryBlock(int i)
    {
        storyBoard.SetText(storyScript.GetStoryBlock(i));
    }
}
