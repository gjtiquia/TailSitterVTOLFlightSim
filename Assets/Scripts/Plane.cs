using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Plane : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] private Rigidbody rgbd;
    [SerializeField] float thrust = 8000;
    [SerializeField] float pitchDeg = 100;
    [SerializeField] float rollDeg = 100;
    [SerializeField] float yawDeg = 100;

    private void FixedUpdate()
    {
        // Roll, Pitch, Yaw
        transform.Rotate(
            playerInput.pitch * pitchDeg * Time.fixedDeltaTime,
            playerInput.yaw * yawDeg * Time.fixedDeltaTime,
            playerInput.roll * rollDeg * Time.fixedDeltaTime,
            Space.Self
        );

        // Thrust
        Vector3 totalThrust = transform.up * Physics.gravity.magnitude * thrust * playerInput.throttle * Time.fixedDeltaTime;
        rgbd.AddForce(totalThrust);
    }
}
