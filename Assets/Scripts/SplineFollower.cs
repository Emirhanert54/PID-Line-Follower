using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;

    [Header("Movement")]
    public float speed = 6f;
    public float turnSpeed = 4f;

    [Header("Path Tracking")]
    public float lookAheadT = 0.03f;
    public int searchSteps = 150;

    private float currentT = 0f;

    void Update()
    {
        if (splineContainer == null)
        {
            return;
        }

        currentT = FindClosestTOnSpline();

        float targetT = currentT + lookAheadT;

        if (targetT > 1f)
        {
            targetT = 1f;
        }

        Vector3 targetPosition = splineContainer.EvaluatePosition(targetT);

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private float FindClosestTOnSpline()
    {
        float closestT = 0f;
        float closestDistance = float.MaxValue;

        for (int i = 0; i <= searchSteps; i++)
        {
            float t = i / (float)searchSteps;

            Vector3 point = splineContainer.EvaluatePosition(t);

            float distance = Vector3.Distance(transform.position, point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }
}