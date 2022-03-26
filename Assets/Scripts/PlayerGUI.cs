using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Plane plane;

    [SerializeField] GameObject playerInputObject;
    Text playerInputText;

    [SerializeField] GameObject thrustObject;
    Text thrustText;

    [SerializeField] GameObject localRotationObject;
    Text localRotationText;

    [SerializeField] GameObject angularRatesObject;
    Text angularRatesText;

    private void Start()
    {
        thrustText = thrustObject.GetComponent<Text>();
        playerInputText = playerInputObject.GetComponent<Text>();
        localRotationText = localRotationObject.GetComponent<Text>();
        angularRatesText = angularRatesObject.GetComponent<Text>();
    }

    void FixedUpdate()
    {
        thrustText.text = "Thrust: " + plane.thrust;


        playerInputText.text =
            "Throttle: " + playerInput.throttle +
            ", Pitch: " + playerInput.pitch +
            ", Roll: " + playerInput.roll +
            ", Yaw: " + playerInput.yaw;

        localRotationText.text = "Local Rotation: " + plane.localRotation;

        angularRatesText.text = "Angular Rates: " + new Vector3(plane.x_rate, plane.y_rate, plane.z_rate);
    }
}
