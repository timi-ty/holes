using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ForegroundManager : MonoBehaviour
{
    public enum Weather { Clear, Rainy, Snowy }
    //public Weather weather;
    public ParticleSystem snowSystem;
    public ParticleSystem rainSystem;
    public ParticleSystem fogSystem;
    public TransitionWall transitionWall;
    private bool isInTransition;
    private List<Transform> cloudEdges = new List<Transform>();
    Vector3 cloudPosition;

    void Start()
    {
        ParticleSystem.ShapeModule shape;

        shape = snowSystem.shape;
        shape.radius = Boundary.visibleWorldExtents.x;

        shape = rainSystem.shape;
        shape.radius = Boundary.visibleWorldExtents.x;

        shape = fogSystem.shape;
        shape.radius = Boundary.visibleWorldExtents.y;


        cloudPosition = new Vector3(0, Boundary.visibleWorldMax.y);

        fogSystem.transform.position = new Vector3(Boundary.visibleWorldMax.x + Boundary.visibleWorldExtents.x, Boundary.visibleWorldCentre.y);

        cloudEdges.Add(snowSystem.transform);
        cloudEdges.Add(rainSystem.transform);
    }

    void FixedUpdate()
    {
        cloudPosition += Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;

        for (int i = 0; i < cloudEdges.Count; i++)
        {
            cloudEdges[i].position = cloudPosition;
        }

        fogSystem.transform.position += Vector3.right * Time.fixedDeltaTime * GameManager.gameSpeed;

        if (!isInTransition)
        {
            transitionWall.HideWall();
        }
    }

    public void StartTransition()
    {
        transitionWall.ShowWall();

        isInTransition = true;
    }

    public void FinishTransition()
    {
        isInTransition = false;
    }

    public void ChangeToRainy()
    {
        StopAllPrecipitation();
        rainSystem.gameObject.SetActive(true);
        rainSystem.Play();
        //weather = Weather.Rainy;
    }

    public void ChangeToSnowy()
    {
        StopAllPrecipitation();
        snowSystem.gameObject.SetActive(true);
        snowSystem.Play();
        //weather = Weather.Snowy;
    }

    public void ChangeToClear()
    {
        StopAllPrecipitation();
        EndFog();
    }

    public void CreateFog()
    {
        fogSystem.gameObject.SetActive(true);

        ParticleSystem.EmissionModule emission;
        emission = fogSystem.emission;
        emission.enabled = true;

        fogSystem.Play();
    }

    public void EndFog()
    {
        ParticleSystem.EmissionModule emission;
        emission = fogSystem.emission;
        emission.enabled = false;

        fogSystem.gameObject.SetActive(false);
    }

    private void StopAllPrecipitation()
    {
        snowSystem.Pause();
        rainSystem.Pause();

        snowSystem.gameObject.SetActive(false);
        rainSystem.gameObject.SetActive(false);
    }
}
