using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector2 playerSize;
    private Vector2 shootOffset;
    public ObjectPool bulletPool;
    public ParticleSystem shotSpark;
    public ParticleSystem hitSpark;
    public float shootRPM;
    public int maxOverdrive;
    public GameObject sheild;
    private int overDrive;
    public bool isInEditorMode;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerBody = GetComponent<Rigidbody2D>();

        playerSize = spriteRenderer.bounds.extents;

        shotSpark = Instantiate(shotSpark, transform);

        hitSpark = Instantiate(hitSpark, transform);

        bulletPool.GenerateBullets(spriteRenderer.color);

        bulletPool.GenerateMasks();

        shootRPM = Mathf.Clamp(shootRPM, 0.1f, Mathf.Infinity);

        originalColor = spriteRenderer.color;

        shootOffset = new Vector2(spriteRenderer.bounds.extents.x + spriteRenderer.bounds.size.x * 0.4f, 0);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("EnemyProjectile") && !sheild.activeSelf)
        {
            StartCoroutine(FlashRed());
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
    }

    IEnumerator Shoot()
    {
        sheild.SetActive(false);
        if (isInEditorMode)
        {
            while (Input.GetMouseButton(0))
            {
                float direction = Random.value * 2.0f - 1.0f;
                bulletPool.ShootBullet(playerBody.position + shootOffset, Quaternion.Euler(0, 0, direction), bulletSpeed: 10, hitSpark);
                shotSpark.transform.SetPositionAndRotation(playerBody.position + shootOffset, Quaternion.Euler(0, 0, direction));
                shotSpark.Play();
                yield return new WaitForSeconds(60.0f / shootRPM);
            }
            overDrive--;
        }
        else
        {
            while (Input.touchCount > 0)
            {
                float direction = Random.value * 2.0f - 1.0f;
                bulletPool.ShootBullet(playerBody.position + shootOffset, Quaternion.Euler(0, 0, direction), bulletSpeed: 10, hitSpark);
                shotSpark.transform.SetPositionAndRotation(playerBody.position + shootOffset, Quaternion.Euler(0, 0, direction));
                shotSpark.Play();
                yield return new WaitForSeconds(60.0f / shootRPM);
            }
            overDrive--;
        }
        sheild.SetActive(true);
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
}