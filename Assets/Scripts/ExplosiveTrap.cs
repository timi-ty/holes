using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{
    private float _coolDownTimer;
    private bool _isHot;
    private Rigidbody2D explosiveBody;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public bool isHot
    {
        get => _isHot; 
        set
        {
            _isHot = value;
            if (isHot)
            {
                _coolDownTimer = 2;
            }
        }
    }

    void Start()
    {
        explosiveBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        transform.tag = "Explosive";
    }

    void Update()
    {
        if(_coolDownTimer > 0)
        {
            _coolDownTimer -= Time.deltaTime;
        }
        else if(_coolDownTimer <= 0)
        {
            isHot = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            explosiveBody.AddForce(1.5f * collision.transform.right, ForceMode2D.Impulse);
            isHot = true;
        }
    }
}
