using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkNode : MonoBehaviour
{
    HingeJoint2D nodeJoint;

    public int resilience { get; set; }
    public bool isBroken { get; private set; }

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
                    isBroken = true;
                    SwingingObstacle swingingObstacle = GetComponentInParent<SwingingObstacle>();
                    if (swingingObstacle) swingingObstacle.BreakChain();
                    Destroy(nodeJoint);
                }
            }
        }
    }
}
