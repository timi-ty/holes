using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public ObjectPool bulletPool;
    public float shootRPM;
    public Sprite bulletSprite;
    private Vector2 shootOffset;
    public ParticleSystem shotSpark;
    public ParticleSystem hitSpark;
    public ParticleSystem deathExplosion;

    public float moveSpeed;
    public float moveAmplitude;
    private Rigidbody2D enemyBody;
    private float yCentre;
    private float angle;

    private Vector2 target;
    private Vector2 size;

    private float health;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        Bounds spriteBounds = spriteRenderer.bounds;
        size = spriteBounds.size;
        shootOffset = new Vector2(-(spriteBounds.extents.x + spriteBounds.size.x * 0.15f), 0);
        enemyBody = GetComponent<Rigidbody2D>();
        yCentre = transform.position.y;
        target = transform.position;
        shootRPM = Mathf.Clamp(shootRPM, 0.1f, Mathf.Infinity);

        shotSpark = Instantiate(shotSpark, transform);

        hitSpark = Instantiate(hitSpark, transform);

        InvokeRepeating("Shoot", 1, 60.0f / shootRPM);

        health = 5;
    }

    int dir = -1;
    void FixedUpdate()
    {
        float inertia = Mathf.Abs((enemyBody.position.y - yCentre) / moveAmplitude);
        float angleVel = 45 * (1 / moveSpeed) * moveSpeed + (135 * (1 / moveSpeed) * moveSpeed * (1 - inertia));
        angle += angleVel * Time.fixedDeltaTime;
        angle %= 360;

        if (enemyBody.position.x - EnemyManager.playerPosition.x < size.x) dir = 3;
        if (enemyBody.position.x > Boundary.visibleWorldMax.x) dir = -1;

        float yPos = yCentre + Mathf.Sin(Mathf.Deg2Rad * angle) * moveAmplitude;
        float xPos = enemyBody.position.x + Time.fixedDeltaTime * moveSpeed * dir;

        Vector2 newPosition = new Vector2(xPos, yPos);
        enemyBody.MovePosition(newPosition);

        AvoidObstacles();

        if (transform.position.x + size.x < Boundary.visibleWorldMin.x)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            health -= 0.85f;
            StartCoroutine(FlashRed());
            if (health <= 0)
            {
                Die();
            }
        }
    }

    void AvoidObstacles()
    {
        yCentre = Mathf.Lerp(yCentre, target.y, Time.deltaTime * moveSpeed);

        Vector2 raycastOrigin = new Vector2(enemyBody.position.x, Boundary.visibleWorldMin.y) + shootOffset;
        RaycastHit2D forwardRay = Physics2D.Raycast(enemyBody.position + shootOffset, Vector2.left);
        bool isBlocked = forwardRay.transform && Mathf.Abs(forwardRay.point.x - enemyBody.position.x) < 2 * size.x;
        bool needsNewTarget = enemyBody.position.x < target.x + size.x;
        if (isBlocked && needsNewTarget)
        {
            //Debug.Log("Blocked By " + forwardRay.transform + " at: " + forwardRay.point);
            forwardRay = Physics2D.Raycast(raycastOrigin, Vector2.left);
        }
        else return;
        while (raycastOrigin.y <= Boundary.visibleWorldMax.y)
        {
            raycastOrigin += Vector2.up * 0.1f;

            RaycastHit2D tempFwrRay = Physics2D.Raycast(raycastOrigin, Vector2.left);

            if (tempFwrRay.transform && tempFwrRay.point.x < forwardRay.point.x)
            {
                forwardRay = tempFwrRay;
            }
        }
        if (forwardRay.transform && forwardRay.point.x < enemyBody.position.x)
        {
            target = forwardRay.point;
            //Debug.Log("New Target " + forwardRay.transform + " at: " + forwardRay.point);
        }
    }

    void Shoot()
    {
        bulletPool.ShootEnemyBullet(enemyBody.position + shootOffset, Quaternion.identity, bulletSpeed: -10, hitSpark, bulletSprite);
        shotSpark.transform.SetPositionAndRotation(enemyBody.position + shootOffset, Quaternion.identity);
        shotSpark.Play();
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        for (int i = 0; i < 15; i++)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, 0.25f);
            yield return null;
        }
    }

    private void Die()
    {
        deathExplosion = Instantiate(deathExplosion);
        deathExplosion.transform.position = transform.position;
        deathExplosion.Play();
        Destroy(gameObject);
    }

    public void TakeOrders(Vector2 position)
    {
        transform.position = position;
    }
}
