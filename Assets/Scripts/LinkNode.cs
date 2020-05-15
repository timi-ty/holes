using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkNode : MonoBehaviour
{
    HingeJoint2D nodeJoint;
    private int _resilience;
    private bool _isBroken;

    public int resilience { get => _resilience; set => _resilience = value; }
    public bool isBroken { get => _isBroken;}

    private void Start()
    {
        nodeJoint = GetComponent<HingeJoint2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            resilience--;
            if (resilience <= 0)
            {
                if (nodeJoint)
                {
                    _isBroken = true;
                    SwingingObstacle swingingObstacle = GetComponentInParent<SwingingObstacle>();
                    if (swingingObstacle) swingingObstacle.BreakChain();
                    Destroy(nodeJoint);
                }
            }
        }
    }
}
