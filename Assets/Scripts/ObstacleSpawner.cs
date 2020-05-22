using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Parameters")]
    public float spawnInterval;
    [Range(0.0f, 1.0f)]
    public float spawnProbability;

    [Header("Resources")]
    public ObstaclesData obstacleData;
    public DestructibleObstacle defaultDestructibleObstacle;
    public SwingingObstacle defaultSwingingObstacle;

    private float timer;

    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnInterval)
        {
            int spawnOdds = Mathf.RoundToInt(1.0f / spawnProbability);
            int spawnRaffle = Random.Range(1, spawnOdds + 1);
            if(spawnRaffle == 1)
            {
                int spawn = Random.Range(0, 2);
                if (spawn == 0) SpawnDestructibleObstacle();
                else SpawnSwingingObstacle();
            }
            timer = 0;
        }
    }

    private void SpawnDestructibleObstacle()
    {
        Vector2 ceilingPoint = Vector2.one * float.NegativeInfinity;
        Vector2 floorPoint = Vector2.one * float.PositiveInfinity;
        Vector3 screedEdge = new Vector3(Boundary.visibleWorldMax.x + (Boundary.visibleWorldSize.x * 0.1f), Boundary.visibleWorldCentre.y);

        RaycastHit2D preRay = Physics2D.Raycast(screedEdge, Vector2.up);
        
        if(preRay.transform != null)
        {
            TileAssembler tilemap = preRay.transform.GetComponent<TileAssembler>();
            if (!tilemap) return;
            obstacleData.Refresh(tilemap.activeEnvironment);
        }

        Sprite obstacleSprite = obstacleData.GetDestructibleSprite();

        int widthRaffle = Random.Range(1, 5);
        float width = obstacleSprite.bounds.size.x * widthRaffle;

        DestructibleObstacle obstacle = Instantiate(defaultDestructibleObstacle, transform.position, Quaternion.identity, transform);

        Vector2 raycastOrigin = new Vector2(screedEdge.x + width/2, Boundary.visibleWorldMin.y);

        while (raycastOrigin.y <= Boundary.visibleWorldMax.y)
        {
            RaycastHit2D rayHitUp = Physics2D.Raycast(raycastOrigin, Vector2.up);
            RaycastHit2D rayHitDown = Physics2D.Raycast(raycastOrigin, Vector2.down);

            raycastOrigin += Vector2.up * 0.1f;

            if (!rayHitUp.transform || !rayHitDown.transform) continue;
            
            ceilingPoint = rayHitUp.transform.CompareTag("BoundaryTilemap") ? rayHitUp.point : ceilingPoint;
            floorPoint = rayHitDown.transform.CompareTag("BoundaryTilemap") ? rayHitDown.point : floorPoint;

            if (rayHitUp.transform.CompareTag("DestructibleObstacle") || rayHitDown.transform.CompareTag("DestructibleObstacle"))
                raycastOrigin += new Vector2(1, -1) * 0.1f;

            if (ceilingPoint.y > floorPoint.y)
            {
                //Debug.Log("Cave height resolved. Spawning obstacle");
                break;
            }
        }

        if (ceilingPoint.y <= floorPoint.y)
        {
            //Debug.Log("Obstacle spawn failed. Could not find resolve cave height.");
            Destroy(obstacle.gameObject);
            return;
        }

        Vector2 obstacleSize = new Vector2(width, ceilingPoint.y - floorPoint.y);

        Vector2 spawnPos = new Vector2(floorPoint.x, floorPoint.y + obstacleSize.y/2.0f);

        obstacle.transform.position = spawnPos;
        obstacle.tag = defaultDestructibleObstacle.tag;
        obstacle.name = obstacleSprite.name + " Obstacle";


        obstacle.Modify(sprite: obstacleSprite,
                        fragmentHeight: null,
                        brittlenessFactor: obstacleData.GetBrittleness(),
                        damageMask: obstacleData.GetDestructionMask(),
                        damageDebris: null,
                        obstacleSize: obstacleSize);
    }

    private void SpawnSwingingObstacle()
    {
        SwingingObstacle swingingObstacle = Instantiate(defaultSwingingObstacle, transform);

        Vector2 ceilingPoint = Vector2.one * float.NegativeInfinity;
        Vector2 floorPoint = Vector2.one * float.PositiveInfinity;
        Vector3 screedEdge = new Vector3(Boundary.visibleWorldMax.x + (Boundary.visibleWorldSize.x * 0.1f), Boundary.visibleWorldCentre.y);
        Vector2 raycastOrigin = new Vector2(screedEdge.x, Boundary.visibleWorldMin.y);

        while (raycastOrigin.y <= Boundary.visibleWorldMax.y)
        {
            RaycastHit2D rayHitUp = Physics2D.Raycast(raycastOrigin, Vector2.up);
            RaycastHit2D rayHitDown = Physics2D.Raycast(raycastOrigin, Vector2.down);

            raycastOrigin += Vector2.up * 0.1f;

            if (!rayHitUp.transform || !rayHitDown.transform) continue;

            ceilingPoint = rayHitUp.transform.CompareTag("BoundaryTilemap") ? rayHitUp.point : ceilingPoint;
            floorPoint = rayHitDown.transform.CompareTag("BoundaryTilemap") ? rayHitDown.point : floorPoint;

            if (rayHitUp.transform.CompareTag("DestructibleObstacle")
                || rayHitDown.transform.CompareTag("DestructibleObstacle"))
                raycastOrigin += new Vector2(1, -1) * 0.1f;

            if (ceilingPoint.y > floorPoint.y)
            {
                //Debug.Log("Cave height resolved. Spawning obstacle");
                break;
            }
        }

        if (ceilingPoint.y <= floorPoint.y)
        {
            Debug.Log("Obstacle spawn failed. Could not find resolve cave height.");
            Destroy(swingingObstacle);
            return;
        }

        RaycastHit2D postRay = Physics2D.Raycast(screedEdge, Vector2.up);

        if (postRay.transform != null)
        {
            TileAssembler tilemap = postRay.transform.GetComponent<TileAssembler>();
            if (!tilemap) { Destroy(swingingObstacle); return; }
            obstacleData.Refresh(tilemap.activeEnvironment);
        }

        Sprite obstacleSprite = obstacleData.GetSwingingSprite();

        float obstacleLength = (ceilingPoint.y - floorPoint.y) / (Random.value + 1.1f);

        Vector2 hingePoint = new Vector2(ceilingPoint.x, ceilingPoint.y);

        swingingObstacle.transform.position = hingePoint;
        swingingObstacle.tag = defaultDestructibleObstacle.tag;
        swingingObstacle.name = obstacleSprite.name + " Obstacle";


        swingingObstacle.Modify(hingePoint: hingePoint,
                                obstacleSprite: obstacleSprite,
                                chainLength: obstacleLength,
                                chainWidth: 0.05f,
                                chainResilience: obstacleData.GetResilience(),
                                obstacleSize: 0.5f,
                                hingeSprite: obstacleData.hingeSprite,
                                isSpinning: obstacleData.GetSpin());
    }
}
