using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerBehaviour player;
    public static Vector2 playerPosition;
    
    void Update()
    {
        if(player) playerPosition = player.transform.position;
    }

    public static void KilledByPlayer(EnemyBehaviour enemy, bool skillfulKill)
    {
        if (enemy.transform)
        {
            GameManager.BumpScore(skillfulKill ? 40 : 25);
        }
    }
}
