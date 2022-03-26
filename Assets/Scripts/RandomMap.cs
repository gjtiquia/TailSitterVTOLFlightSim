using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMap : MonoBehaviour
{
    [SerializeField] float xMin;
    [SerializeField] float xMax;
    [SerializeField] float zMin;
    [SerializeField] float zMax;

    [SerializeField] int numberOfCubes = 100;

    [SerializeField] GameObject cubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfCubes; i++)
        {
            var x = Random.Range(xMin, xMax);
            var z = Random.Range(zMin, zMax);

            var pos = new Vector3(x, 500, z);
            var rotation = Quaternion.identity;

            Instantiate(cubePrefab, pos, rotation);
        }
    }
}
