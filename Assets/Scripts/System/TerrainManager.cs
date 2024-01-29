using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    [SerializeField] private Transform grid;
    [SerializeField] private List<Tile> tilesList = new List<Tile>();
    [SerializeField] private Vector2 tileSize;
    [SerializeField] private Vector2 terrainSize;

    private void OnEnable()
    {
        Instance = this;

        for (int i = 0; i < grid.childCount; i++)
        {
            Tile tile = grid.GetChild(i).GetComponent<Tile>();
            tilesList.Add(tile);
        }

        foreach (Tile tile in tilesList)
        {
            if(!tile.GetTileType().walkable)
            {
                BoxCollider collider = tile.GetComponent<BoxCollider>();
                collider.size = new Vector3(1, 2, 1);
                collider.center = new Vector3(0, 0.5f, 0);
            }
        }
    }

    public Tile GetTile(Vector2 pos)
    {
        foreach (Tile tile in tilesList)
        {
            Vector3 tilePos = tile.transform.position;
            if (!(pos.x <= tilePos.x + tileSize.x / 2 && pos.x >= tilePos.x - tileSize.x / 2))
                continue;
            if (!(pos.y <= tilePos.z + tileSize.y / 2 && pos.y >= tilePos.z - tileSize.y / 2))
                continue;

            return tile;
        }

        return null;
    }
}
