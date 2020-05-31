using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerBehaviour player;
    public static Vector2 playerPosition;
    private EnemySpawner enemySpawner;


    private void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
    }

    void Update()
    {
        if(player) playerPosition = player.transform.position;
    }

    public void SpawnBoss(GameManager.Environment bossEnvironment)
    {
        enemySpawner.SpawnBoss(bossEnvironment);
    }

    public static void BossDefeated()
    {
        GameManager.FinishBossFight();
    }

    public static void KilledByPlayer(EnemyBehaviour enemy, bool skillfulKill)
    {
        if (enemy.transform)
        {
            GameManager.BumpScore(skillfulKill ? 40 : 25);
        }
    }
}
