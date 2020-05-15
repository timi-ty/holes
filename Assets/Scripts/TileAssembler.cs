using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAssembler : MonoBehaviour
{
    private int[,] tileMatrix;
    private int[] matrixSize = new int[2];

    private Tilemap tilemap;
    public CaveTile caveTile;

    private void OnEnable()
    {
        tilemap = GetComponent<Tilemap>();
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

    public void AssembleTiles()
    {
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
        transform.position = (position == 1 ? Vector2.zero : Vector2.right * matrixSize[0]) + (Vector2.right * matrixSize[0]/2) + Boundary.visibleWorldCentre; 
    }
}
