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
    public float shootRPM;
    public Color bulletColor;
    public int maxOverdrive;
    //public GameObject sheild;
    private int overDrive;
    public bool isInEditorMode;
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
            ApplyInertia(Vector2.right * Time.fixedDeltaTime * GameManager.gameSpeed);
        }

        playerBody.MoveRotation(angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("EnemyProjectile"))// && !sheild.activeSelf)
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
        if (collision.transform.CompareTag("Explosive"))// && !sheild.activeSelf)
        {
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

    private void ApplyInertia(Vector2 deltaPos)
    {
        Boundary playerBounds = Boundary.ScreenBoundary(playerSize);

        Vector2 position = playerBody.position + deltaPos;

        Vector2 clampedNewPos = new Vector2(Mathf.Clamp(position.x,
                                            playerBounds.leftBound, playerBounds.rightBound),
                                            Mathf.Clamp(position.y,
                                            playerBounds.downBound, playerBounds.upBound));

        playerBody.MovePosition(clampedNewPos);

        angle = Mathf.LerpAngle(angle, 0, Time.fixedDeltaTime * 5);
    }

    IEnumerator Shoot()
    {
        //sheild.SetActive(false);
        propeller.speed = shootRPM * 2;
        if (isInEditorMode)
        {
            while (Input.GetMouseButton(0))
            {
                Quaternion sporadic = Quaternion.Euler(0, 0, Random.value * 2);
                bulletPool.ShootBullet(playerGun.position, playerGun.rotation * sporadic, bulletSpeed: 10, hitSpark);
                shotSpark.transform.SetPositionAndRotation(playerGun.position, playerGun.rotation);
                shotSpark.Play();
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
        //sheild.SetActive(true);
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
        Destroy(gameObject);
    }
}