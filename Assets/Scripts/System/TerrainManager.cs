using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    [SerializeField] private Transform grid;
    [SerializeField] private List<Tile> tilesList = new List<Tile>();
    [SerializeField] private GameObject tilePrefab;
    [Tooltip("Taille des tuilles en 2D")]
    [SerializeField] private Vector2 tilesSize = Vector2.one;
    [Tooltip("Taille du terrain en 2D")]
    [SerializeField] private Vector2 terrainSize = Vector2.one;

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
            //Les tailles sont des Vector2 donc "y" décrit leur profondeur dans l'espace
            Vector3 tilePos = tile.transform.position;
            if (!(pos.x <= tilePos.x + tilesSize.x / 2 && pos.x >= tilePos.x - tilesSize.x / 2))
                continue;
            if (!(pos.y <= tilePos.z + tilesSize.y / 2 && pos.y >= tilePos.z - tilesSize.y / 2)) 
                continue;

            return tile;
        }

        return null;
    }

    public Vector2 GetTilesSize()
    {
        return tilesSize;
    }

    public Vector2 GetTerrainSize()
    {
        return terrainSize;
    }

    public void GenerateTerrain(Vector2 _tarrainSize, Vector2 _tileSize)
    {
        ClearTerrain();

        for (int i = 0; i < _tarrainSize.x; i++)
            for (int j = 0; j < _tarrainSize.y; j++)
            {
                Vector3 currentPos = new Vector3(i * _tileSize.x, transform.position.y, j * _tileSize.y);

                Instantiate(tilePrefab, currentPos, Quaternion.identity, grid);
            }
    }

    public void ClearTerrain()
    {
        while (grid.childCount > 0)
        {
            DestroyImmediate(grid.GetChild(0).gameObject);
        }
    }
}
