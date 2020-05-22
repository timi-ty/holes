using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CaveTile : Tile
{
    public Sprite[] m_Sprites;
    public Sprite m_Preview;
    public bool enableCeilingMods;
    public Sprite leftCornerMod, midMod, rightCornerMod;
    public int leftTrigger, rightTrigger;

    [Header("Extras")]
    public GameManager.Environment environment;

    public SpriteRenderer defaultDebris;
    public List<Sprite> debrisSprites = new List<Sprite>();
    public List<Sprite> folliageSprites = new List<Sprite>();

    // This refreshes itself and other CaveTiles that are orthogonally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasCaveTile(tilemap, position))
                    tilemap.RefreshTile(position);
            }
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        bool up = HasCaveTile(tilemap, location + new Vector3Int(0, 1, 0));
        bool upRight = HasCaveTile(tilemap, location + new Vector3Int(1, 1, 0));
        bool right = HasCaveTile(tilemap, location + new Vector3Int(1, 0, 0));
        bool downRight = HasCaveTile(tilemap, location + new Vector3Int(1, -1, 0));
        bool down = HasCaveTile(tilemap, location + new Vector3Int(0, -1, 0));
        bool downLeft = HasCaveTile(tilemap, location + new Vector3Int(-1, -1, 0));
        bool left = HasCaveTile(tilemap, location + new Vector3Int(-1, 0, 0));
        bool upLeft = HasCaveTile(tilemap, location + new Vector3Int(-1, 1, 0));
        
        int index = GetSpriteIndex(up, upRight, right, downRight, down, downLeft, left, upLeft);

        if (index >= 0 && index < m_Sprites.Length)
        {
            tileData.sprite = enableCeilingMods ? ModifyCeilingSprite(index, location, tilemap) : m_Sprites[index];
            tileData.color = Color.white;
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;
        }
        else
        {
            Debug.LogWarning("Not enough sprites in CaveTile instance");
        }
    }

    private bool HasCaveTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

    private Sprite ModifyCeilingSprite(int index, Vector3Int location, ITilemap tilemap)
    {
        Sprite right = tilemap.GetSprite(location + new Vector3Int(1, 0, 0));
        Sprite left = tilemap.GetSprite(location + new Vector3Int(-1, 0, 0));

        Sprite rightTrigger = m_Sprites[this.rightTrigger];
        Sprite leftTrigger = m_Sprites[this.leftTrigger];

        if (index == this.rightTrigger)
        {
            Vector3Int position = new Vector3Int(-1, 0, 0);
            while (HasCaveTile(tilemap, location + position))
            {
                tilemap.RefreshTile(location + position);
                position += new Vector3Int(-1, 0, 0);
            }
        }

        switch (index)
        {
            case 8:
                return rightTrigger.Equals(right) || midMod.Equals(right) 
                    || leftTrigger.Equals(left) || midMod.Equals(left) ? midMod : m_Sprites[index];
            case 11:
                return rightTrigger.Equals(right) || midMod.Equals(right) ? leftCornerMod : m_Sprites[index];
            case 12:
                return leftTrigger.Equals(left) || midMod.Equals(left)  ? rightCornerMod : m_Sprites[index];
        }
        return m_Sprites[index];
    }

    private int GetSpriteIndex(bool up, bool upRight, bool right, bool downRight, bool down, bool downLeft, bool left, bool upLeft)
    {
        if(up && upRight && right && downRight && down && downLeft && left && upLeft)
        {
            return 4;
        }
        else if (up && !upRight && right && downRight && down && downLeft && left && upLeft)
        {
            return 9;
        }
        else if (up && upRight && right && !downRight && down && downLeft && left && upLeft)
        {
            return 14;
        }
        else if (up && upRight && right && downRight && down && !downLeft && left && upLeft)
        {
            return 13;
        }
        else if (!up && !upRight && right && downRight && down && downLeft && left && upLeft)
        {
            return 10;
        }
        else if (!up && upRight && right && downRight && down && downLeft && left && !upLeft)
        {
            return 6;
        }
        else if (up && upRight && right && !downRight && !down && !downLeft && !left)
        {
            return 11;
        }
        else if (!up && !upRight && right && downRight && down && !left && !upLeft)
        {
            return 0;
        }
        else if (!up && !upRight && !right && down && downLeft && left && !upLeft)
        {
            return 2;
        }
        else if (up && upRight && right && downRight && !down && left && upLeft)
        {
            return enableCeilingMods ? 16 : 8;
        }
        else if (up && upRight && right && !down && downLeft && left && upLeft)
        {
            return enableCeilingMods ? 15 : 8;
        }
        else if (up && upRight && right && down && downLeft && left && !upLeft)
        {
            return 7;
        }
        else if (up && !right && !downRight && !down && left && upLeft)
        {
            return 12;
        }
        else if (up && upRight && right && !down && left && upLeft)
        {
            return 8;
        }
        else if (!up && !upRight && right && left && !upLeft)
        {
            return 1;
        }
        else if (up && down && !left)
        {
            return 3;
        }
        else if (up && down && !right)
        {
            return 5;
        }
        //Debug.LogWarning("Could not find any appropriate sprite for this condition. Applying default...");
        return 4;
    }
    
#if UNITY_EDITOR

    [MenuItem("Assets/Create/Cave Tile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Cave Tile", "New Cave Tile", "Asset", "Save Cave Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CaveTile>(), path);
    }
#endif
}
