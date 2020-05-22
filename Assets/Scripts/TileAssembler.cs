using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAssembler : MonoBehaviour
{
    private int[,] tileMatrix;
    private int[] matrixSize = new int[2];

    private Tilemap tilemap;
    private Collider2D tilemapCollider;

    private SpriteRenderer defaultDebris;
    private List<Sprite> debrisSprites = new List<Sprite>();
    private List<Sprite> folliageSprites = new List<Sprite>();

    [HideInInspector]
    public GameManager.Environment activeEnvironment;

    private void OnEnable()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapCollider = GetComponent<Collider2D>();
        matrixSize[1] = Mathf.CeilToInt(Boundary.visibleWorldHeight) + (Mathf.CeilToInt(Boundary.visibleWorldHeight) % 2);
        matrixSize[0] = 3 * matrixSize[1];
        tilemap.size = new Vector3Int(matrixSize[0], matrixSize[1], 0);
    }


    private void BuildMatrix()
    {
        tileMatrix = new int[matrixSize[1], matrixSize[0]];
        for (int x = 0; x < tileMatrix.GetLength(1); x+=2)
        {
            int startPosLow = tileMatrix.GetLength(0);
            int endPosLow = Random.Range(((tileMatrix.GetLength(0) - 1) * 2 / 5) + 1, startPosLow);

            int startPosHigh = Random.Range(0, ((tileMatrix.GetLength(0) - 1) * 2 / 5));
            int endPosHigh = 0;
            for (int y = tileMatrix.GetLength(0) - 1; y >= 0; y--)
            {
                if(y <= startPosLow && y >= endPosLow)  tileMatrix[y, x] = tileMatrix[y, x + 1] = 1;
                else if ((y <= startPosHigh && y >= endPosHigh)) tileMatrix[y, x] = tileMatrix[y, x + 1] = 1;
            }
        }
    }

    public void AssembleTiles(CaveTile caveTile)
    {
        defaultDebris = caveTile.defaultDebris;
        debrisSprites = caveTile.debrisSprites;
        folliageSprites = caveTile.folliageSprites;
        activeEnvironment = caveTile.environment;

        tilemap.ClearAllTiles();
        BuildMatrix();

        for (int x = 0; x < tileMatrix.GetLength(1); x += 2)
        {
            int xPos = x - tileMatrix.GetLength(1) / 2;
            for (int y = tileMatrix.GetLength(0) - 1; y >= 0; y--)
            {
                int yPos = -(y + 1) + tileMatrix.GetLength(0) / 2;
                if (tileMatrix[y, x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(xPos, yPos, 0), caveTile);
                    tilemap.SetTile(new Vector3Int(xPos + 1, yPos, 0), caveTile);
                }
            }
        }
        //Paint two extra layers on top and bottom to fix all tile mismatches
        for (int x = 0; x < tileMatrix.GetLength(1); x ++)
        {
            int xPos = x - tileMatrix.GetLength(1) / 2;
            int top = Mathf.CeilToInt(tileMatrix.GetLength(0) / 2.0f);
            int bottom = -Mathf.RoundToInt((tileMatrix.GetLength(0) + 1)/ 2.0f);

            tilemap.SetTile(new Vector3Int(xPos, top - 1, 0), caveTile);
            tilemap.SetTile(new Vector3Int(xPos, top, 0), caveTile);
            tilemap.SetTile(new Vector3Int(xPos, bottom, 0), caveTile);
            tilemap.SetTile(new Vector3Int(xPos, bottom + 1, 0), caveTile);
        }
        tilemap.RefreshAllTiles();
    }

    public void Prepare(int position)
    {
        transform.position = (position == 1 ? Vector2.zero : Vector2.right * matrixSize[0]) + 
            (Vector2.right * matrixSize[0]/2) + Boundary.visibleWorldCentre; 
    }

    public void SpawnDebris()
    {
        int debrisRaffle = Random.Range(0, debrisSprites.Count);

        defaultDebris.sprite = debrisSprites[debrisRaffle];

        SpriteRenderer debris = Instantiate(defaultDebris, Vector3.zero, Quaternion.Euler(0, 0, (Random.value * 180) - 90));

        if (Boundary.visibleWorldMax.x + Mathf.Max(debris.bounds.extents.x, debris.bounds.extents.y) > tilemapCollider.bounds.max.x)
        {
            Destroy(debris.gameObject);
            return;
        }

        float xPos = Random.Range(Boundary.visibleWorldMax.x + Mathf.Max(debris.bounds.extents.x, debris.bounds.extents.y), tilemapCollider.bounds.max.x);
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

    public void SpawnFolliage()
    {
        int spriteRaffle = Random.Range(0, folliageSprites.Count);
        Sprite folliageSprite = folliageSprites[spriteRaffle];

        GameObject folliageObject = new GameObject();
        SpriteRenderer folliageRenderer = folliageObject.AddComponent<SpriteRenderer>();
        folliageRenderer.sprite = folliageSprite;

        if (Boundary.visibleWorldMax.x + folliageRenderer.bounds.extents.x > tilemapCollider.bounds.max.x ||
            Boundary.visibleWorldMax.x < tilemapCollider.bounds.min.x)
        {
            Destroy(folliageObject);
            return;
        }

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
