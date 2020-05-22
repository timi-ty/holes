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
    private float coolDownTimer
    {
        get => _coolDownTimer; 
        set
        {
            _coolDownTimer = value;
            if (_coolDownTimer <= 0)
            {
                _coolDownTimer = 0;
                isHot = false;
            }
        }
    }

    public bool isHot
    {
        get => _isHot; 
        set
        {
            _isHot = value;
            if (isHot)
            {
                _coolDownTimer = 2;
                //StartCoroutine(FlashRed());
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
        coolDownTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            explosiveBody.AddForce(1.5f * collision.transform.right, ForceMode2D.Impulse);
            isHot = true;
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        int frames = (int) (2 / Time.deltaTime);
        for (int i = 0; i < frames; i++)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }
}
