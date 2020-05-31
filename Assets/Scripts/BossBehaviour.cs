using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehaviour : MonoBehaviour
{
    private List<Transform> holes = new List<Transform>();
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float health;
    [Header("Weapons")]
    public List<BossBall> Balls = new List<BossBall>();
    public ParticleSystem boomSystem;
    public ParticleSystem deathExplosion;

    [Header("Boss Type")]
    public GameManager.Environment environment;

    private float timer;
    private bool isCameraStopped;
    void Start()
    {
        Transform holeHolder = transform.Find("Holes");
        for(int i = 0; i < holeHolder.childCount; i++)
        {
            holes.Add(holeHolder.GetChild(i));
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }


    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 1)
        {
            Shoot(2);
            timer = 0;
        }

        if (!isCameraStopped)
        {
            if(Boundary.visibleWorldMax.x >= transform.position.x - 0.1f * spriteRenderer.bounds.size.x)
            {
                GameManager.StopCamera();
                isCameraStopped = true;
            }
        }
    }

    private void Shoot(int count)
    {
        Vector2 position = Vector2.positiveInfinity;
        for (int i = 0; i < holes.Count; i++)
        {
            if (Mathf.Abs(holes[i].position.y - EnemyManager.playerPosition.y) 
                < Mathf.Abs(position.y - EnemyManager.playerPosition.y))
            {
                position = holes[i].position;
            }
        }
        int decider = Random.Range(0, Balls.Count);

        BossBall ball = Instantiate(Balls[decider], position, Quaternion.identity);

        Instantiate(boomSystem, position, Quaternion.identity);

        ball.Move(-4);

        for(int i = 1; i < count; i++)
        {
            Vector2 position1 = holes[Random.Range(0, holes.Count)].position;

            int decider1 = Random.Range(0, Balls.Count);

            BossBall ball1 = Instantiate(Balls[decider1], position1, Quaternion.identity);

            Instantiate(boomSystem, position1, Quaternion.identity);

            ball1.Move(Random.Range(-2, -5));
        }
    }

    public void TakeOrders(Vector2 position, Vector3 scale, float sizeX, float health)
    {
        transform.position = position;

        transform.localScale = scale;

        this.health = health;
    }

    public void TakeDamage()
    {
        health -= 0.5f;

        if(health <= 0 && enabled)
        {
            enabled = false;

            EnemyManager.BossDefeated();

            for(int i = 0; i < 5; i++)
            {
                StartCoroutine(FlashToDeath(i * 0.22f));
            }

            Instantiate(deathExplosion, transform.position, Quaternion.identity);

            Destroy(gameObject, 1.2f);
        }
        else if(enabled)
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        for (int i = 0; i < 15; i++)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, 0.25f);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }

    private IEnumerator FlashToDeath(float wait)
    {
        yield return new WaitForSeconds(wait);
        spriteRenderer.color = Color.black;
        for (int i = 0; i < 15; i++)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, 0.25f);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }
}
