using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

// Script d'editor (carpeta Editor: no entra a la build del joc).
// Es fa servir un sol cop des del menú Tools > Configurar Tilemap. Després es pot esborrar.
public static class TilemapSetup
{
    const string CarpetaTiles = "Assets/Tiles";
    const string CarpetaPalettes = "Assets/Tiles/Palettes";
    const string SpriteOrigen = "Assets/Sprites/block.png";
    const string SpriteTile = "Assets/Tiles/terra_placeholder.png";
    const string AssetTile = "Assets/Tiles/Terra.asset";
    const string NomPaleta = "Terra";

    [MenuItem("Tools/Configurar Tilemap")]
    public static void Configurar()
    {
        CrearCarpetes();
        Sprite sprite = PrepararSprite();
        Tile tile = CrearTile(sprite);
        CrearPaleta(tile);
        CrearTilemapAEscena();

        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap configurat. Obre Window > 2D > Tile Palette per pintar.");
    }

    private static void CrearCarpetes()
    {
        if (!AssetDatabase.IsValidFolder(CarpetaTiles))
            AssetDatabase.CreateFolder("Assets", "Tiles");
        if (!AssetDatabase.IsValidFolder(CarpetaPalettes))
            AssetDatabase.CreateFolder(CarpetaTiles, "Palettes");
    }

    // Copiem block.png (el mapa antic el continua fent servir) i el configurem com a tile:
    // PPU = amplada en píxels, així el sprite ocupa exactament 1 unitat = 1 cel·la
    private static Sprite PrepararSprite()
    {
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(SpriteTile) == null)
            AssetDatabase.CopyAsset(SpriteOrigen, SpriteTile);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(SpriteTile);
        importer.GetSourceTextureWidthAndHeight(out int amplada, out int _);

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = amplada;
        importer.filterMode = FilterMode.Point;                 // píxels nítids, sense difuminar
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(SpriteTile);
    }

    private static Tile CrearTile(Sprite sprite)
    {
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(AssetTile);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, AssetTile);
        }

        tile.sprite = sprite;
        // Grid: el collider és la cel·la sencera. Amb art de formes irregulars es canviaria a Sprite
        tile.colliderType = Tile.ColliderType.Grid;
        EditorUtility.SetDirty(tile);
        return tile;
    }

    // Una paleta és un prefab amb un Tilemap a dins: les tiles que hi poses són les que pots triar per pintar
    private static void CrearPaleta(Tile tile)
    {
        string ruta = CarpetaPalettes + "/" + NomPaleta + ".prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(ruta) == null)
        {
            GridPaletteUtility.CreateNewPalette(
                CarpetaPalettes,
                NomPaleta,
                GridLayout.CellLayout.Rectangle,
                GridPalette.CellSizing.Automatic,
                new Vector3(1f, 1f, 0f),
                GridLayout.CellSwizzle.XYZ,
                TransparencySortMode.Default,
                new Vector3(0f, 0f, 1f));
        }

        GameObject arrel = PrefabUtility.LoadPrefabContents(ruta);
        arrel.GetComponentInChildren<Tilemap>().SetTile(Vector3Int.zero, tile);
        PrefabUtility.SaveAsPrefabAsset(arrel, ruta);
        PrefabUtility.UnloadPrefabContents(arrel);
    }

    private static void CrearTilemapAEscena()
    {
        if (GameObject.Find("Grid") != null)
        {
            Debug.Log("Ja hi ha un Grid a l'escena, no en creo un altre.");
            return;
        }

        // Grid: defineix la mida de cel·la (1x1 unitats). Els Tilemaps en són fills
        GameObject grid = new GameObject("Grid", typeof(Grid));
        Undo.RegisterCreatedObjectUndo(grid, "Crear Tilemap");

        GameObject terra = new GameObject("Terra", typeof(Tilemap), typeof(TilemapRenderer));
        terra.transform.SetParent(grid.transform, false);
        terra.tag = "Terra";

        // Rigidbody2D estàtic: el composite el necessita, i Static vol dir que no es mou mai
        Rigidbody2D rb = terra.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        // Un collider per tile, que el composite fusiona en un sol contorn sense junts
        TilemapCollider2D colTiles = terra.AddComponent<TilemapCollider2D>();
        colTiles.compositeOperation = Collider2D.CompositeOperation.Merge;

        // Polygons (i no Outlines): formes plenes, així les consultes com OverlapBox
        // detecten el terra encara que la caixa quedi del tot a dins
        CompositeCollider2D composite = terra.AddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

        EditorSceneManager.MarkSceneDirty(terra.scene);
        Selection.activeGameObject = terra;
    }
}
