using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    [Header("Background Manager")]
    public BackgroundManager backgroundManager;

    [Header("Foreground Manager")]
    public ForegroundManager foregroundManager;

    [Header("Enemy Manager")]
    public EnemyManager enemyManager;

    private StoryManager storyManager;

    public enum Environment { Desert, Snow };

    private const float gameAcceleration = 0.1f / 40;
    private const float levelDuration = 80;
    [Header("GameSpeed")]
    public float _gameSpeed;

    public static float gameSpeed { get => gameManager._gameSpeed; private set { gameManager._gameSpeed = value; }}
    public static float enemySpawnRPM;
    public static int tileIndex;
    public static int tileCount;
    public static bool levelUpPending;
    public static bool bossFightPending;
    public static bool bossFightInProgress;
    public static bool isScreenOcluded;

    private static float gameSpeedBeforeBoss;

    private int _score;
    private int score
    {
        get => _score; 
        set
        {
            _score = value;
            scoreText.SetText(score.ToString());
        }
    }
    [Header("Score Text")]
    public TextMeshProUGUI scoreText;

    private float gameTimer;
    private int game_level;
    

    void Start()
    {
        gameTimer = 0;

        game_level = 0;

        score = 0;

        gameManager = this;

        storyManager = GetComponent<StoryManager>();

        GameLevelUp();

        //Invoke("PrepareForBossFight", 10);
    }

    void Update()
    {
        gameTimer += bossFightInProgress ? 0 : Time.deltaTime;

        gameSpeed += bossFightInProgress ? 0 : gameAcceleration * Time.deltaTime;

        if (gameTimer >= levelDuration * game_level)
        {
            GameLevelUp();
            gameTimer = 0;
        }
    }

    private void GameLevelUp()
    {
        game_level++;

        tileIndex = (game_level - 1) % tileCount;

        levelUpPending = game_level > 1 ? true : false;

        switch (game_level)
        {
            case 1:
                gameSpeed = 0.8f;
                enemySpawnRPM = 10;
                break;
            case 2:
                enemySpawnRPM = 12;
                break;
        }
    }

    private void PlayerLevelUp()
    {
        
    }

    private void PrepareForBossFight()
    {
        //if (game_level > 1)
        //{
            bossFightPending = true;
        //}
    }

    public static void StartBossFight(Environment bossEnvironment)
    {
        gameSpeedBeforeBoss = gameSpeed;
        bossFightPending = false;
        bossFightInProgress = true;
        gameManager.enemyManager.SpawnBoss(bossEnvironment);
    }

    public static void FinishBossFight()
    {
        gameSpeed = gameSpeedBeforeBoss;
        bossFightInProgress = false;

        BumpScore(250);
    }

    public static void BumpScore(int bump)
    {
        gameManager.score += bump;
    }

    public static void StopCamera()
    {
        gameSpeed = 0;
    }

    public static void StartEnvironmentTransition()
    {
        //Disable enemy spawner
        //Disable obstacle spawner
        //Set player to auto-pilot

        isScreenOcluded = true;

        gameManager.foregroundManager.StartTransition();
        
        gameManager.storyManager.ShowStoryBlock(gameManager.game_level);
    }

    public static void EndEnvironmentTransition(Environment newEnvironment)
    {
        gameManager.foregroundManager.FinishTransition();
        SetWeather(newEnvironment);
        SetBackground(newEnvironment);
        levelUpPending = false;
    }

    public static void TransitionConcluded()
    {
        //Enable enemy spawner
        //Enable obstacle spawner
        //Set player to normal controls
        isScreenOcluded = false;
    }

    private static void SetWeather(Environment activeEnvironment)
    {
        switch (activeEnvironment)
        {
            case Environment.Snow:
                gameManager.foregroundManager.ChangeToSnowy();
                break;
            default:
                gameManager.foregroundManager.ChangeToClear();
                break;
        }
    }

    private static void SetBackground(Environment activeEnvironment)
    {
        gameManager.backgroundManager.SetBackground(activeEnvironment);
    }
}
