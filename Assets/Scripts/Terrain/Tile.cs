using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileType_SO tileType;

    public TileType_SO GetTileType()
    {
        return tileType;
    }
}
