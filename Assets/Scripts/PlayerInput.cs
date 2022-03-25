using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerControls controls;
    public float throttle { get; private set; }
    public float yaw { get; private set; }
    public float pitch { get; private set; }
    public float roll { get; private set; }

    void Awake()
    {
        controls = new PlayerControls();

        // Callback Funtion with Lambda expression, ctx == context
        controls.Gameplay.Throttle.performed += ctx => Throttle(ctx);
        controls.Gameplay.Yaw.performed += ctx => Yaw(ctx);
        controls.Gameplay.Pitch.performed += ctx => Pitch(ctx, true);
        controls.Gameplay.Roll.performed += ctx => Roll(ctx, true);
        controls.Gameplay.Reset.performed += ctx =>
        {
            transform.position = new Vector3(-1.6f, 2f, 10.51f);
            transform.eulerAngles = new Vector3(0, 146.78f, 0);
        };
    }

    public void Throttle(InputAction.CallbackContext ctx)
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
}
