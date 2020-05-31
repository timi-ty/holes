using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector2 playerSize;
    private Propeller propeller;
    public Transform playerGun;
    public ObjectPool bulletPool;
    public ParticleSystem shotSpark;
    public ParticleSystem hitSpark;
    public ParticleSystem deathExplosion;
    [Range(800, 1500)] //I determined the RPM range for optimal experience and performance 
    public float shootRPM;
    public Color bulletColor;
    public int maxOverdrive;
    public bool isInEditorMode;
    private int overDrive;
    private Vector2 autoPilotTarget;
    private float angle;
    private float health = 3;
    public Image healthBar;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerBody = GetComponent<Rigidbody2D>();

        playerSize = spriteRenderer.bounds.extents;

        shotSpark = Instantiate(shotSpark, transform);

        hitSpark = Instantiate(hitSpark, transform);

        bulletPool.GenerateBullets(bulletColor);

        bulletPool.GenerateMasks();

        shootRPM = Mathf.Clamp(shootRPM, 0.1f, Mathf.Infinity);

        originalColor = spriteRenderer.color;

        propeller = GetComponentInChildren<Propeller>();
        propeller.speed = shootRPM / 2;

        autoPilotTarget = playerBody.position;
    }

    void Update()
    {
        if (isInEditorMode)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (overDrive <= maxOverdrive)
                    {
                        StartCoroutine(Shoot());
                        overDrive++;
                    }
                    lastPos = pos;
                }
                else
                {
                    isUnderControl = true;
                }
            }
            else isUnderControl = false;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 pos = touch.position;
                pos = Camera.main.ScreenToWorldPoint(pos);
                if (touch.phase == TouchPhase.Began)
                {
                    if (overDrive <= maxOverdrive)
                    {
                        StartCoroutine(Shoot());
                        overDrive++;
                    }
                    lastPos = pos;
                }
                else
                {
                    isUnderControl = true;
                }
            }
            else isUnderControl = false;
        }
    }

    bool isUnderControl;
    private void FixedUpdate()
    {
        if (isUnderControl)
        {
            Touch touch = isInEditorMode ? new Touch() : Input.GetTouch(0);
            Vector2 pos = isInEditorMode ? (Vector2) Input.mousePosition : touch.position;
            pos = Camera.main.ScreenToWorldPoint(pos);
            MovePlayer(pos);
        }
        else
        {
            AutoPilot();
        }

        playerBody.MoveRotation(angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("EnemyProjectile"))
        {
            StartCoroutine(FlashRed());
            health--;
            switch (health)
            {
                case 0: 
                    healthBar.fillAmount = 0;
                    break;
                case 1:
                    healthBar.fillAmount = 0.2f;
                    break;
                case 2:
                    healthBar.fillAmount = 0.8f;
                    break;
                case 3:
                    healthBar.fillAmount = 1;
                    break;
            }
            if(health <= 0)
            {
                Die();
            }
        }
        if (collision.transform.CompareTag("Explosive"))
        {
            if(collision.rigidbody)
                collision.rigidbody.AddForce(collision.GetContact(0).normal * -20, ForceMode2D.Impulse);
            Die();
        }
    }

    Vector2 lastPos;
    private void MovePlayer(Vector2 pos)
    {
        Boundary playerBounds = Boundary.ScreenBoundary(playerSize);

        Vector2 deltaPos = pos - lastPos;

        Vector2 position = playerBody.position + deltaPos;

        Vector2 clampedNewPos = new Vector2(Mathf.Clamp(position.x,
                                            playerBounds.leftBound, playerBounds.rightBound),
                                            Mathf.Clamp(position.y,
                                            playerBounds.downBound, playerBounds.upBound));

        playerBody.MovePosition(clampedNewPos);

        lastPos = pos;

        Vector2 normalToPlayer = Vector2.Perpendicular(transform.right);
        Vector2 projection = Vector3.Project(deltaPos, normalToPlayer);
        float deltaAngle = 45 * projection.magnitude * (Vector2.Angle(normalToPlayer, projection) == 0 ? 1 : -1);
        angle += deltaAngle;

        angle = (angle % 360) + (angle < 0 ? 360 : 0);
    }

    private void ApplyInertia(float deltaX, float newY)
    {
        Boundary playerBounds = Boundary.ScreenBoundary(playerSize);

        Vector2 newPos = new Vector2(playerBody.position.x + deltaX, newY);

        Vector2 clampedNewPos = new Vector2(Mathf.Clamp(newPos.x,
                                            playerBounds.leftBound, playerBounds.rightBound),
                                            Mathf.Clamp(newPos.y,
                                            playerBounds.downBound, playerBounds.upBound));

        playerBody.MovePosition(clampedNewPos);

        angle = Mathf.LerpAngle(angle, 0, Time.fixedDeltaTime * 5);
    }

    private void AutoPilot()
    {
        float deltaX = Time.fixedDeltaTime * GameManager.gameSpeed;

        if (!GameManager.isScreenOcluded)
        {
            ApplyInertia(deltaX, playerBody.position.y);
            return;
        }

        int layerMask = LayerMask.GetMask("Tilemap");

        RaycastHit2D rayHitUp = Physics2D.Raycast(playerBody.position, Vector2.up, Boundary.visibleWorldSize.y, layerMask);
        RaycastHit2D rayHitDown = Physics2D.Raycast(playerBody.position, Vector2.down, Boundary.visibleWorldSize.y, layerMask);

        if (rayHitUp.transform && rayHitUp.point.y - playerBody.position.y < spriteRenderer.bounds.size.y * 0.6f)
        {
            autoPilotTarget.y -= Time.fixedDeltaTime * GameManager.gameSpeed;
        }
        if (rayHitDown.transform && playerBody.position.y - rayHitDown.point.y < spriteRenderer.bounds.size.y * 0.6f)
        {
            autoPilotTarget.y += Time.fixedDeltaTime * GameManager.gameSpeed;
        }

        float newY = Mathf.Lerp(playerBody.position.y, autoPilotTarget.y, Time.fixedDeltaTime * 5 * GameManager.gameSpeed);

        ApplyInertia(deltaX, newY);

        Vector2 raycastOrigin = new Vector2(playerBody.position.x + spriteRenderer.bounds.size.x, Boundary.visibleWorldMin.y);
        RaycastHit2D forwardRay = Physics2D.Raycast(playerBody.position + new Vector2(spriteRenderer.bounds.size.x, 0), Vector2.right);
        bool isBlocked = forwardRay.transform && Mathf.Abs(forwardRay.point.x - playerBody.position.x) < 4 * spriteRenderer.bounds.size.x;
        bool needsNewTarget = playerBody.position.x > autoPilotTarget.x - spriteRenderer.bounds.size.x;
        if (isBlocked && needsNewTarget)
        {
            forwardRay = Physics2D.Raycast(raycastOrigin, Vector2.right);
        }
        else return;
        while (raycastOrigin.y <= Boundary.visibleWorldMax.y)
        {
            raycastOrigin += Vector2.up * 0.1f;

            RaycastHit2D tempFwrRay = Physics2D.Raycast(raycastOrigin, Vector2.right);

            if (tempFwrRay.transform && tempFwrRay.point.x > forwardRay.point.x)
            {
                forwardRay = tempFwrRay;
            }
            else if (!tempFwrRay.transform)
            {
                autoPilotTarget = new Vector2(Boundary.visibleWorldMax.x, raycastOrigin.y) + Vector2.up * spriteRenderer.bounds.extents.y;
                return;
            }
        }
        if (forwardRay.transform && forwardRay.point.x > playerBody.position.x)
        {
            autoPilotTarget = forwardRay.point + Vector2.up * spriteRenderer.bounds.extents.y;
        }
    }

    IEnumerator Shoot()
    {
        propeller.speed = shootRPM * 2;
        if (isInEditorMode)
        {
            while (Input.GetMouseButton(0))
            {
                Quaternion sporadic = Quaternion.Euler(0, 0, Random.value * 2);
                bulletPool.ShootBullet(playerGun.position, playerGun.rotation * sporadic, bulletSpeed: 10, hitSpark);
                shotSpark.transform.SetPositionAndRotation(playerGun.position, playerGun.rotation);
                shotSpark.Play();
                CameraDirector.CreateVibration(CameraDirector.VibrationLevel.Light, 60.0f / shootRPM);
                yield return new WaitForSeconds(60.0f / shootRPM);
            }
            overDrive--;
        }
        else
        {
            while (Input.touchCount > 0)
            {
                Quaternion sporadic = Quaternion.Euler(0, 0, Random.value * 2);
                bulletPool.ShootBullet(playerGun.position, playerGun.rotation * sporadic, bulletSpeed: 10, hitSpark);
                shotSpark.transform.SetPositionAndRotation(playerGun.position, playerGun.rotation);
                shotSpark.Play();
                yield return new WaitForSeconds(60.0f / shootRPM);
            }
            overDrive--;
        }

        propeller.speed = shootRPM / 2;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        for (int i = 0; i < 60; i++)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, 0.0625f);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        healthBar.fillAmount = 0;
        deathExplosion = Instantiate(deathExplosion);
        deathExplosion.transform.position = transform.position;
        deathExplosion.Play();
        CameraDirector.CreateVibration(CameraDirector.VibrationLevel.Heavy, 0.25f);
        Destroy(gameObject);
    }
}