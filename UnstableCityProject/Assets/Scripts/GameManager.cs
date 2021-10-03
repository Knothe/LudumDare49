using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Manages game states, turns, all that stuff
public class GameManager : MonoBehaviour
{
    [SerializeField]
    UIInGameManager uiGameManager;

    [SerializeField]
    PlayerManager playerManager;

    [SerializeField]
    TileMapManager tileMap;

    [SerializeField]
    int actionsPerTurn, totalTurns;

    [SerializeField]
    ConstructionCost homeCost, factoryCost, foodCost;

    int actionCount, currentTurn;
    bool actionMenu = false;

    int wood, water, ore;
    float constantContamination;

    private void Awake() {
        wood = water = ore = 0;
        actionCount = 1;
        currentTurn = 1;
        uiGameManager.SetBuildText(homeCost.Text, factoryCost.Text, foodCost.Text);
        UpdateAllUI();
        constantContamination = 0;
    }

    public void FinishedTransition(CanvasID canvas) {

    }

    public void ShowMenu(int i) {
        actionMenu = true;
        tileMap.DesactivateTileMenu();
        switch (i) {
            case 0: uiGameManager.BuildMenu(
                homeCost.CanBuild(wood, ore, water),
                factoryCost.CanBuild(wood, ore, water),
                foodCost.CanBuild(wood, ore, water)
                );
                break;
            case 1: uiGameManager.DestroyMenu();
                break;
            case 2: uiGameManager.RepairMenu();
                break;
        }
    }

    public void Build(int id) {
        if (id == 4)
            SubstractBuildCost(homeCost);
        else if(id == 5)
            SubstractBuildCost(factoryCost);
        else if(id == 6)
            SubstractBuildCost(foodCost);
        tileMap.BuildInSelected(id);
        uiGameManager.DesactivateMenus();
        tileMap.ClearTileSelection();
        ActionRealized();
    }

    void SubstractBuildCost(ConstructionCost cost) {
        wood -= cost.wood;
        water -= cost.water;
        ore -= cost.ore;
    }

    public void Collect() {
        int matCollected = tileMap.CollectFromSelected(out float contamination);
        constantContamination += contamination;
        if (matCollected == 0)
            wood++;
        else if (matCollected == 1)
            ore++;
        else if (matCollected == 2)
            water++;
        UpdateAllUI();
        tileMap.ClearTileSelection();
        ActionRealized();
    }

    public void Destroy() {
        tileMap.DestroySelected();
        tileMap.ClearTileSelection();
        ActionRealized();
    }

    public void Repair() {
        
    }

    public void OnClick(int i, Vector2 mousePos) {
        if (actionMenu) {
            uiGameManager.DesactivateMenus();
            tileMap.ClearTileSelection();
            actionMenu = false;
        }
        else {
            tileMap.OnClick(i, mousePos);
        }
    }

    void UpdateAllUI() =>
        uiGameManager.UpdateAll(currentTurn, actionCount, actionsPerTurn, wood, water, ore);

    void ActionRealized() {
        actionCount++;
        if(actionCount > actionsPerTurn) {
            EndTurn();
            return;
        }
        UpdateAllUI();
    }

    void EndTurn() {
        actionCount = 1;
        currentTurn++;
        float cont = constantContamination + tileMap.CountContamination();
        Debug.Log("Contamination: " + cont);
        if(currentTurn > totalTurns) {
            EndGame();
            return;
        }
        // Calcular contaminación y Rollear catástrofe
        UpdateAllUI();
    }

    void EndGame() {
        Debug.Log("EndGame");
    }
}

[Serializable]
public class ConstructionCost {
    public int wood;
    public int ore;
    public int water;

    public bool CanBuild(int wood, int ore, int water) {
        return wood >= this.wood &&
            ore >= this.ore &&
            water >= this.water;
    }

    public string Text =>
        "Wood: " + wood + "\n" +
        "Ore: " + ore + "\n" +
        "Water: " + water;
        
}