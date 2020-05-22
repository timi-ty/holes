using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private float bulletSpeed;
    private ParticleSystem hitSpark;
    private void Start()
    {
        Disable();
    }

    void Update()
    {
        transform.position += transform.right * Time.deltaTime * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitSpark)
        {
            hitSpark.transform.position = transform.position;
            hitSpark.Play();
        }
        if(!transform.CompareTag("Projectile") || !collision.transform.CompareTag("Player"))
        {
            Disable();
        }
    }

    public void Shoot(Vector3 position, Quaternion rotation, float bulletSpeed, ParticleSystem hitSpark)
    {
        transform.SetPositionAndRotation(position, rotation);
        this.bulletSpeed = bulletSpeed;
        this.hitSpark = hitSpark;

        Invoke("Disable", 2);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
