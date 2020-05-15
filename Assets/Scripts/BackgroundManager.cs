using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    List<List<Transform>> backgroundSets = new List<List<Transform>>();
    Vector2 backgroundExtents;
    private const float backgroundSpeed = 0.01875f;
    [Range(0f, 12f)]
    public List<int> layerSpeedRatio = new List<int>();
    void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (!spriteRenderer)
        {
            throw new UnityException("Background parent must contain at least one sprite as a child object");
        }

        bool applyDefaultRatio = false;

        if(layerSpeedRatio.Count < transform.childCount)
        {
            Debug.LogWarning("Speed ratio should correspond with number of background layers. Applying default ratio...");
            applyDefaultRatio = true;
            layerSpeedRatio = new List<int>();
        }

        backgroundExtents = ScaleBackgroundToScreen();

        int backgroundSetCount = Mathf.CeilToInt(Boundary.visibleWorldExtents.x / backgroundExtents.x) + 1;

        Vector3 position = new Vector3(Boundary.visibleWorldMin.x + backgroundExtents.x, Boundary.visibleWorldCentre.y);

        for (int i = 0; i < backgroundSetCount; i++)
        {
            List<Transform> backgroundSet = new List<Transform>();

            for (int j = 0; j < transform.childCount; j++)
            {
                if (i == 0)
                {
                    Transform backgroundLayer = transform.GetChild(j);
                    backgroundLayer.position = position;
                    backgroundSet.Add(backgroundLayer);

                    if (applyDefaultRatio) layerSpeedRatio.Add(j + 1);
                }
                else
                {
                    Transform backgroundLayer = Instantiate(transform.GetChild(j), position, Quaternion.identity);
                    backgroundLayer.name = transform.GetChild(j).name;
                    backgroundSet.Add(backgroundLayer);
                }
            }

            backgroundSets.Add(backgroundSet);

            position.x += backgroundExtents.x * 1.99f;  //0.01f margin to avoid tearing between background sets
        }

        foreach(List<Transform> backgroundSet in backgroundSets)
        {
            foreach (Transform backgroundLayer in backgroundSet)
            {
                Vector3 formerScale = backgroundLayer.localScale;
                backgroundLayer.SetParent(transform);
                backgroundLayer.localScale = formerScale;
            }
        }
    }

    void FixedUpdate()
    {
        for(int i = 0; i < backgroundSets.Count; i++)
        {
            List<Transform> backgroundSet = backgroundSets[i];
            float screenRange = Boundary.visibleWorldMin.x - backgroundExtents.x;
            float backgroundWidth = backgroundExtents.x * 1.99f; //0.01f margin to avoid tearing between background sets

            for (int j = 0; j < backgroundSet.Count; j++)
            {
                Transform backgroundLayer = backgroundSet[j];

                bool isOutOfScreen = backgroundLayer.position.x <= screenRange;

                if (isOutOfScreen)
                {
                    Vector3 offset = new Vector2(backgroundWidth * backgroundSets.Count, 0);
                    backgroundLayer.position += offset;
                }

                Vector3 deltaPosition = (Vector3.left * Time.fixedDeltaTime * (Mathf.Pow(2, (layerSpeedRatio[j]))) 
                    * backgroundSpeed * GameManager.gameSpeed) + (Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed);

                backgroundLayer.position += deltaPosition;
            }
        }
    }

    private Vector2 ScaleBackgroundToScreen()
    {
        Vector2 imageSize = Camera.main.WorldToScreenPoint(Boundary.visibleWorldMin + (Vector2) spriteRenderer.bounds.size);

        float scale = Camera.main.pixelHeight / imageSize.y;

        transform.localScale = new Vector3(scale, scale, 1);

        return spriteRenderer.bounds.extents;
    }
}