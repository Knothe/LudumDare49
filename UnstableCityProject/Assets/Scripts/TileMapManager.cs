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
    Color selectColor, disabledColor;

    [SerializeField]
    ActionUIController actionUI;

    Dictionary<TileBase, TileData> dataFromTiles;
    //Tile selectedTile;
    LogicTile[,] tileValues;
    TileData[][] startingTileValues;
    Vector3Int arrayOffset;
    Vector3Int selectedPointGrid;
    Vector3Int selectedPointArray;
    
    Vector3Int GridToArray(Vector3Int value) => value - arrayOffset;
    Vector3Int ArrayToGrid(Vector3Int value) => value + arrayOffset;

    List<LogicTile> inactivePoints = new List<LogicTile>();

    public int countWood { get; private set; }
    public int countOre { get; private set; }
    public int countWater { get; private set; }


    private void Awake() {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas) {
            foreach (TileBase tile in tileData.tiles) {
                dataFromTiles.Add(tile, tileData);
                tileData.Initialize();
            }
        }
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
        startingTileValues = new TileData[size.x][];
        for(Vector3Int value = Vector3Int.zero; value.x < size.x; value.x++) {
            startingTileValues[value.x] = new TileData[size.y];
            for(value.y = 0; value.y < size.y; value.y++) {
                TileData data = GetTileData(value + arrayOffset);
                if (data == null)
                    continue;
                startingTileValues[value.x][value.y] = data;
            }
        }
    }

    public void StartGrid() {
        countWood = countOre = countWater = 0;
        inactivePoints.Clear();
        tileValues = new LogicTile[startingTileValues.Length, startingTileValues[0].Length];
        for(Vector3Int value = Vector3Int.zero; value.x < startingTileValues.Length; value.x++) {
            for(value.y = 0; value.y < startingTileValues[value.x].Length; value.y++) {
                tileValues[value.x, value.y] = new LogicTile(startingTileValues[value.x][value.y], value);
                map.SetTile(ArrayToGrid(value), startingTileValues[value.x][value.y].GetRandomTile());
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

    public bool OnClick(int i, Vector2 mousePos) {
        if (i == 0) 
            return ActivateTileMenu(mousePos);
        
        return false;
    }

    public void DesactivateTileMenu() {
        actionUI.Desactivate();
    }

    public void ClearTileSelection() {
        DesactivateTileMenu();
        SetTileColour(Color.white, selectedPointGrid);
        selectedPointGrid = Vector3Int.one * -1;
    }

    bool ActivateTileMenu(Vector2 mousePos) {
        if (!CheckClickedTile(mousePos, out Vector3Int gridPosition) || gridPosition == selectedPointGrid) {
            ClearTileSelection();
            return false;
        }
        Vector3Int arrayPos = GridToArray(gridPosition);
        if (!tileValues[arrayPos.x, arrayPos.y].isActive) {
            ClearTileSelection();
            return false;
        }
        SetTileColour(Color.white, selectedPointGrid);
        selectedPointGrid = gridPosition;
        selectedPointArray = arrayPos;
        actionUI.ShowMenu(tileValues[selectedPointArray.x, selectedPointArray.y].actionValue, Input.mousePosition, 
            tileValues[selectedPointArray.x, selectedPointArray.y].healthPercentage);
        SetTileColour(selectColor, selectedPointGrid);
        return true;
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
        LogicTile selected = tileValues[selectedPointArray.x, selectedPointArray.y];
        selected.RegainHealth();
        selected.AddAction(1);
        selected.DeleteAction(8);
        ModifyActive(1, selected.id);
    }

    public void BuildInSelected(int id) {
        TileData data = GetTileData(id);
        map.SetTile(selectedPointGrid, data.GetRandomTile());
        UpdateArrayTile(selectedPointArray, data);
        ModifyActive(1, id);
    }
    
    public void ModifyActive(int mod, int id) {
        if (id == 4)
            countWood += mod;
        else if (id == 5)
            countOre += mod;
        else if (id == 6)
            countWater += mod;
    }

    // Ahorita los naturales están dando 2 y los humanos 1
    public int CollectFromSelected(out int contamination, out int quantity) {
        LogicTile selected = tileValues[selectedPointArray.x, selectedPointArray.y];
        contamination = 0;
        int material = (selected.id - 1) % 3;
        if (selected.id >= 4) {
            // Colecting from human made stuff
            quantity = 1;
            if (selected.DealDamage(1)) {
                selected.DeleteAction(1);
                selected.AddAction(8);
                ModifyActive(-1, selected.id);
            }
        }
        else {
            // Colecting from nature
            quantity = 2;
            if (selected.DealDamage(1)) {
                contamination = selected.contaminationByDead;
                DestroyAt(selectedPointGrid);
            }
        }
        return material;
    }

    public void GridTurnCheck(ref int wood, ref int ore, ref int water) {
        foreach(LogicTile tile in tileValues) {
            if(tile.id >= 4) {
                if (tile.isActive && tile.CanRecolect()) {
                    if (tile.DealDamage(1)) {
                        tile.DeleteAction(1);
                        tile.AddAction(8);
                        ModifyActive(-1, tile.id);
                    }
                    if (tile.id == 4) { wood++; }
                    if (tile.id == 5) { ore++; }
                    if (tile.id == 6) { water++; }
                }
            } 
            else if(tile.id >= 1) {
                tile.RecoverHealth();
            }
        }
        for(int i = inactivePoints.Count - 1; i >= 0; i--) {
            inactivePoints[i].inactiveTurnsLeft--;
            if (inactivePoints[i].inactiveTurnsLeft <= 0) {
                inactivePoints[i].isActive = true;
                SetTileColour(disabledColor, ArrayToGrid(inactivePoints[i].arrayLocation));
                ModifyActive(1, inactivePoints[i].id);
                inactivePoints.RemoveAt(i);
            }
        }
    }

    public void ApplyCatastrophe(int type, int damage, int inactiveTurns) {
        int type2 = type + 3;
        DesactivateRandomTile(type, type2, inactiveTurns);
        DesactivateRandomTile(type, type2, inactiveTurns);
        DealDamageSameType(type2, damage);
    }

    void DesactivateRandomTile(int id1, int id2, int inactiveTurns) {
        bool desactivated = false;
        Vector3Int index = Vector3Int.zero;
        while (!desactivated) {
            index.x = Random.Range(0, startingTileValues.Length);
            index.y = Random.Range(0, startingTileValues[0].Length);
            LogicTile tile = tileValues[index.x, index.y];
            if((tile.id == id1 || tile.id == id2) && tile.isActive && tile.CanRecolect()) {
                ModifyActive(-1, id2);
                tile.isActive = false;
                tile.inactiveTurnsLeft = inactiveTurns;
                inactivePoints.Add(tile);
                SetTileColour(disabledColor, ArrayToGrid(index));
                return;
            }
        }
    }

    void DealDamageSameType(int id1, int damage) {
        foreach(LogicTile tile in tileValues) {
            if(tile.id == id1) {
                if (tile.DealDamage(damage)) {
                    tile.DeleteAction(1);
                    tile.AddAction(8);
                    if(tile.isActive)
                        ModifyActive(-1, tile.id);
                }
            }
        }
    }

    /// <summary> Destroys something and replaces it with floor </summary>
    /// <param name="indexGrid">Grid index to destroy</param>
    void DestroyAt(Vector3Int indexGrid) {
        TileData data = GetTileData(0);

        Vector3Int indexArray = GridToArray(indexGrid);
        if (tileValues[indexArray.x, indexArray.y].id >= 4)
            ModifyActive(-1, tileValues[indexArray.x, indexArray.y].id);

        map.SetTile(indexGrid, data.GetRandomTile());
        UpdateArrayTile(GridToArray(indexGrid), data);
    }

    public int CountContamination() {
        int contamination = 0;
        foreach(LogicTile tile in tileValues) {
            contamination += tile.contaminationByExisting;
        }
        return contamination;
    }

    void UpdateArrayTile(Vector3Int indexArray, TileData data) =>
        tileValues[indexArray.x, indexArray.y].SetNewValues(data, indexArray);

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
