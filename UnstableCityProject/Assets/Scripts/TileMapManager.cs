using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour {
    [SerializeField]
    Tilemap map;

    [SerializeField]
    List<TileData> tileDatas;

    [SerializeField]
    Color selectColor;

    [SerializeField]
    ActionUIController actionUI;

    Dictionary<TileBase, TileData> dataFromTiles;
    //Tile selectedTile;
    LogicTile[,] tileValues;
    Vector3Int arrayOffset;
    bool selecting;
    Vector3Int selectedPointGrid;
    Vector3Int selectedPointArray;

    Vector3Int GridToArray(Vector3Int value) => value - arrayOffset;
    Vector3Int ArrayToGrid(Vector3Int value) => value + arrayOffset;

    private void Awake() {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas) {
            foreach (TileBase tile in tileData.tiles) {
                dataFromTiles.Add(tile, tileData);
                tileData.Initialize();
            }
        }
        //selectedTile = new Tile();
        //selectedTile.tileBase = null;
        selecting = false;
        SetMapArray();
    }

    void SetMapArray() {
        Vector3Int inferiorLimit; // Offset for the index?
        Vector3Int superiorLimit;

        if (!FindInferiorLimit(out inferiorLimit, out superiorLimit))
            return;

        FindSuperiorLimit(ref inferiorLimit, ref superiorLimit);
        arrayOffset = inferiorLimit;
        FillTileValues(superiorLimit - inferiorLimit);
    }

    void FillTileValues(Vector3Int size) {
        size.x++;
        size.y++;
        Debug.Log(size);
        tileValues = new LogicTile[size.x, size.y];
        for(Vector3Int value = Vector3Int.zero; value.x < size.x; value.x++) {
            for(value.y = 0; value.y < size.y; value.y++) {
                TileData data = GetTileData(value + arrayOffset);
                if (data == null)
                    continue;
                tileValues[value.x, value.y] = new LogicTile(data);
            }
        }
    }

    bool FindInferiorLimit(out Vector3Int inferiorLimit, out Vector3Int superiorLimit) {
        BoundsInt b = map.cellBounds;
        inferiorLimit = b.min;
        superiorLimit = b.max;
        while (inferiorLimit.x != superiorLimit.x && inferiorLimit.y != superiorLimit.y) {
            TileBase tile = map.GetTile(inferiorLimit);
            if (tile != null)
                break;
            inferiorLimit.x++;
            inferiorLimit.y++;
        }
        if (inferiorLimit.x == superiorLimit.x || inferiorLimit.y == superiorLimit.y) {
            Debug.Log("Reset Map Bounds");
            return false;
        }
        inferiorLimit.x--;
        if (map.GetTile(inferiorLimit) != null)
            FindBound(new Vector3Int(-1, 0, 0), ref inferiorLimit);
        else
            FindBound(new Vector3Int(0, -1, 0), ref inferiorLimit);
        return true;
    }

    void FindSuperiorLimit(ref Vector3Int inferiorLimit, ref Vector3Int superiorLimit) {
        Vector3Int movement = new Vector3Int(1, 0, 0);
        Vector3Int temp = inferiorLimit;
        do {
            temp += movement;
        } while (map.GetTile(temp) != null);
        temp -= movement;
        superiorLimit.x = temp.x;
        movement.Set(0, 1, 0);
        temp = inferiorLimit;
        do {
            temp += movement;
        } while (map.GetTile(temp) != null);
        temp -= movement;
        superiorLimit.y = temp.y;
    }

    void FindBound(Vector3Int movement, ref Vector3Int inferiorLimit) {
        inferiorLimit.x++;
        while(map.GetTile(inferiorLimit) != null) {
            inferiorLimit += movement;
        }
        inferiorLimit -= movement;
    }

    public void OnClick(int i, Vector2 mousePos) {
        if (i == 0) {
            ActivateTileMenu(mousePos);
        }
    }

    public void DesactivateTileMenu() {
        actionUI.Desactivate();
    }

    public void ClearTileSelection() {
        DesactivateTileMenu();
        SetTileColour(Color.white, selectedPointGrid);
        selecting = false;
        selectedPointGrid = Vector3Int.one * -1;
    }

    void ActivateTileMenu(Vector2 mousePos) {
        if (!CheckClickedTile(mousePos, out Vector3Int gridPosition) || gridPosition == selectedPointGrid) {
            ClearTileSelection();
            return;
        }
        selecting = true;
        SetTileColour(Color.white, selectedPointGrid);
        selectedPointGrid = gridPosition;
        selectedPointArray = GridToArray(gridPosition);
        actionUI.ShowMenu(tileValues[selectedPointArray.x, selectedPointArray.y].actionValue, Input.mousePosition, 
            tileValues[selectedPointArray.x, selectedPointArray.y].healthPercentage);
        SetTileColour(selectColor, selectedPointGrid);
    }

    bool CheckClickedTile(Vector2 mousePosition, out Vector3Int gridPosition) {
        gridPosition = map.WorldToCell(mousePosition);
        return map.GetTile(gridPosition);
    }

    TileData GetTileData(Vector3Int tileIndex) {
        TileBase tileBase = map.GetTile(tileIndex);
        return (tileBase == null) ? null: dataFromTiles[tileBase];
    }

    void SetTileColour(Color color, Vector3Int position) {
        map.SetTileFlags(position, TileFlags.None);
        map.SetColor(position, color);
    }

    public void DestroySelected() {
        DestroyAt(selectedPointGrid);
    }

    public void RepairSelected() {
        // No sé qué conlleva reparar algo
    }

    public void BuildInSelected(int id) {
        TileData data = GetTileData(id);
        map.SetTile(selectedPointGrid, data.GetRandomTile());
        UpdateArrayTile(selectedPointArray, data);
    }

    public int CollectFromSelected(out float contamination) {
        LogicTile selected = tileValues[selectedPointArray.x, selectedPointArray.y];
        contamination = 0;
        int material = (selected.id - 1) % 3;
        if (selected.id >= 4) {
            // Colecting from human made stuff
        }
        else {
            // Colecting from nature
            if (selected.DealDamage(1)) {
                contamination = selected.contaminationByDead;
                DestroyAt(selectedPointGrid);
            }
        }
        return material;
    }

    /// <summary> Destroys something and replaces it with floor </summary>
    /// <param name="indexGrid">Grid index to destroy</param>
    void DestroyAt(Vector3Int indexGrid) {
        TileData data = GetTileData(0);
        map.SetTile(indexGrid, data.GetRandomTile());
        UpdateArrayTile(GridToArray(indexGrid), data);
    }

    public float CountContamination() {
        float contamination = 0;
        foreach(LogicTile tile in tileValues) {
            contamination += tile.contaminationByExisting;
        }
        return contamination;
    }

    void UpdateArrayTile(Vector3Int indexArray, TileData data) =>
        tileValues[indexArray.x, indexArray.y].SetNewValues(data);

    TileData GetTileData(int id) {
        foreach(TileData tile in tileDatas) {
            if (tile.id == id)
                return tile;
        }
        return null;
    }

    public void DamageType(int i) {

    }

    public Vector3Int GetSelectedPoint() => selectedPointGrid;
}
