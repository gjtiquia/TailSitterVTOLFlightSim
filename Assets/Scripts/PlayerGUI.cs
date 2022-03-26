using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] FixedWing fixedWing;

    [SerializeField]

    // Update is called once per frame
    void FixedUpdate()
    {

        string thrust = "Thrust: " + fixedWing.thrust;
        string torque = "Steering Torque: " + fixedWing.steeringTorque.ToString("F5");


        
    }
}
