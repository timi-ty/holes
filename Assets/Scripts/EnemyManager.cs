using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerBehaviour player;
    public static Vector2 playerPosition;
    
    void Update()
    {
        playerPosition = player.transform.position;
    }
}
