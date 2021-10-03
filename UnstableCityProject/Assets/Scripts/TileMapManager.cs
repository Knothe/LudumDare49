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
    Vector3Int selectedPoint;

    Tile selectedTile;

    private void Awake() {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in tileDatas) {
            foreach (TileBase tile in tileData.tiles) {
                dataFromTiles.Add(tile, tileData);
                tileData.Initialize();
            }
        }
        selectedTile = new Tile();
        selectedTile.tileBase = null;
    }

    public void OnClick(int i, Vector2 mousePos) {
        if (i == 0) {
            if (selectedTile.tileBase != null)
                DesactivateTileMenu();
            else
                ActivateTileMenu(mousePos);
        }
    }

    void DesactivateTileMenu() {
        selectedTile.tileBase = null;
        actionUI.Desactivate();
        SetTileColour(Color.white, selectedPoint);
        selectedTile.ResetValues();
    }

    void ActivateTileMenu(Vector2 mousePos) {
        if (!GetTileValues(mousePos))
            return;
        selectedPoint = selectedTile.gridPosition;
        print("At position " + selectedTile.gridPosition + " there is a " + selectedTile.tileData.id);
        actionUI.ShowMenu(selectedTile.tileData.actionValue, Input.mousePosition);
        SetTileColour(selectColor, selectedTile.gridPosition);
    }

    bool GetTileValues(Vector2 mousePos) {
        selectedTile.gridPosition = map.WorldToCell(mousePos);
        selectedTile.tileBase = map.GetTile(selectedTile.gridPosition);
        if (selectedTile.tileBase == null)
            return false;
        selectedTile.tileData = dataFromTiles[selectedTile.tileBase];
        return true;
    }

    void SetTileColour(Color color, Vector3Int position) {
        map.SetTileFlags(position, TileFlags.None);
        map.SetColor(position, color);
    }

    public void Button1() {

    }

    public void Button2() {

    }

    public void Button3() {

    }

    struct Tile {
        public TileBase tileBase;
        public TileData tileData;
        public Vector3Int gridPosition;

        public void ResetValues() {
            tileBase = null;
            tileData = null;
            gridPosition = Vector3Int.zero;
        }
    }
}
