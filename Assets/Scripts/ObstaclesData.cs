using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObstaclesData : ScriptableObject
{
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

    [Header("Saw")]
    public Sprite sawObstacle;

    [Header("CircleSaw")]
    public Sprite circleSawObstacle;

    [Header("Chain")]
    public Material chainMaterial;
    [Range(0, 10)]
    public int chainResilience;

    [Header("Rope")]
    public Material ropeMaterial;
    [Range(0, 10)]
    public int ropeResilience;

    [Header("Hinge")]
    public Sprite hingeSprite;

    private GameManager.Environment environment;

    public void Refresh(GameManager.Environment environment)
    {
        this.environment = environment;
    }

    public Sprite GetDestructibleSprite()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return crateObstacle;
            case GameManager.Environment.Snow:
                return iceObstacle;
        }
        Debug.LogError("No Environment Set");
        return null;
    }

    public Sprite GetSwingingSprite()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return boulderObstacle;
            case GameManager.Environment.Snow:
                return circleSawObstacle;
        }
        Debug.LogError("No Environment Set");
        return null;
    }

    public Sprite GetDestructionMask()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return crateDestructionMask;
            case GameManager.Environment.Snow:
                return iceDestructionMask;
        }
        Debug.LogError("No Environment Set");
        return null;
    }

    public int GetBrittleness()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return crateBrittlenessFactor;
            case GameManager.Environment.Snow:
                return iceBrittlenessFactor;
        }
        Debug.LogError("No Environment Set");
        return 0;
    }

    public int GetResilience()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return ropeResilience;
            case GameManager.Environment.Snow:
                return chainResilience;
        }
        Debug.LogError("No Environment Set");
        return 0;
    }

    public Material GetRopeMaterial()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return ropeMaterial;
            case GameManager.Environment.Snow:
                return chainMaterial;
        }
        Debug.LogError("No Environment Set");
        return null;
    }

    public bool GetSpin()
    {
        switch (environment)
        {
            case GameManager.Environment.Desert:
                return false;
            case GameManager.Environment.Snow:
                return true;
        }
        Debug.LogError("No Environment Set");
        return false;
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