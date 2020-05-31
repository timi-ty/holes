using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    const float sizeFactor = 1.333333333f;

    public Camera sceneCamera;
    private static CameraDirector cameraDirector;
    public enum VibrationLevel { Light, Medium, Heavy };

    private List<Vector2> destPoints = new List<Vector2>();
    private Vector2[] currentPath = new Vector2[3];
    [Min(3)]
    public int destPointCount;
    [Range(0.0f, 25.0f)]
    public float vibrationFrequency;
    [Range(0.0f, 0.1f)]
    public float vibrationMagnitude;
    private float[] defaultVibration;

    private Vector3 equilibriumPos;
    private Vector2 deviationFromEquilibrium;

    void OnEnable()
    {
        int perfectAspectRatio = 2;
        float deviation = perfectAspectRatio - sceneCamera.aspect;
        float deltaSize = deviation * sizeFactor;
        sceneCamera.orthographicSize = 5 + deltaSize;

        cameraDirector = this;

        equilibriumPos = sceneCamera.transform.position;

        GenerateDestPoints();

        defaultVibration = new float[] { vibrationFrequency, vibrationMagnitude };
    }

    void FixedUpdate()
    {
        equilibriumPos += Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;
        sceneCamera.transform.position = equilibriumPos + (Vector3)deviationFromEquilibrium;

        VibrateCamera();
    }

    private void VibrateCamera()
    {
        //vibrationMaginitude is the distance applied between any two destPoints. Thus, multiply by destPoint.Count to get total distance for one cycle.
        float speed = vibrationFrequency * vibrationMagnitude * destPoints.Count;
        Vector2 direction = currentPath[2];
        Vector2 delta_pos = direction * speed * Time.fixedDeltaTime;
        deviationFromEquilibrium += delta_pos;

        Vector2 startPoint = currentPath[0];
        Vector2 endPoint = currentPath[1];
        Vector2 currentPoint = deviationFromEquilibrium;

        if((currentPoint - startPoint).magnitude >= (endPoint - startPoint).magnitude)
        {
            startPoint = currentPoint;
            endPoint = destPoints[Random.Range(0, destPoints.Count)] * vibrationMagnitude;

            currentPath[0] = startPoint;
            currentPath[1] = endPoint;
            currentPath[2] = (endPoint - startPoint).normalized;
        }
    }

    private void GenerateDestPoints()
    {
        destPoints.Add(Vector2.zero);
        for(int i = 1; i < destPointCount; i++)
        {
            destPoints.Add(Random.insideUnitCircle);
            Debug.Log(destPoints[i]);
        }

        currentPath[0] = destPoints[0] * vibrationMagnitude;
        currentPath[1] = destPoints[1] * vibrationMagnitude;
        currentPath[2] = (destPoints[1] - destPoints[0]).normalized;

        deviationFromEquilibrium = currentPath[0];
    }

    public static void CreateVibration(VibrationLevel vibration, float t)
    {
        switch (vibration)
        {
            case VibrationLevel.Light:
                cameraDirector.vibrationFrequency = 13;
                cameraDirector.vibrationMagnitude = 0.005f;
                break;
            case VibrationLevel.Medium:
                cameraDirector.vibrationFrequency = 8;
                cameraDirector.vibrationMagnitude = 0.015f;
                break;
            case VibrationLevel.Heavy:
                cameraDirector.vibrationFrequency = 10;
                cameraDirector.vibrationMagnitude = 0.03f;
                break;
        }

        cameraDirector.Invoke("ResetToRestVibration", t);
    }

    private void ResetToRestVibration()
    {
        vibrationFrequency = defaultVibration[0];
        vibrationMagnitude = defaultVibration[1];
    }
}
