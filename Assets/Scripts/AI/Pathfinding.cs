using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Collections;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Vector3 target;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<Tile> tileGrid;
    [SerializeField] private bool enablePathfinding;
    [SerializeField] private bool showCalculationTime;
    [SerializeField] private float pathUpdateTime = 2;

    private int currentWaypointIndex = 0;

    private List<Vector3> currentPath = new List<Vector3>();
    private List<Vector2> obstacles = new List<Vector2>();
    private Guid pathfindingID = System.Guid.NewGuid();

    public event Action OnPathfindingUpdate;

    private void OnEnable()
    {
        StartCoroutine(OnEnableDelay());
    }

    private IEnumerator OnEnableDelay()
    {
        yield return new WaitUntil(() => TerrainManager.Instance != null);

        for (int i = 0; i < TerrainManager.Instance.GetTilesCount(); i++)
        {
            tileGrid.Add(TerrainManager.Instance.GetTile(i));
        }
    }

    private void Update()
    {
        if (!enablePathfinding)
        {
            TimerManager.Cancel("PathFindUpdate" + pathfindingID);
            currentPath.Clear();    
            return;
        }

        if(!TimerManager.StartTimer(pathUpdateTime, "PathFindUpdate" + pathfindingID))
        {
            currentPath = GetPath(new Vector2(transform.position.x, transform.position.z),
                new Vector2(target.x, target.z));
            currentWaypointIndex = 1;
            OnPathfindingUpdate?.Invoke();
        }

        if (currentPath.Count == 0) return;

        if (transform.position == currentPath[currentWaypointIndex] && currentWaypointIndex < currentPath.Count - 1)
            currentWaypointIndex++;

        if (currentWaypointIndex >= currentPath.Count - 1 && transform.position == currentPath[currentPath.Count - 1])
            enablePathfinding = false;
    }

    public void SetTarget(Vector3 _target, bool _enablePathfinding = true)
    {
        target = _target;
        enablePathfinding = _enablePathfinding;
        currentWaypointIndex = 1;
        TimerManager.Cancel("PathFindUpdate" + pathfindingID);
    }

    public void StopPathfiding()
    {
        enablePathfinding = false;
    }

    public List<Vector3> GetCurrentPath()
    { 
        return currentPath; 
    }

    public int GetCurrentWaypointIndex()
    { 
        return currentWaypointIndex;
    }

    private List<Vector3> GetPath(Vector2 currentPos, Vector2 _target)
    {
        List<Vector3> res = new List<Vector3>();

        Vector2Int intCurrentPos = Vector2Int.RoundToInt(currentPos);
        Vector2Int intTarget = Vector2Int.RoundToInt(_target);
        NativeList<int2> walls = new NativeList<int2>(Allocator.TempJob);

        obstacles.Clear();
        foreach(Tile tile in tileGrid)
        {
            Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
            Vector3Int posInt = Vector3Int.RoundToInt(pos);
            if(!tile.GetTileType().walkable)
            {
                walls.Add(new int2(50 + posInt.x - intCurrentPos.x, 50 + posInt.y - intCurrentPos.y));
            }
        }

        float startTime = Time.realtimeSinceStartup;

        NativeList<int2> newPath = new NativeList<int2>(1, Allocator.TempJob);
        NativeList<int> __enablePathfinding = new NativeList<int>(1, Allocator.TempJob) { 1 };
        FindPathJob findPathJob = new FindPathJob {
            startPos = new int2(50 + intTarget.x - intCurrentPos.x, 50 + intTarget.y - intCurrentPos.y),
            endPos = new int2(50, 50),
            wallsList = walls,
            path = newPath,
            _enablePathfinding = __enablePathfinding
        };
        JobHandle jobHandle = findPathJob.Schedule();

        jobHandle.Complete();

        foreach (int2 pos in newPath)
        {
            Vector2 finalPos = currentPos + new Vector2(pos.x - 50, pos.y - 50);
            res.Add(new Vector3(finalPos.x, transform.position.y, finalPos.y));
        }
        enablePathfinding = findPathJob._enablePathfinding[0] == 1 ? true : false;

        newPath.Dispose();
        walls.Dispose();
        __enablePathfinding.Dispose();

        if (showCalculationTime)
        {
            float endTime = Time.realtimeSinceStartup;
            Debug.Log("Pathfindind Time : " + (endTime - startTime) * 1000f);
        }

        if (res.Count >= 2)
        {
            res[0] = transform.position;
            res[res.Count - 1] = target;
        }
        return res;
    }

    [BurstCompile]
    public struct FindPathJob : IJob
    {
        public int2 startPos;
        public int2 endPos;
        public NativeList<int2> wallsList;
        public NativeList<int2> path;
        public NativeList<int> _enablePathfinding;

        public void Execute()
        {
            int2 gridSize = new int2(100, 100);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // Left
            neighbourOffsetArray[1] = new int2(1, 0); //Right
            neighbourOffsetArray[2] = new int2(0, 1); //Up
            neighbourOffsetArray[3] = new int2(0, -1); //Down
            neighbourOffsetArray[4] = new int2(-1, -1); //Down Left
            neighbourOffsetArray[5] = new int2(-1, 1); //Up Left
            neighbourOffsetArray[6] = new int2(1, -1); // Down Right
            neighbourOffsetArray[7] = new int2(1, 1); //Up Right

            for (int x = 0; x < gridSize.x; x++) //populate the grid
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPos);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = !wallsList.Contains(new int2(x, y));
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            int endNodeIndex = CalculateIndex(endPos.x, endPos.y, gridSize.x);

            PathNode startNode = pathNodeArray[CalculateIndex(startPos.x, startPos.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            //Algorithm
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex) break; //reached the destination

                for (int i = 0; i < openList.Length; i++) //remove current node from openList
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosision = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInGrid(neighbourPosision, gridSize)) continue; //neighbour not on a valid position

                    int neighbourNodeIndex = CalculateIndex(neighbourPosision.x, neighbourPosision.y, gridSize.x);
                    if (closedList.Contains(neighbourNodeIndex)) continue; // neighbour already searched

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable) continue;

                    int2 currentNodePos = new int2(currentNode.x, currentNode.y);

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePos, neighbourPosision);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                Debug.Log("Didn't find a path");
                _enablePathfinding[0] = 0;
            }
            else
            {
                CalculatePath(pathNodeArray, endNode, path);
            }

            pathNodeArray.Dispose();
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        private void CalculatePath(NativeArray<PathNode> _pathNodeArray, PathNode _endNode, NativeList<int2> path)
        {
            if (_endNode.cameFromNodeIndex == -1)
            {
                Debug.LogWarning("Could't find a path");
                return;
            }
            else
            {
                path.Add(new int2(_endNode.x, _endNode.y));

                PathNode currentNode = _endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = _pathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }
            }
        }
        private bool IsPositionInGrid(int2 pos, int2 _gridSize)
        {
            return pos.x >= 0 && pos.y >= 0
                && pos.x < _gridSize.x && pos.y < _gridSize.y;
        }
        private int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }
        private int CalculateDistanceCost(int2 aPos, int2 bPos)
        {
            int xDistance = math.abs(aPos.x - bPos.x);
            int yDistance = math.abs(aPos.y - bPos.y);
            int remainingDistance = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remainingDistance;
        }
        private int GetLowestFCostNodeIndex(NativeList<int> _openList, NativeArray<PathNode> _pathNodeArray)
        {
            PathNode lowestCostPathNode = _pathNodeArray[_openList[0]];
            for (int i = 1; i < _openList.Length; i++)
            {
                PathNode testPathNode = _pathNodeArray[_openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.index;
        }

        private struct PathNode
        {
            public int x, y;

            public int index;

            public int gCost, hCost, fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }

            public void SetWalkableState(bool state)
            {
                isWalkable = state;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = currentWaypointIndex; i < currentPath.Count; i++)
        {
            if (i == 0) Gizmos.DrawLine(transform.position, currentPath[i]);

            else Gizmos.DrawLine(currentPath[i - 1], currentPath[i]);
        }
        Gizmos.color = Color.red;
        foreach (Tile tile in tileGrid)
        {
            if(!tile.GetTileType().walkable)
            {
                Gizmos.DrawWireCube(tile.transform.position, Vector3.one);
            }
        }
    }
}
