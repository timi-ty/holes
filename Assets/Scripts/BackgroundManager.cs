using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundManager : MonoBehaviour
{
    private List<List<Transform>> backgroundSets = new List<List<Transform>>();
    private Transform backgroundOverlay;
    private Vector2 backgroundExtents;
    private const float backgroundSpeed = 0.01875f;
    private List<int> layerSpeedRatio = new List<int>();

    public List<Background> backgrounds = new List<Background>();
    
    void Start()
    {

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

        if (backgroundOverlay)
        {
            Vector3 deltaPosition = Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;

            backgroundOverlay.position += deltaPosition;
        }
    }

    public void SetBackground(GameManager.Environment environment)
    {
        Background background = GetBackground(environment);

        Sprite overlay = background.overlay;

        layerSpeedRatio = background.layerSpeedRatio;

        backgroundExtents = ScaleBackgroundToScreen(background.GetBackgroundSize());

        int backgroundSetCount = Mathf.CeilToInt(Boundary.visibleWorldExtents.x / backgroundExtents.x) + 1;

        Vector3 position = new Vector3(Boundary.visibleWorldMin.x + backgroundExtents.x, Boundary.visibleWorldCentre.y);

        //Destroy all old background sets
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        backgroundSets.Clear();

        //Create new background sets
        for (int i = 0; i < backgroundSetCount; i++)
        {
            List<Transform> backgroundSet = new List<Transform>();

            for (int j = 0; j < background.layerCount; j++)
            {
                GameObject layerObject = new GameObject();
                layerObject.transform.SetPositionAndRotation(position, Quaternion.identity);
                SpriteRenderer layerRenderer = layerObject.AddComponent<SpriteRenderer>();
                layerRenderer.sprite = background.GetLayerSprite(j);
                layerRenderer.color = background.overlayTint;
                layerRenderer.sortingLayerName = "Background";
                layerRenderer.sortingOrder = j;
                layerObject.name = layerRenderer.sprite.name;
                backgroundSet.Add(layerObject.transform);
            }

            backgroundSets.Add(backgroundSet);

            position.x += backgroundExtents.x * 1.99f;  //0.01f margin to avoid tearing between background sets

            if (i == backgroundSetCount - 1 && overlay)
            {
                PlaceOverlay(overlay, background.layerCount, background.overlayTint);
            }
        }

        foreach (List<Transform> backgroundSet in backgroundSets)
        {
            foreach (Transform backgroundLayer in backgroundSet)
            {
                Vector3 formerScale = backgroundLayer.localScale;
                backgroundLayer.SetParent(transform);
                backgroundLayer.localScale = formerScale;
            }
        }
    }

    private Background GetBackground(GameManager.Environment environment)
    {
        List<int> candidates = new List<int>();
        for (int i = 0; i < backgrounds.Count; i++)
        {
            if (backgrounds[i].environment == environment)
            {
                candidates.Add(i);
            }
        }

        int raffle = Random.Range(0, candidates.Count);
            
        return backgrounds[candidates[raffle]];
    }

    private Vector2 ScaleBackgroundToScreen(Vector2 backgroundSize)
    {
        Vector2 imageSize = Camera.main.WorldToScreenPoint(Boundary.visibleWorldMin + backgroundSize);

        float scale = Camera.main.pixelHeight / imageSize.y;

        transform.localScale = new Vector3(scale, scale, 1);

        return (backgroundSize * scale) / 2;
    }

    private void PlaceOverlay(Sprite overlay, int sortingOrder, Color tint)
    {
        Vector2 overlaySize = Camera.main.WorldToScreenPoint(Boundary.visibleWorldMin + (Vector2)overlay.bounds.size);

        float overlayScaleX = Camera.main.pixelWidth / overlaySize.x;
        float overlayScaleY = Camera.main.pixelHeight / overlaySize.y;

        GameObject overlayObject = new GameObject();
        backgroundOverlay = overlayObject.transform;
        backgroundOverlay.SetPositionAndRotation(Boundary.visibleWorldCentre, Quaternion.identity);
        backgroundOverlay.localScale = new Vector3(overlayScaleX, overlayScaleY, 1) * 1.1f;
        backgroundOverlay.SetParent(transform);
        SpriteRenderer overlayRenderer = overlayObject.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = overlay;
        overlayRenderer.color = tint;
        overlayRenderer.sortingLayerName = "Background";
        overlayRenderer.sortingOrder = sortingOrder;
        overlayObject.name = "Overlay";
    }
}