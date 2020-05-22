using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Background : ScriptableObject
{
    [Header("Layers")]
    public List<Sprite> layers = new List<Sprite>();
    [Range(0f, 12f)]
    public List<int> layerSpeedRatio = new List<int>();

    [Header("Optional Overlay")]
    public Sprite overlay;
    public Color overlayTint;

    [Header("Environment")]
    public GameManager.Environment environment;

    public int layerCount => layers.Count;

    public Vector2 GetBackgroundSize()
    {
        return layers[0].bounds.size;
    }

    public Sprite GetLayerSprite(int layerIndex)
    {
        return layers[layerIndex];
    }

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Background")]
    public static void CreateObstacleData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Background", "New Background", "Asset", "Save Background", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Background>(), path);
    }
#endif
}