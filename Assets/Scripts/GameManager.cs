using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float gameSpeed;
    public static float enemySpawnRPM;
    void Start()
    {
        gameSpeed = 0.8f;
        enemySpawnRPM = 10;
    }

    void Update()
    {
        
    }
}
