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

    public static List<Vector3> GetPositionsAround(Vector3 center, float minDist, float maxDist, float positionsCount)
    {
        List<Vector3> positions = new List<Vector3>();
        minDist = Mathf.Clamp(minDist, 0, maxDist);
        maxDist = Mathf.Clamp(maxDist, minDist, float.PositiveInfinity);
        for (int i = 0; i < positionsCount; i++)
        {
            float angle = i * (360f / positionsCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.right;
            Vector3 pos = center + dir * UnityEngine.Random.Range(minDist, maxDist);
            positions.Add(pos);
        }

        return positions;
    }
}
