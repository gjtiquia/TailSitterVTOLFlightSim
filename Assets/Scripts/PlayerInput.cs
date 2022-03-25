using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Rigidbody rgbd;
    [SerializeField] float thrust;
    [SerializeField] float pitchDeg;
    [SerializeField] float rollDeg;
    [SerializeField] float yawDeg;

    PlayerControls controls;
    float throttle;
    float yaw;
    float pitch;
    float roll;

    void Awake()
    {
        controls = new PlayerControls();

        // Callback Funtion with Lambda expression, ctx == context
        controls.Gameplay.Throttle.performed += ctx => Throttle(ctx);
        controls.Gameplay.Throttle.canceled += ctx => Debug.Log("Cancelled");
        controls.Gameplay.Yaw.performed += ctx => Yaw(ctx);
        controls.Gameplay.Pitch.performed += ctx => Pitch(ctx, true);
        controls.Gameplay.Roll.performed += ctx => Roll(ctx, true);
        controls.Gameplay.Reset.performed += ctx =>
        {
            transform.position = new Vector3(-1.6f, 2f, 10.51f);
            transform.eulerAngles = new Vector3(0, 146.78f, 0);
        };
    }

    private void Throttle(InputAction.CallbackContext ctx)
    {
        // Map [-1,1] => [0, 1]
        throttle = (ctx.ReadValue<float>() + 1) / 2;
    }

    private void Yaw(InputAction.CallbackContext ctx, bool invert = false)
    {
        yaw = ctx.ReadValue<float>();
        if (invert)
        {
            yaw *= -1;
        }
    }

    private void Pitch(InputAction.CallbackContext ctx, bool invert = false)
    {
        pitch = ctx.ReadValue<float>();
        if (invert)
        {
            pitch *= -1;
        }
    }

    private void Roll(InputAction.CallbackContext ctx, bool invert = false)
    {
        roll = ctx.ReadValue<float>();
        if (invert)
        {
            roll *= -1;
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        // Roll, Pitch, Yaw
        transform.Rotate(
            pitch * pitchDeg * Time.fixedDeltaTime,
            yaw * yawDeg * Time.fixedDeltaTime,
            roll * rollDeg * Time.fixedDeltaTime,
            Space.Self
        );

        // Thrust
        Vector3 totalThrust = transform.up * Physics.gravity.magnitude * thrust * throttle * Time.fixedDeltaTime;
        rgbd.AddForce(totalThrust);

        Debug.Log("Throttle: " + totalThrust.magnitude);
    }
}
