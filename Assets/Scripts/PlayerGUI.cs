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

    [SerializeField] GameObject localVelocityObject;
    Text localVelocityText;

    [SerializeField] GameObject angleOfAttackObject;
    Text angleOfAttackText;

    [SerializeField] GameObject liftForceObject;
    Text liftForceText;

    [SerializeField] GameObject inducedLiftObject;
    Text inducedLiftText;

    [SerializeField] GameObject inducedDragObject;
    Text inducedDragText;

    private void Start()
    {
        thrustText = thrustObject.GetComponent<Text>();
        playerInputText = playerInputObject.GetComponent<Text>();
        localRotationText = localRotationObject.GetComponent<Text>();
        angularRatesText = angularRatesObject.GetComponent<Text>();
        localVelocityText = localVelocityObject.GetComponent<Text>();
        angleOfAttackText = angleOfAttackObject.GetComponent<Text>();
        liftForceText = liftForceObject.GetComponent<Text>();
        inducedLiftText = inducedLiftObject.GetComponent<Text>();
        inducedDragText = inducedDragObject.GetComponent<Text>();
    }

    void FixedUpdate()
    {
        thrustText.text = "Thrust: " + plane.thrust;


        playerInputText.text =
            "Throttle: " + playerInput.throttle +
            ", Pitch: " + playerInput.pitch +
            ", Roll: " + playerInput.roll +
            ", Yaw: " + playerInput.yaw +
            ", Level: " + playerInput.levelSwitch
            ;

        localRotationText.text = "Local Rotation: " + plane.localRotation;

        angularRatesText.text = "Angular Rates: " + new Vector3(plane.x_rate, plane.y_rate, plane.z_rate);

        localVelocityText.text = "Local Velocity: " + plane.localVelocity;

        angleOfAttackText.text = "Angle of Attack (deg): " + Mathf.Rad2Deg * plane.angleOfAttack;

        liftForceText.text = "Total Lift: " + plane.totalLift;

        inducedLiftText.text = "Induced Lift: " + plane.inducedLift;

        inducedDragText.text = "Induced Drag: " + plane.inducedDrag;
    }
}
