using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    private TileAssembler tilemap1;
    private TileAssembler tilemap2;
    private TileAssembler leadingTilemap;
    private Collider2D tilemap1Collider;
    private Collider2D tilemap2Collider;
    private Collider2D leadingTilemapCollider;

    [Header("Parameters")]
    public float debrisDensity;
    public float folliagedensity;

    [Header("Resources")]
    public List<CaveTile> caveTiles;

    private void OnEnable()
    {
        GameManager.tileCount = caveTiles.Count;
    }

    void Start()
    {
        if (transform.childCount < 2) throw new UnityException("Needs two tilemaps to scroll. Add tilemaps to your grid.");

        tilemap1 = transform.GetChild(0).GetComponent<TileAssembler>();
        tilemap2 = transform.GetChild(1).GetComponent<TileAssembler>();

        tilemap1.Prepare(1);
        tilemap2.Prepare(2);

        tilemap1.AssembleTiles(caveTiles[GameManager.tileIndex]);
        tilemap2.AssembleTiles(caveTiles[GameManager.tileIndex]);

        tilemap1Collider = tilemap1.GetComponent<Collider2D>();
        tilemap2Collider = tilemap2.GetComponent<Collider2D>();

        if (!tilemap1 || !tilemap2) throw new UnityException("Needs two tilemaps to scroll. Add only tilemaps to your grid.");

        InvokeRepeating("SpawnDebris", 1, 5/(debrisDensity + 0.1f));
        InvokeRepeating("SpawnFolliage", 1, 5 / folliagedensity + 0.1f);

        GameManager.EndEnvironmentTransition(tilemap1.activeEnvironment);
    }

    
    void Update()
    {
        ScrollTileMaps();

        ConfirmEnvironmentSync();
    }

    void ScrollTileMaps()
    {
        if (tilemap1.transform.position.x <= Boundary.visibleWorldCentre.x && tilemap2.transform.position.x < tilemap1.transform.position.x)
        {
            tilemap2.AssembleTiles(caveTiles[GameManager.tileIndex]);
            if(GameManager.levelUpPending)
            {
                leadingTilemap = tilemap2;
                leadingTilemapCollider = tilemap2Collider;
            }

            tilemap2.transform.position = tilemap1.transform.position + (Vector3.right * 
                (tilemap2Collider.bounds.extents.x + tilemap1Collider.bounds.extents.x));
            for (int i = 0; i < tilemap2.transform.childCount; i++)
            {
                Destroy(tilemap2.transform.GetChild(i).gameObject);
            }
        }

        if (tilemap2.transform.position.x <= Boundary.visibleWorldCentre.x && tilemap1.transform.position.x < tilemap2.transform.position.x)
        {
            tilemap1.AssembleTiles(caveTiles[GameManager.tileIndex]);
            if (GameManager.levelUpPending)
            {
                leadingTilemap = tilemap1;
                leadingTilemapCollider = tilemap1Collider;
            }

            tilemap1.transform.position = tilemap2.transform.position + (Vector3.right * 
                (tilemap1Collider.bounds.extents.x + tilemap2Collider.bounds.extents.x));
            for (int i = 0; i < tilemap1.transform.childCount; i++)
            {
                Destroy(tilemap1.transform.GetChild(i).gameObject);
            }
        }
    }

    bool transitioningBackground;
    private void ConfirmEnvironmentSync()
    {
        if (!GameManager.levelUpPending || !leadingTilemap) return;

        if (leadingTilemap.transform.position.x - leadingTilemapCollider.bounds.extents.x <= Boundary.visibleWorldMax.x && !transitioningBackground)
        {
            GameManager.StartEnvironmentTransition();
            transitioningBackground = true;
        }

        if (leadingTilemap.transform.position.x - leadingTilemapCollider.bounds.extents.x <= Boundary.visibleWorldMin.x)
        {
            GameManager.EndEnvironmentTransition(leadingTilemap.activeEnvironment);
            leadingTilemap = null;
            leadingTilemapCollider = null;
            transitioningBackground = false;
        }
    }

    private void SpawnDebris()
    {
        tilemap1.SpawnDebris();
        tilemap2.SpawnDebris();
    }

    private void SpawnFolliage()
    {
        tilemap1.SpawnFolliage();
        tilemap2.SpawnFolliage();
    }
}
