using UnityEngine;
using UnityEngine.Splines;

public class PIDLaneFollower : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;
    public Rigidbody rb;

    [Header("PID Parameters")]
    public float Kp = 0.7f;
    public float Ki = 0f;
    public float Kd = 0.6f;

    [Header("Vehicle Parameters")]
    public float vehicleMass = 1000f;
    public float vehicleSpeed = 6.5f;

    [Header("Tracking Settings")]
    public int searchSteps = 800;
    public float tangentStepT = 0.015f;
    public float steeringSensitivity = 0.18f;
    public float steeringResponse = 2.6f;

    [Header("Smoothing")]
    public float steeringSmoothness = 5f;

    [Header("Live Values")]
    public float error_e;
    public float controllerOutput_u;
    public float closestT;

    private float integral;
    private float previousError;
    private float smoothedControl;

    private bool lastPidActive = true;
    private Vector3 lockedStraightDirection;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.mass = vehicleMass;
        rb.drag = 0f;
        rb.angularDrag = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        lockedStraightDirection = transform.forward;
        lockedStraightDirection.y = 0f;
        lockedStraightDirection.Normalize();

        closestT = FindClosestTOnSpline();
    }

    void FixedUpdate()
    {
        if (splineContainer == null || rb == null)
            return;

        rb.mass = vehicleMass;

        bool pidActive =
            Mathf.Abs(Kp) + Mathf.Abs(Ki) + Mathf.Abs(Kd) > 0.0001f;

        if (!pidActive)
        {
            if (lastPidActive)
            {
                lockedStraightDirection = rb.rotation * Vector3.forward;
                lockedStraightDirection.y = 0f;

                if (lockedStraightDirection.magnitude < 0.001f)
                    lockedStraightDirection = transform.forward;

                lockedStraightDirection.Normalize();
            }

            lastPidActive = false;

            error_e = 0f;
            controllerOutput_u = 0f;
            integral = 0f;
            previousError = 0f;
            smoothedControl = 0f;

            rb.MovePosition(
                rb.position + lockedStraightDirection * vehicleSpeed * Time.fixedDeltaTime
            );

            return;
        }

        lastPidActive = true;

        closestT = FindClosestTOnSpline();

        Vector3 centerPoint = splineContainer.EvaluatePosition(closestT);

        float nextT = Mathf.Clamp01(closestT + tangentStepT);
        Vector3 nextPoint = splineContainer.EvaluatePosition(nextT);

        Vector3 pathForward = nextPoint - centerPoint;
        pathForward.y = 0f;

        if (pathForward.magnitude < 0.001f)
            return;

        pathForward.Normalize();

        Vector3 pathRight = Vector3.Cross(Vector3.up, pathForward).normalized;

        Vector3 carToCenter = rb.position - centerPoint;
        carToCenter.y = 0f;

        error_e = Vector3.Dot(carToCenter, pathRight);

        float dt = Time.fixedDeltaTime;

        integral += error_e * dt;

        float derivative = (error_e - previousError) / dt;

        controllerOutput_u =
            -(Kp * error_e + Ki * integral + Kd * derivative);

        previousError = error_e;

        smoothedControl = Mathf.Lerp(
            smoothedControl,
            controllerOutput_u,
            steeringSmoothness * dt
        );

        float speedSteeringBoost = Mathf.Clamp(vehicleSpeed / 6f, 1f, 1.8f);

        Vector3 desiredDirection =
            pathForward +
            pathRight * smoothedControl * steeringSensitivity * speedSteeringBoost;

        desiredDirection.y = 0f;

        if (desiredDirection.magnitude > 0.001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(desiredDirection.normalized, Vector3.up);

            float massEffect = 1000f / Mathf.Max(vehicleMass, 100f);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    desiredRotation,
                    steeringResponse * massEffect * dt
                )
            );
        }

        Vector3 moveDirection = rb.rotation * Vector3.forward;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        rb.MovePosition(
            rb.position + moveDirection * vehicleSpeed * dt
        );
    }

    private float FindClosestTOnSpline()
    {
        float bestT = 0f;
        float bestDistance = float.MaxValue;

        for (int i = 0; i <= searchSteps; i++)
        {
            float t = i / (float)searchSteps;
            Vector3 point = splineContainer.EvaluatePosition(t);

            float distance = Vector3.Distance(rb.position, point);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestT = t;
            }
        }

        return bestT;
    }
}