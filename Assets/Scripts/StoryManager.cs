using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public StoryScript storyScript;
    public TextMeshProUGUI storyTitleTMP;
    public TextMeshProUGUI storyBodyTMP;
    public Image tracksLogo;
    public TransitionWall storyWall;

    public void ShowStoryBlock(int playerLevel)
    {
        storyTitleTMP.SetText(storyScript.GetStoryBlock(playerLevel - 2)[0]);
        storyBodyTMP.SetText(storyScript.GetStoryBlock(playerLevel - 2)[1]);
    }

    private void Update()
    {
        storyTitleTMP.rectTransform.position = new Vector2(storyWall.transform.position.x, storyTitleTMP.rectTransform.position.y);
        storyBodyTMP.rectTransform.position = new Vector2(storyWall.transform.position.x, storyBodyTMP.rectTransform.position.y);
        tracksLogo.rectTransform.position = new Vector2(storyWall.transform.position.x - Boundary.visibleWorldExtents.x * 0.6f, 
            tracksLogo.rectTransform.position.y);
    }

    

    //Story blocks are indexed with *player level* (which is increased by accumulating story progress points) and not *game level*. 
    //During a campaign, the story board is only displayed when the player reaches a new *player level*.
    //However the last story board is always displayed at the first *game level* transition.
    //Player can not receive any story progress points before the first *game level* transition.
    //All these conditions are required to ensure that the player never missies a story block.
}
