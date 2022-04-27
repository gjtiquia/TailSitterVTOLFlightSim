using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField]
    GameObject plane;

    float initialDistance = 0;
    float initialFOV = 60;
    float endFOV = 1f;

    void UpdateFieldOfView()
    {
        float currentDistance = Vector3.Distance(transform.position, plane.transform.position);
        float ratio = (currentDistance - initialDistance) / initialDistance;

        float newFOV = initialFOV - (initialFOV - endFOV) * Mathf.Sqrt(ratio);

        gameObject.GetComponent<Camera>().fieldOfView = newFOV;
    }

    private void Start()
    {
        initialDistance = Vector3.Distance(transform.position, plane.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(plane.GetComponent<Transform>());
        //UpdateFieldOfView();
    }
}
