using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Sprite spikeSprite;
    public float rotationSpeed;
    public float spikeSize;
    public Color spikeColor;
    private List<Transform> spikes = new List<Transform>();
    void Start()
    {
        transform.position = new Vector2(Boundary.visibleWorldMin.x, Boundary.visibleWorldCentre.y);
        GenerateSpikes();
    }

    void FixedUpdate()
    {
        for(int i = 0; i < spikes.Count; i++)
        {
            Transform spike = spikes[i];

            int dir = i % 2 == 0 ? 1 : -1;

            spike.rotation *= Quaternion.Euler(0, 0, rotationSpeed * dir * Time.fixedDeltaTime * GameManager.gameSpeed);
        }

        transform.position += Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;
    }

    void GenerateSpikes()
    {
        float spikeHeight = spikeSprite.bounds.size.y;
        float scale = spikeSize / spikeHeight;
        spikeHeight *= scale;
        int spikeCount = Mathf.CeilToInt(Boundary.visibleWorldHeight / spikeHeight) + 1;

        Vector2 spikePos = Boundary.visibleWorldMin;
        for(int i = 0; i < spikeCount; i++)
        {
            GameObject spike = new GameObject();
            spike.transform.SetParent(transform);

            SpriteRenderer spikeRenderer = spike.AddComponent<SpriteRenderer>();
            spikeRenderer.sprite = spikeSprite;
            spikeRenderer.color = spikeColor;
            spikeRenderer.sortingLayerName = "Tilemap";
            spikeRenderer.sortingOrder = 1;

            spike.transform.SetPositionAndRotation(spikePos, Quaternion.Euler(0, 0, i * 40));
            spike.transform.localScale *= scale;

            spikes.Add(spike.transform);

            spikePos += Vector2.up * spikeHeight;
        }
    }
}
