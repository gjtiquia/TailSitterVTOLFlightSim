using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField]
    GameObject plane;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(plane.GetComponent<Transform>());
    }
}
