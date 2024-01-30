using System.Collections.Generic;
using UnityEngine;

public class MathUtility
{
    public static Vector3 EvaluateSlerp(Vector3 start, Vector3 end, float t, float centerOffset = 0f)
    {
        Vector3 centerPivot = (start + end) * 0.5f;

        centerPivot -= new Vector3(0, -centerOffset);

        Vector3 startRelativeCenter = start - centerPivot;
        Vector3 endRelativeCenter = end - centerPivot;

        return Vector3.Slerp(startRelativeCenter, endRelativeCenter, t) + centerPivot;
    }
}
