using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    TileAssembler tilemap1;
    TileAssembler tilemap2;
    Collider2D tilemap1Collider;
    Collider2D tilemap2Collider;
    [Header("Parameters")]
    public float debrisDensity;
    public float folliagedensity;

    [Header("Resources")]
    public List<Sprite> debrisSprites = new List<Sprite>();
    public SpriteRenderer defaultDebris;
    public List<Sprite> folliageSprites = new List<Sprite>();
    void Start()
    {
        if (transform.childCount < 2) throw new UnityException("Needs two tilemaps to scroll. Add tilemaps to your grid.");

        tilemap1 = transform.GetChild(0).GetComponent<TileAssembler>();
        tilemap2 = transform.GetChild(1).GetComponent<TileAssembler>();

        tilemap1.Prepare(1);
        tilemap2.Prepare(2);

        tilemap1.AssembleTiles();
        tilemap2.AssembleTiles();

        tilemap1Collider = tilemap1.GetComponent<Collider2D>();
        tilemap2Collider = tilemap2.GetComponent<Collider2D>();

        if (!tilemap1 || !tilemap2) throw new UnityException("Needs two tilemaps to scroll. Add only tilemaps to your grid.");

        InvokeRepeating("SpawnTilemapDebris", 1, 5/(debrisDensity + 0.1f));
        InvokeRepeating("SpawnTilemapFolliage", 1, 5 / folliagedensity + 0.1f);
    }

    
    void Update()
    {
        ScrollTileMaps();
    }

    void ScrollTileMaps()
    {
        if (tilemap1.transform.position.x <= Boundary.visibleWorldCentre.x && tilemap2.transform.position.x < tilemap1.transform.position.x)
        {
            tilemap2.AssembleTiles();

            tilemap2.transform.position = tilemap1.transform.position + (Vector3.right * 
                (tilemap2Collider.bounds.extents.x + tilemap1Collider.bounds.extents.x));
            int count = tilemap2.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(tilemap2.transform.GetChild(i).gameObject);
            }
        }

        if (tilemap2.transform.position.x <= Boundary.visibleWorldCentre.x && tilemap1.transform.position.x < tilemap2.transform.position.x)
        {
            tilemap1.AssembleTiles();

            tilemap1.transform.position = tilemap2.transform.position + (Vector3.right * 
                (tilemap1Collider.bounds.extents.x + tilemap2Collider.bounds.extents.x));
            int count = tilemap1.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(tilemap1.transform.GetChild(i).gameObject);
            }
        }
    }

    void SpawnTilemapDebris()
    {
        int debrisRaffle = Random.Range(0, debrisSprites.Count);

        defaultDebris.sprite = debrisSprites[debrisRaffle];

        SpriteRenderer debris = Instantiate(defaultDebris, Vector3.zero, Quaternion.Euler(0, 0, (Random.value * 180) - 90));

        float maximumX = Mathf.Max(tilemap1Collider.bounds.max.x, tilemap2Collider.bounds.max.x);

        float xPos = Random.Range(Boundary.visibleWorldMax.x + Mathf.Max(debris.bounds.extents.x, debris.bounds.extents.y), maximumX);
        float yPos = (Random.value * Boundary.visibleWorldSize.y) + Boundary.visibleWorldMin.y;

        int layerMask = LayerMask.GetMask("Tilemap");

        RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(xPos, yPos), 
            Vector2.up * (1 + (2 * Random.Range(-1, 1))), Boundary.visibleWorldSize.y, layerMask);

        if (!raycastHit || !raycastHit.transform.CompareTag("BoundaryTilemap"))
        {
            Destroy(debris.gameObject);
            //Debug.Log("Didn't find Ground or Ceiling, destroying...");
            return;
        }

        debris.transform.SetParent(raycastHit.transform);
        debris.transform.position = raycastHit.point;
    }

    void SpawnTilemapFolliage()
    {
        int spriteRaffle = Random.Range(0, folliageSprites.Count);
        Sprite folliageSprite = folliageSprites[spriteRaffle];

        GameObject folliageObject = new GameObject();
        SpriteRenderer folliageRenderer = folliageObject.AddComponent<SpriteRenderer>();
        folliageRenderer.sprite = folliageSprite;

        folliageObject.name = folliageSprite.name + " Folliage";
        folliageObject.transform.localScale = Vector3.one * (0.5f + (Random.value * 0.5f));
        float xPos = Boundary.visibleWorldMax.x + (Boundary.visibleWorldSize.x * 0.1f) + folliageRenderer.bounds.size.x;
        float yPos = Boundary.visibleWorldMin.y;

        Vector2 ceilingPoint = Vector2.one * float.NegativeInfinity;
        Vector2 floorPoint = Vector2.one * float.PositiveInfinity;
        Vector2 raycastOrigin = new Vector2(xPos, yPos);
        int layerMask = LayerMask.GetMask("Tilemap");

        Transform parent = transform;

        while (raycastOrigin.y <= Boundary.visibleWorldMax.y)
        {
            RaycastHit2D rayHitUp = Physics2D.Raycast(raycastOrigin, Vector2.up, Boundary.visibleWorldSize.y, layerMask);
            RaycastHit2D rayHitDown = Physics2D.Raycast(raycastOrigin, Vector2.down, Boundary.visibleWorldSize.y, layerMask);

            raycastOrigin += Vector2.up * 0.1f;

            if (!rayHitUp.transform || !rayHitDown.transform) continue;

            ceilingPoint = rayHitUp.transform.CompareTag("BoundaryTilemap") ? rayHitUp.point : ceilingPoint;
            floorPoint = rayHitDown.transform.CompareTag("BoundaryTilemap") ? rayHitDown.point : floorPoint;

            if (ceilingPoint.y > floorPoint.y)
            {
                //Debug.Log("Cave height resolved. Spawning obstacle");
                parent = rayHitDown.transform;
                break;
            }
        }

        if (ceilingPoint.y <= floorPoint.y)
        {
            //Debug.Log("Obstacle spawn failed. Could not find resolve cave height.");
            Destroy(folliageObject);
            return;
        }

        yPos = floorPoint.y + folliageRenderer.bounds.extents.y;

        folliageObject.transform.SetParent(parent);
        folliageObject.transform.position = new Vector2(xPos, yPos);

        /*********************************************************************************************************/

        Vector3 bottomLeft = folliageRenderer.bounds.min;
        RaycastHit2D rayHitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, Boundary.visibleWorldSize.y, layerMask);
        RaycastHit2D rayHitMiddle = Physics2D.Raycast(new Vector2(folliageRenderer.bounds.center.x, folliageRenderer.bounds.min.y), Vector2.down, 
            Boundary.visibleWorldSize.y, layerMask);
        Vector3 nudgePos = Vector2.zero;

        while (rayHitLeft.point.y < rayHitMiddle.point.y)
        {
            nudgePos += Vector3.right * 0.05f;

            rayHitLeft = Physics2D.Raycast(bottomLeft + nudgePos, Vector2.down, Boundary.visibleWorldSize.y, layerMask);
        }

        folliageObject.transform.position += nudgePos;

        Vector3 bottomRight = new Vector2(folliageRenderer.bounds.max.x, folliageRenderer.bounds.min.y);
        RaycastHit2D rayHitRight = Physics2D.Raycast(bottomRight, Vector2.down, Boundary.visibleWorldSize.y, layerMask);
        nudgePos = Vector2.zero;

        while (rayHitRight.point.y < rayHitMiddle.point.y)
        {
            nudgePos += Vector3.left * 0.05f;

            rayHitRight = Physics2D.Raycast(bottomRight + nudgePos, Vector2.down, Boundary.visibleWorldSize.y, layerMask);
        }

        folliageObject.transform.position += nudgePos;

        float minFloor = float.PositiveInfinity;
        for (float xPoint = folliageRenderer.bounds.min.x; xPoint < folliageRenderer.bounds.max.x; xPoint += 0.05f)
        {
            minFloor = Mathf.Min(Physics2D.Raycast(new Vector2(xPoint, yPos), Vector2.down, Boundary.visibleWorldSize.y, layerMask).point.y, minFloor);
        }

        yPos = minFloor + folliageRenderer.bounds.extents.y;

        folliageObject.transform.position = new Vector2(folliageObject.transform.position.x, yPos);
    }
}
