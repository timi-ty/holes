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

    public enum Environment { Desert, Snow };

    private const float gameAcceleration = 0.1f / 40;
    private const float levelDuration = 40;

    public static float gameSpeed;
    public static float enemySpawnRPM;
    public static int tileIndex;
    public static int tileCount;
    public static bool levelUpPending;

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
    private int level;

    void Start()
    {
        gameTimer = 0;

        level = 0;

        score = 0;

        gameManager = this;

        LevelUp();
    }

    void Update()
    {
        gameTimer += Time.deltaTime;

        gameSpeed += gameAcceleration * Time.deltaTime;

        if (gameTimer >= levelDuration * level)
        {
            LevelUp();
            gameTimer = 0;
        }
    }

    private void LevelUp()
    {
        level++;

        tileIndex = (level - 1) % tileCount;

        levelUpPending = level > 1 ? true : false;

        switch (level)
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

    public static void BumpScore(int bump)
    {
        gameManager.score += bump;
    }

    public static void StartEnvironmentTransition()
    {
        gameManager.foregroundManager.StartTransition();
    }

    public static void EndEnvironmentTransition(Environment newEnvironment)
    {
        gameManager.foregroundManager.FinishTransition();
        SetWeather(newEnvironment);
        SetBackground(newEnvironment);
        levelUpPending = false;
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
