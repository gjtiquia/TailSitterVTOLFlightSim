using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Plane : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Rigidbody rgbd;
    [SerializeField] float maxThrust = 8000;
    [SerializeField] float pitchDeg = 100;
    [SerializeField] float rollDeg = 100;
    [SerializeField] float yawDeg = 100;

    [Header("Drag")]
    [SerializeField] float dragForward = 1;
    [SerializeField] float dragBack = 1;
    [SerializeField] float dragLeft = 1;
    [SerializeField] float dragRight = 1;
    [SerializeField] float dragTop = 1;
    [SerializeField] float dragBottom = 1;
    [SerializeField] Vector3 angularDrag;

    [Header("Lift")]
    [SerializeField] float liftPower;
    [SerializeField] AnimationCurve liftAOACurve;
    [SerializeField] float inducedDrag;
    [SerializeField] AnimationCurve inducedDragCurve;

    [Header("Steering")]
    [SerializeField] Vector3 turnSpeed;
    [SerializeField] Vector3 turnAcceleration;
    [SerializeField] AnimationCurve steeringCurve;

    public Vector3 velocity { get; private set; }
    public Vector3 localVelocity { get; private set; }
    public Vector3 localAngularVelocity { get; private set; }
    public float angleOfAttack { get; private set; }


    //void FixedUpdate()
    //{
    //    // Roll, Pitch, Yaw
    //    transform.Rotate(
    //        playerInput.pitch * pitchDeg * Time.fixedDeltaTime,
    //        playerInput.yaw * yawDeg * Time.fixedDeltaTime,
    //        playerInput.roll * rollDeg * Time.fixedDeltaTime,
    //        Space.Self
    //    );

    //    // Thrust
    //    Vector3 totalThrust = transform.up * Physics.gravity.magnitude * maxThrust * playerInput.throttle * Time.fixedDeltaTime;
    //    rgbd.AddForce(totalThrust);
    //}

    void CalculateState(float dt)
    {
        var invRotation = Quaternion.Inverse(rgbd.rotation);
        velocity = rgbd.velocity;
        localVelocity = invRotation * velocity;  //transform world velocity into local space
        localAngularVelocity = invRotation * rgbd.angularVelocity;  //transform into local space
        CalculateAngleOfAttack();
    }

    void DebugState()
    {
        Debug.Log("AoA: " + angleOfAttack);
    }

    void CalculateAngleOfAttack()
    {
        // Original:
        //angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

        // Modified:
        angleOfAttack = Mathf.Atan2(localVelocity.z, localVelocity.y);
    }

    void UpdateThrust()
    {
        rgbd.AddRelativeForce(playerInput.throttle * maxThrust * Vector3.up);
    }

    void UpdateDrag()
    {
        var lv = localVelocity;
        var lv2 = lv.sqrMagnitude;  //velocity squared
                                    //calculate coefficient of drag depending on direction on velocity
        var coefficient = Utilities.Scale6(
            lv.normalized,
            dragRight, dragLeft,
            dragTop, dragBottom,
            dragForward, dragBack
        );

        var drag = coefficient.magnitude * lv2 * -lv.normalized;    //drag is opposite direction of velocity
        rgbd.AddRelativeForce(drag);
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve)
    {
        var liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);    //project velocity onto YZ plane
        var v2 = liftVelocity.sqrMagnitude;                                     //square of velocity
                                                                                //lift = velocity^2 * coefficient * liftPower
                                                                                //coefficient varies with AOA
        var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        var liftForce = v2 * liftCoefficient * liftPower;
        //lift is perpendicular to velocity
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        var lift = liftDirection * liftForce;
        //induced drag varies with square of lift coefficient
        //var dragForce = liftCoefficient * liftCoefficient;
        //var dragDirection = -liftVelocity.normalized;
        //var inducedDrag = dragDirection * v2 * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));
        //return lift + inducedDrag;
        return lift;
    }

    Vector3 UpdateLift()
    {
        var liftForce = CalculateLift(
            angleOfAttack, Vector3.right,
            liftPower,
            liftAOACurve,
            inducedDragCurve
        );

        rgbd.AddRelativeForce(liftForce);
        return liftForce;
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        var error = targetVelocity - angularVelocity;
        var accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }

    void UpdateSteering(float dt)
    {
        var speed = Mathf.Max(0, localVelocity.z);
        var steeringPower = steeringCurve.Evaluate(speed);

        Vector3 controlInput = new Vector3(playerInput.pitch, playerInput.roll, playerInput.yaw);
        var targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower);
        var av = localAngularVelocity * Mathf.Rad2Deg;

        var correction = new Vector3(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
        );

        rgbd.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);    //ignore rigidbody mass
    }

    void UpdateAngularDrag()
    {
        var av = localAngularVelocity;
        var drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        rgbd.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    void FixedUpdate()
    {
        float dt = Time.deltaTime;

        // Calculate States
        CalculateState(dt);

        // Debug
        //DebugState();

        // Apply Updates
        UpdateThrust();
        Vector3 liftForce = UpdateLift();
        //UpdateSteering(dt);
        //UpdateDrag();

        // Debug
        //Debug.Log("Lift force: " + liftForce);

        // Calculate again for other systems to read the new state
        CalculateState(dt);
    }
}
