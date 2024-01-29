using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerDrawer : Editor
{
    private TerrainManager terrainManager;
    private TileType_SO tileTypeBrush;
    private Transform currentSelection;
    private bool draw = false;

    public void OnEnable()
    {
        terrainManager = (TerrainManager)target;
        Selection.selectionChanged += PaintTile;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("TerrainGeneration", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate"))
        {
            terrainManager.GenerateTerrain(terrainManager.GetTerrainSize(), terrainManager.GetTilesSize());
        }
        if (GUILayout.Button("Clear"))
        {
            terrainManager.ClearTerrain();
        }

        EditorGUILayout.LabelField("Terrain Painting", EditorStyles.boldLabel);
        EditorGUILayout.PrefixLabel("Draw");
        draw = EditorGUILayout.Toggle(draw);
        EditorGUILayout.PrefixLabel("Brush");
        tileTypeBrush = (TileType_SO)EditorGUILayout.ObjectField(tileTypeBrush, typeof(TileType_SO), false);
    }

    public void PaintTile()
    {
        if (!draw) return;

        currentSelection = Selection.activeTransform;
        if (currentSelection == null) return;
        Debug.Log("hey");
        Tile currentSelectedTile = currentSelection.GetComponent<Tile>();
        if (currentSelectedTile == null) return;
        Debug.Log("hey1");
        MeshRenderer meshRenderer = currentSelectedTile.GetComponent<MeshRenderer>();
        TileType_SO tileType = currentSelectedTile.GetTileType();
        Debug.Log("hey2");
        if (tileType == null) return;
        if (tileType.tileMaterial == null) return;
        Debug.Log("hey3");
        if (tileTypeBrush == null)
        {
            Debug.LogWarning("No Tile Brush was Selected");
            return;
        }

        meshRenderer.material = tileType.tileMaterial;
        currentSelectedTile.SetTileType(tileTypeBrush);
    }
}
