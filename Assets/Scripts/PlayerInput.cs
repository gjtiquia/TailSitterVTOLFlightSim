using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Deadzone")]
    [SerializeField] float deadzoneSize = 0.01f;

    PlayerControls controls;
    public float throttle { get; private set; }
    public float yaw { get; private set; }
    public float pitch { get; private set; }
    public float roll { get; private set; }
    public bool levelSwitch { get; private set; }

    void Awake()
    {
        controls = new PlayerControls();

        levelSwitch = false;

        // Callback Funtion with Lambda expression, ctx == context
        controls.Gameplay.Throttle.performed += ctx => Throttle(ctx);
        controls.Gameplay.Yaw.performed += ctx => Yaw(ctx);
        controls.Gameplay.Pitch.performed += ctx => Pitch(ctx, true);
        controls.Gameplay.Roll.performed += ctx => Roll(ctx);
        controls.Gameplay.Reset.performed += ctx =>
        {
            transform.position = new Vector3(-1.6f, 2f, 10.51f);
            transform.eulerAngles = new Vector3(0, 146.78f, 0);
        };
        controls.Gameplay.LevelSwitch.performed += ctx => LevelSwitch(ctx);
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

        if (Mathf.Abs(yaw) < deadzoneSize) yaw = 0;
    }

    private void Pitch(InputAction.CallbackContext ctx, bool invert = false)
    {
        pitch = ctx.ReadValue<float>();
        if (invert)
        {
            pitch *= -1;
        }

        if (Mathf.Abs(pitch) < deadzoneSize) pitch = 0;
    }

    private void Roll(InputAction.CallbackContext ctx, bool invert = false)
    {
        roll = ctx.ReadValue<float>();
        if (invert)
        {
            roll *= -1;
        }

        if (Mathf.Abs(roll) < deadzoneSize) roll = 0;
    }

    private void LevelSwitch(InputAction.CallbackContext ctx)
    {
        var levelOn = ctx.ReadValue<float>();
        if (levelOn == 1)
        {
            levelSwitch = true;
        }
        else if (levelOn == -1)
        {
            levelSwitch = false;
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
