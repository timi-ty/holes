using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObstaclesData : ScriptableObject
{
    public enum DestructibleObstacleType { Crate, Stone, Ice }
    public enum SwingingObstacleType { Boulder, Saw, CircleSaw }
    public enum ObstacleSuspender { Rope, Chain }

    [Header("Crate")]
    public Sprite crateObstacle;
    public Sprite crateDestructionMask;
    [Range(0, 10)]
    public int crateBrittlenessFactor;

    [Header("Stone")]
    public Sprite stoneObstacle;
    public Sprite stoneDestructionMask;
    [Range(0, 10)]
    public int stoneBrittlenessFactor;

    [Header("Ice")]
    public Sprite iceObstacle;
    public Sprite iceDestructionMask;
    [Range(0, 10)]
    public int iceBrittlenessFactor;

    [Header("Boulder")]
    public Sprite boulderObstacle;
    public Sprite boulderDestructionMask;
    public int boulderBrittlenesFactor;

    [Header("Saw")]
    public Sprite sawObstacle;
    public Sprite SawDestructionMask;
    public int sawBrittlenessFactor;

    [Header("CircleSaw")]
    public Sprite circleSawObstacle;
    public Sprite circleSawDestructionMask;
    public int circleSawBrittlenessFactor;

    [Header("Chain")]
    public List<Sprite> chainLinkSprites;
    public int chainLinkLength;
    public int chainResilience;

    [Header("Rope")]
    public List<Sprite> ropeLinkSprites;
    public int ropeLinkLength;
    public int ropeResilience;

    [Header("Hinge")]
    public Sprite hingeSprite;

    public Sprite GetSprite(DestructibleObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case DestructibleObstacleType.Crate: return crateObstacle;
            case DestructibleObstacleType.Stone: return stoneObstacle;
            case DestructibleObstacleType.Ice: return iceObstacle;
        }
        Debug.LogError("Invalid obstacle type");
        return null;
    }

    public Sprite GetSprite(SwingingObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case SwingingObstacleType.Boulder: return boulderObstacle;
            case SwingingObstacleType.Saw: return sawObstacle;
            case SwingingObstacleType.CircleSaw: return circleSawObstacle;
        }
        Debug.LogError("Invalid obstacle type");
        return null;
    }

    public Sprite GetMask(DestructibleObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case DestructibleObstacleType.Crate: return crateDestructionMask;
            case DestructibleObstacleType.Stone: return stoneDestructionMask;
            case DestructibleObstacleType.Ice: return iceDestructionMask;
        }
        Debug.LogError("Invalid obstacle type");
        return null;
    }

    public Sprite GetMask(SwingingObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case SwingingObstacleType.Boulder: return boulderDestructionMask;
            case SwingingObstacleType.Saw: return SawDestructionMask;
            case SwingingObstacleType.CircleSaw: return circleSawDestructionMask;
        }
        Debug.LogError("Invalid obstacle type");
        return null;
    }

    public int GetBrittleness(DestructibleObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case DestructibleObstacleType.Crate: return crateBrittlenessFactor;
            case DestructibleObstacleType.Stone: return stoneBrittlenessFactor;
            case DestructibleObstacleType.Ice: return iceBrittlenessFactor;
        }
        Debug.LogError("Invalid obstacle type");
        return 0;
    }

    public int GetBrittleness(SwingingObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case SwingingObstacleType.Boulder: return boulderBrittlenesFactor;
            case SwingingObstacleType.Saw: return sawBrittlenessFactor;
            case SwingingObstacleType.CircleSaw: return circleSawBrittlenessFactor;
        }
        Debug.LogError("Invalid obstacle type");
        return 0;
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Obstacle Data")]
    public static void CreateObstacleData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Obstacle Data", "New Obstacle Data", "Asset", "Save Obstacle Data", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ObstaclesData>(), path);
    }
#endif
}