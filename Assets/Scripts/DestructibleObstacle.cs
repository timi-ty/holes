using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Vector2 fragmentSize;
    private int brittlenessFactor;
    private Sprite damageMask;
    private Vector3 damageMaskScale;
    public ObjectPool spriteMaskPool;
    public float fragmentHeight;
    public ParticleSystem damageDebris;
    void Start()
    {
        if(transform.localScale != Vector3.one)
        {
            Debug.LogWarning("DestructibleObstacle transform should have a scale of 1 to function correctly.");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();

        ScaleDamageMask();

        CreateFragments();
    }


    void Update()
    {
        if (spriteRenderer.bounds.max.x < Boundary.visibleWorldMin.x - (Boundary.visibleWorldSize.x * 0.1f))
        {
            spriteMaskPool.ReturnMasks(transform);
            Destroy(gameObject);
        }
    }

    int lumpDamage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        BoxCollider2D fragmentBox = (BoxCollider2D) collision.otherCollider;
        float deltaOffset = 0.0125f / transform.lossyScale.x;
        float deltaSize = 0.025f / transform.lossyScale.x;
        if (collision.collider.CompareTag("Projectile"))
        {
            for(int i = 0; i < brittlenessFactor; i++)
            {
                fragmentBox.offset += new Vector2(deltaOffset, 0);
                fragmentBox.size -= new Vector2(deltaSize, 0);

                Vector2 damagePoint = Vector2.Scale(fragmentBox.offset -
                    new Vector2(fragmentBox.size.x / 2, 0), transform.lossyScale) + (Vector2)transform.position;
                Quaternion damageRotation = Quaternion.Euler(0, 0, (Random.value * 60) - 30);

                if (fragmentBox.size.x <= 0.003f)
                {
                    damagePoint.x = spriteRenderer.bounds.max.x;
                    lumpDamage = 999;
                    Destroy(fragmentBox);
                }

                SpriteMask mask = spriteMaskPool.GetMask(transform);
                if (mask && lumpDamage >= 5)
                {
                    mask.sprite = damageMask;
                    mask.transform.localScale = damageMaskScale;
                    if (lumpDamage == 999)
                        damagePoint.x -= mask.bounds.extents.x;
                    mask.transform.SetPositionAndRotation(damagePoint, damageRotation);

                    lumpDamage = 0;
                }

                damageDebris.transform.SetPositionAndRotation(damagePoint, damageRotation);
                damageDebris.Play();

                if (fragmentBox.size.x <= 0.003f)
                {
                    Destroy(fragmentBox);
                }

                if(brittlenessFactor > 0)
                    lumpDamage++;
            }
        }
    }

    private void CreateFragments()
    {
        int fragmentCount = Mathf.CeilToInt(spriteRenderer.bounds.size.y / fragmentHeight);
        Vector2 fragmentEdge = Vector3.Scale(spriteRenderer.bounds.extents,
            new Vector3(1 / transform.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z));
        float unscaledFragmentHeight = fragmentHeight / transform.lossyScale.y; 
        fragmentSize = new Vector2(fragmentEdge.x * 2, unscaledFragmentHeight);
        for (int i = 0; i < fragmentCount; i++)
        {
            if (i < fragmentCount - 1)
            {
                BoxCollider2D fragment = gameObject.AddComponent<BoxCollider2D>();
                fragment.size = fragmentSize;
                fragment.offset = new Vector2(0, -fragmentEdge.y + (unscaledFragmentHeight * i) + (unscaledFragmentHeight * 0.5f));
            }
            else
            {
                BoxCollider2D fragment = gameObject.AddComponent<BoxCollider2D>();
                fragment.size = fragmentSize;
                fragment.offset = new Vector2(0, fragmentEdge.y - (unscaledFragmentHeight * 0.5f));
            }
        }
    }

    private void ScaleDamageMask()
    {
        float maskHeight = damageMask.bounds.size.y;
        float scale = (fragmentHeight * 1.1f) / maskHeight;

        damageMaskScale = Vector3.one * scale;
        damageMaskScale = Vector3.Scale(damageMaskScale,
            new Vector3(1 / transform.lossyScale.x,
            1 / transform.lossyScale.y,
            1 / transform.lossyScale.z) );
    }

    public void Modify(Sprite sprite, float? fragmentHeight, int? brittlenessFactor,
        Sprite damageMask, ParticleSystem damageDebris, Vector2? obstacleSize)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        if (fragmentHeight.HasValue)
        {
            this.fragmentHeight = fragmentHeight.Value;
        }
        if(brittlenessFactor.HasValue)
        {
            this.brittlenessFactor = brittlenessFactor.Value;
        }
        if (damageMask)
        {
            this.damageMask = damageMask;
        }
        if (damageDebris)
        {
            this.damageDebris = damageDebris;
        }
        if (obstacleSize.HasValue)
        {
            spriteRenderer.size = obstacleSize.Value;
        }
    }
}
