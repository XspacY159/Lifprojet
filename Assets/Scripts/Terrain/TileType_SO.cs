using UnityEngine;

[CreateAssetMenu(fileName = "NewTileType", menuName = "LifProjet/NewTileType")]
public class TileType_SO : ScriptableObject
{
    [Header("Tile properties")]

    public string tileName = "new tile type";
    public bool walkable = true;
    [Range(0f, 1f)] public float movingSpeedCoeff = 1;
    public Material tileMaterial;
}