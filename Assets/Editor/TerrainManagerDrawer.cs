using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerDrawer : Editor
{
    private TerrainManager terrainManager;
    private TileType_SO tileTypeBrush;
    private Transform currentSelection;
    private bool draw = false;
    private bool showDrawing = false;

    public void OnEnable()
    {
        terrainManager = (TerrainManager)target;
        Selection.selectionChanged += PaintTile;
    }

    private void OnDisable()
    {
        draw = false;
        Selection.selectionChanged -= PaintTile;
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

        showDrawing = EditorGUILayout.BeginFoldoutHeaderGroup(showDrawing, "Terrain Painting");

        if(showDrawing )
        {
            EditorGUILayout.PrefixLabel("Draw");
            draw = GUILayout.Toggle(draw, "Draw", "Button");

            EditorGUILayout.PrefixLabel("Brush");
            tileTypeBrush = (TileType_SO)EditorGUILayout.ObjectField(tileTypeBrush, typeof(TileType_SO), false);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public void PaintTile()
    {
        if (!draw) return;

        currentSelection = Selection.activeTransform;
        if (currentSelection == null) return;

        Tile currentSelectedTile = currentSelection.GetComponent<Tile>();
        if (currentSelectedTile == null) return;

        MeshRenderer meshRenderer = currentSelectedTile.GetComponent<MeshRenderer>();
        TileType_SO tileType = currentSelectedTile.GetTileType();

        if (tileType == null) return;
        if (tileTypeBrush == null)
        {
            Debug.LogWarning("No Tile Brush was Selected");
            return;
        }


        Undo.RecordObject(currentSelectedTile, "Painted Tile");
        Undo.RecordObject(meshRenderer, "Painted Tile");

        if(currentSelectedTile.GetTileType() != tileTypeBrush)
            currentSelectedTile.SetTileType(tileTypeBrush);
        if(meshRenderer.sharedMaterial != tileTypeBrush.tileMaterial)
            meshRenderer.sharedMaterial = tileTypeBrush.tileMaterial;
    }
}
