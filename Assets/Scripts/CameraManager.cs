using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    const float sizeFactor = 1.333333333f;
    void OnEnable()
    {
        int perfectAspectRatio = 2;
        float deviation = perfectAspectRatio - Camera.main.aspect;
        float deltaSize = deviation * sizeFactor;
        Camera.main.orthographicSize += deltaSize;
    }

    void FixedUpdate()
    {
        transform.position += Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;
    }
}
