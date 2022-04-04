using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Plane : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Rigidbody rgbd;

    [Header("Physical Parameters")]
    [SerializeField] float maxThrust = 8000;

    [Header("Hover Flight")]
    [SerializeField] float maxHoverPitchRate = 720;
    [SerializeField] float maxHoverRollRate = 720;
    [SerializeField] float maxHoverYawRate = 540;
    [SerializeField] float hoverExpo = 0.69f;
    [SerializeField] float hoverExpoYaw = 0.69f;
    [SerializeField] float hoverSuperExpo = 0.7f;
    [SerializeField] float hoverSuperExpoYaw = 0.7f;

    [Header("Level Flight")]
    [SerializeField] float maxLevelPitchRate = 90;
    [SerializeField] float maxLevelRollRate = 90;
    [SerializeField] float maxLevelYawRate = 45;

    [Header("Lift")]
    [SerializeField] float liftPower;
    [SerializeField] AnimationCurve liftAOACurve;
    [SerializeField] float inducedDragPower;
    [SerializeField] AnimationCurve inducedDragCurve;

    public bool hoverMode => playerInput.levelSwitch == false;
    public Vector3 thrust { get; private set; }
    public Vector3 torque { get; private set; }
    public Vector3 localRotation { get; private set; }
    public Vector3 velocity { get; private set; }
    public Vector3 localVelocity { get; private set; }
    public float angleOfAttack { get; private set; }
    public Vector3 totalLift { get; private set; }
    public Vector3 inducedLift { get; private set; }
    public Vector3 inducedDrag { get; private set; }

    public float x_input { get; private set; }
    public float y_input { get; private set; }
    public float z_input { get; private set; }

    public float x_maxRate { get; private set; }
    public float y_maxRate { get; private set; }
    public float z_maxRate { get; private set; }

    public float x_rate { get; private set; }
    public float y_rate { get; private set; }
    public float z_rate { get; private set; }

    void CalculateState()
    {
        var invRotation = Quaternion.Inverse(rgbd.rotation);
        velocity = rgbd.velocity;
        localVelocity = invRotation * velocity;  //transform world velocity into local space
        localRotation = transform.localEulerAngles;
        CalculateAngleOfAttack();
    }

    void CalculateAngleOfAttack()
    {
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

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
        inducedLift = liftDirection * liftForce;
        //induced drag varies with square of lift coefficient
        var dragForce = liftCoefficient * liftCoefficient;
        var dragDirection = -liftVelocity.normalized;
        inducedDrag = dragDirection * v2 * dragForce * inducedDragPower * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));
        return inducedLift + inducedDrag;
    }

    void UpdateMode()
    {
        if (hoverMode)
        {
            x_input = playerInput.pitch;
            y_input = playerInput.roll;
            z_input = playerInput.yaw;

            x_maxRate = maxHoverPitchRate;
            y_maxRate = maxHoverRollRate;
            z_maxRate = maxHoverYawRate;
        }
        else
        {
            x_input = playerInput.pitch;
            y_input = playerInput.yaw;
            z_input = playerInput.roll;

            x_maxRate = maxLevelPitchRate;
            y_maxRate = maxLevelRollRate;
            z_maxRate = maxLevelYawRate;
        }
    }

    void UpdateThrust(float dt)
    {
        thrust = playerInput.throttle * maxThrust * dt * Vector3.forward;
        rgbd.AddRelativeForce(thrust);
    }

    void UpdateAngle(float dt)
    {
        if (hoverMode)
        {
            // Hover Flight Stick Input Mapping
            // y = 𝑟(𝑥^3+𝑥(1−𝑓))(1−𝑔)/(1−𝑔|𝑥|)
            //
            // y = rate [-max,max]
            // x = input [-1, 1]
            // r = max
            // f = exponential factor
            // g = super exponential factor

            // X (Pitch)
            var r = x_maxRate;
            var x = x_input;
            var f = hoverExpo;
            var g = hoverSuperExpo;
            x_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));

            // Y (Roll)
            r = y_maxRate;
            x = y_input;
            f = hoverExpo;
            g = hoverSuperExpo;
            y_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));

            // Z (Yaw)
            r = z_maxRate;
            x = z_input;
            f = hoverExpoYaw;
            g = hoverSuperExpoYaw;
            z_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));
        }

        else
        {
            // Level Flight Stick Input Mapping
            // linear mapping [-1, 1] => [-r, r]

            // Pitch
            x_rate = x_input * x_maxRate;

            // Yaw
            y_rate = y_input * y_maxRate;

            // Roll
            z_rate = z_input * z_maxRate * -1;
        }

        transform.Rotate(Vector3.right, x_rate * dt);
        transform.Rotate(Vector3.up, y_rate * dt);
        transform.Rotate(Vector3.forward, z_rate * dt);
    }

    void UpdateLift(float dt)
    {
        var liftForce = CalculateLift(
            angleOfAttack, Vector3.right,
            liftPower,
            liftAOACurve,
            inducedDragCurve
        );

        totalLift = liftForce * dt;

        rgbd.AddRelativeForce(totalLift);
    }

    private void FixedUpdate()
    {
        var dt = Time.deltaTime;

        CalculateState();

        // Map player input to corresponding axis
        // Map correct parameters to corresponding flight mode
        UpdateMode();

        UpdateThrust(dt);
        UpdateLift(dt);
        UpdateAngle(dt);

        CalculateState();
    }
}
