using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionWall : MonoBehaviour
{
    private SpriteRenderer wallRenderer;
    public SpriteRenderer wallShadow;

    private void Start()
    {
        wallRenderer = GetComponent<SpriteRenderer>();

        SizeWallToScreen();
    }

    private void SizeWallToScreen()
    {
        Vector2 diff = wallShadow.bounds.size - wallRenderer.bounds.size;
        Vector2 offset = Vector2.one * 2; // Adjust as needed to buffer the side edges of the wall.

        Vector2 imageSize = Camera.main.WorldToScreenPoint(Boundary.visibleWorldMin + (Vector2) wallRenderer.bounds.size);

        float scaleY = Camera.main.pixelHeight / imageSize.y;
        float scaleX = Camera.main.pixelWidth / imageSize.x;

        wallRenderer.size *= new Vector3(scaleX, scaleY, 1);
        wallRenderer.size += offset; // Refer to offset above for explanation.
        wallShadow.transform.localScale = transform.localScale;
        wallShadow.size = wallRenderer.size + diff;
    }

    public void ShowWall()
    {
        wallRenderer.enabled = true;
        wallShadow.enabled = true;

        transform.position = new Vector3(Boundary.visibleWorldMax.x +
            wallShadow.bounds.extents.x, Boundary.visibleWorldCentre.y);

        StartCoroutine(AdjustWall());
    }

    public void HideWall()
    {
        if (wallShadow.bounds.max.x
            <= Boundary.visibleWorldMin.x && wallRenderer.enabled)
        {
            wallRenderer.enabled = false;
            wallShadow.enabled = false;

            GameManager.TransitionConcluded();
        }
    }

    private IEnumerator AdjustWall()
    {
        float offset = 1; //float offset should always be half of the x compoent of the offset vector in SizeWallToScreen().
        float diff = wallShadow.bounds.extents.x - wallRenderer.bounds.extents.x; //to elimiate effect of the shadow on calcuating wall adjustment.
        offset += diff;
        float pos = 0;
        while (pos < offset)
        {
            pos += Time.deltaTime * 0.25f * offset;
            transform.position += Vector3.left * Time.deltaTime * 0.25f * offset;
            yield return null;
        }
    }
}
