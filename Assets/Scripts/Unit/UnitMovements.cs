using System.Collections.Generic;
using UnityEngine;

public class UnitMovements : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinder;
    [SerializeField] private UnitGeneral unit; 
    private UnitStats unitStats;

    private void OnEnable()
    {
        unitStats = unit.GetStats();
    }

    private void FixedUpdate()
    {
        List<Vector3> currentPath = pathfinder.GetCurrentPath();

        if (currentPath == null) return;
        if (currentPath.Count == 0) return;

        int currentWaypointIndex = pathfinder.GetCurrentWaypointIndex();

        transform.position = Vector3.MoveTowards(transform.position, currentPath[currentWaypointIndex], 
            unitStats.moveSpeed * Time.fixedDeltaTime);
    }

    public void GoTo(Vector3 _pos)
    {
        pathfinder.SetTarget(_pos);
    }

    public void StopGoTo()
    {
        pathfinder.StopPathfiding();
    }
}
