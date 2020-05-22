using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPool : ScriptableObject
{
    public int maskCount;
    public int bulletCount;
    public BulletBehaviour bulletPrefab;
    public BulletBehaviour enemyBulletPrefab;
    GameObject spriteMaskHolder;
    GameObject bulletHolder;
    GameObject enemyBulletHolder;

    public void GenerateMasks()
    {
        if (!spriteMaskHolder)
        {
            spriteMaskHolder = new GameObject();
            spriteMaskHolder.name = "Sprite Mask Pool";
        }

        int existingCount = spriteMaskHolder.transform.childCount;

        for (int i = 0; i < maskCount; i++)
        {
            GameObject maskObject = new GameObject();
            SpriteMask spriteMask = maskObject.AddComponent<SpriteMask>();
            spriteMask.name = "Sprite Mask " + (i + 1 + existingCount);
            spriteMask.transform.SetParent(spriteMaskHolder.transform);
        }

        Debug.Log("Generated " + maskCount + " more Sprite masks");
    }

    public void GenerateBullets(Color? color)
    {
        if (!bulletHolder)
        {
            bulletHolder = new GameObject();
            bulletHolder.name = "Bullet Pool";
        }

        int existingCount = bulletHolder.transform.childCount;

        for (int i = 0; i < bulletCount; i++)
        {
            BulletBehaviour bullet = Instantiate(bulletPrefab);

            SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            TrailRenderer trailRenderer = bullet.GetComponent<TrailRenderer>();

            if (color.HasValue)
            {
                spriteRenderer.color = color.Value;
                trailRenderer.startColor = color.Value;
            }

            bullet.name = "Bullet " + (i + 1 + existingCount);
            bullet.transform.SetParent(bulletHolder.transform);
        }

        Debug.Log("Generated " + bulletCount + " more bullets");
    }

    public void GenerateEnemyBullets(Color? color)
    {
        if (!enemyBulletHolder)
        {
            enemyBulletHolder = new GameObject();
            enemyBulletHolder.name = "Enemy Bullet Pool";
        }

        int existingCount = enemyBulletHolder.transform.childCount;

        for (int i = 0; i < bulletCount; i++)
        {
            BulletBehaviour bullet = Instantiate(enemyBulletPrefab);

            SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();

            if (color.HasValue)
            {
                spriteRenderer.color = color.Value;
            }

            bullet.name = "Bullet " + (i + 1 + existingCount);
            bullet.transform.SetParent(enemyBulletHolder.transform);
        }

        Debug.Log("Generated " + bulletCount + " more enemy bullets");
    }

    public void DestroyAllMasks()
    {
        for (int i = 0; i < spriteMaskHolder.transform.childCount; i++)
        {
            DestroyImmediate(spriteMaskHolder.transform.GetChild(i).gameObject);
        }
        DestroyImmediate(spriteMaskHolder);

        Debug.Log("Deleted sprite mask pool");
    }

    public void DestroyAllBullets()
    {
        for (int i = 0; i < bulletHolder.transform.childCount; i++)
        {
            DestroyImmediate(bulletHolder.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < enemyBulletHolder.transform.childCount; i++)
        {
            DestroyImmediate(enemyBulletHolder.transform.GetChild(i).gameObject);
        }
        DestroyImmediate(bulletHolder);

        Debug.Log("Deleted bullet pool");
    }

    public SpriteMask GetMask(Transform userTransform)
    {
        if(spriteMaskHolder.transform.childCount > 0)
        {

            SpriteMask mask = spriteMaskHolder.transform.GetChild(spriteMaskHolder.transform.childCount - 1).GetComponent<SpriteMask>();
            if (mask) mask.transform.SetParent(userTransform);
            else Debug.LogWarning("Failed to obtain sprite mask. Sprite mask pool should only contain SpriteMask children.");
            return mask;
        }
        else
        {
            Debug.LogWarning("Failed to obtain sprite mask. Sprite mask pool empty.");
            return null;
        }
    }

    public void ShootBullet(Vector3 bulletPosition, Quaternion bulletRotation, float bulletSpeed, ParticleSystem hitSpark)
    {
        if (bulletHolder.transform.childCount > 0)
        {

            BulletBehaviour bullet = bulletHolder.transform.GetChild(0).GetComponent<BulletBehaviour>();
            if (bullet && !bullet.gameObject.activeSelf)
            {
                bullet.transform.SetAsLastSibling();
                bullet.gameObject.SetActive(true);
                bullet.Shoot(bulletPosition, bulletRotation, bulletSpeed, hitSpark);
            }
            else Debug.LogWarning("Failed to obtain bullet. Bullet pool should only contain BulletBehaviour children.");
        }
        else
        {
            Debug.LogWarning("Failed to obtain bullet. Bullet pool empty.");
        }
    }

    public void ShootEnemyBullet(Vector3 bulletPosition, Quaternion bulletRotation, float bulletSpeed, ParticleSystem hitSpark, Sprite bulletSprite)
    {
        if (bulletHolder.transform.childCount > 0)
        {

            BulletBehaviour bullet = enemyBulletHolder.transform.GetChild(0).GetComponent<BulletBehaviour>();
            if (bullet && !bullet.gameObject.activeSelf)
            {
                bullet.transform.SetAsLastSibling();
                bullet.gameObject.SetActive(true);
                SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = bulletSprite;
                bullet.Shoot(bulletPosition, bulletRotation, bulletSpeed, hitSpark);
            }
            else Debug.LogWarning("Failed to obtain bullet. Bullet pool should only contain BulletBehaviour children.");
        }
        else
        {
            Debug.LogWarning("Failed to obtain bullet. Bullet pool empty.");
        }
    }

    public void ReturnMasks(Transform userTransform)
    {
        SpriteMask[] recoveredMasks = userTransform.GetComponentsInChildren<SpriteMask>();
        for(int i = 0; i < recoveredMasks.Length; i++)
        {
            recoveredMasks[i].sprite = null;
            recoveredMasks[i].transform.SetParent(spriteMaskHolder.transform);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Object Pool")]
    public static void CreateSpriteMaskPool()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Object Pool", "New Object Pool", "Asset", "Save Object Pool", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ObjectPool>(), path);
    }
#endif
}