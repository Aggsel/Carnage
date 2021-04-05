using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///             V 1.0
///             Made by Carl for testing purposes (grenades, bullet drop)
///             2021-04-02
/// </summary>
public class TrajectoryPrediction : MonoBehaviour
{
    [Range(0.1f, 0.6f)]
    [SerializeField] private float intervals = 0.1f;
    [SerializeField] private float incline = 12f;
    [SerializeField] private float reach = 10f;
    [Range(0.1f, 0.8f)]
    [SerializeField] private float rayStopDist = 0.1f;

    private List<Vector3> generatedPoints = new List<Vector3>();

    private void Update ()
    {
        CurveSimulation();
    }

    //Try to calculate a trajectory preditction curve
    private void CurveSimulation()
    {
        generatedPoints = PointGeneration();

        for (int i = 0; i < generatedPoints.Count; i++)
        {
            if (i + 1 < generatedPoints.Count)
            {
                Debug.DrawLine(generatedPoints[i], generatedPoints[i + 1], Color.blue);
            }
        }
    }

    //collision check function
    private bool CollisionCheck(Vector3 pos)
    {
        Collider[] col = Physics.OverlapSphere(pos, rayStopDist);

        if (col.Length > 0)
        {
            return true;
        }

        return false;
    }

    //Generate the points in 3d-space
    private List<Vector3> PointGeneration()
    {
        List<Vector3> points = new List<Vector3>();

        float maxDuration = 5f;
        float timeStepInterval = intervals;
        int maxSteps = (int)(maxDuration / timeStepInterval);
        Vector3 directionVector = Camera.main.transform.forward;
        Vector3 launchPosition = Camera.main.transform.position + Camera.main.transform.forward;

        float vel = incline * reach * Time.fixedDeltaTime;

        for (int i = 0; i < maxSteps; i++)
        {
            //f(y) = (x0 + x*t, y0 + y*t - 9.81t^2/2)
            Vector3 calculatedPosition = launchPosition + directionVector * vel * i * timeStepInterval;
            calculatedPosition.y += Physics.gravity.y / 2 * Mathf.Pow(i * timeStepInterval, 2);

            points.Add(calculatedPosition);

            //check if the newly added position collides with something, then stop adding additional positions
            if (CollisionCheck(calculatedPosition))
            {
                break;
            }
        }

        return points;
    }
}