using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    //similar to Vector3.Scale, but has separate factor negative values on each axis
    public static Vector3 Scale6(
        Vector3 value,
        float posX, float negX,
        float posY, float negY,
        float posZ, float negZ
    )
    {
        Vector3 result = value;

        if (result.x > 0) result.x *= posX;
        else if (result.x < 0) result.x *= negX;

        if (result.y > 0) result.y *= posY;
        else if (result.y < 0) result.y *= negY;

        if (result.z > 0) result.z *= posZ;
        else if (result.z < 0) result.z *= negZ;

        return result;
    }
}
