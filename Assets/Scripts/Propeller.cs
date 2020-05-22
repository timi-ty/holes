using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    public float speed;
    
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, -speed * Time.deltaTime);
    }
}
