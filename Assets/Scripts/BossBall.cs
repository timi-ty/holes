using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBall : MonoBehaviour
{
    private float speed;
    public ParticleSystem hitEffect;
    void Start()
    {

    }

    private void Update()
    {
        transform.position += (Vector3) Vector2.right * speed * Time.deltaTime;

        if(transform.position.x < Boundary.visibleWorldMin.x - Boundary.visibleWorldSize.x * 0.2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void Move(float speed)
    {
        this.speed = speed;
    }
}
