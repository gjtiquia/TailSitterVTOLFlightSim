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
    float hoverExpoYaw = 0.69f;
    [SerializeField] float hoverSuperExpo = 0.7f;
    float hoverSuperExpoYaw = 0.7f;

    [Header("Level Flight")]
    [SerializeField] float maxLevelPitchRate = 90;
    [SerializeField] float maxLevelRollRate = 90;
    [SerializeField] float maxLevelYawRate = 45;

    public bool hoverMode { get; private set; }
    public Vector3 thrust { get; private set; }
    public Vector3 torque { get; private set; }
    public Vector3 localRotation { get; private set; }

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
        localRotation = transform.localEulerAngles;
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
        thrust = playerInput.throttle * maxThrust * Time.deltaTime * Vector3.forward;
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

            // X
            var r = x_maxRate;
            var x = x_input;
            var f = hoverExpo;
            var g = hoverSuperExpo;
            x_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));

            // Y
            r = y_maxRate;
            x = y_input;
            f = hoverExpo;
            g = hoverSuperExpo;
            y_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));

            // Z
            r = z_maxRate;
            x = z_input;
            f = hoverExpo;
            g = hoverSuperExpo;
            z_rate = r * (Mathf.Pow(x, 3) + x * (1 - f)) * (1 - g) / (1 - g * Mathf.Abs(x));
        }

        transform.Rotate(Vector3.right, x_rate * dt);
        transform.Rotate(Vector3.up, y_rate * dt);
        transform.Rotate(Vector3.forward, z_rate * dt);
    }

    private void Start()
    {
        hoverMode = true;
    }

    private void FixedUpdate()
    {
        var dt = Time.deltaTime;

        CalculateState();

        // Map player input to corresponding axis
        // Map correct parameters to corresponding flight mode
        UpdateMode();

        UpdateThrust(dt);
        UpdateAngle(dt);

    }
}
