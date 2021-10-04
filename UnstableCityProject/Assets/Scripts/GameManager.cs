using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Manages game states, turns, all that stuff
public class GameManager : MonoBehaviour
{
    [SerializeField]
    UIStateManager uIStateManager;

    [SerializeField]
    UIInGameManager uiGameManager;

    [SerializeField]
    UIEndingController endingUI;

    [SerializeField]
    TileMapManager tileMap;

    [SerializeField]
    int actionsPerTurn, totalTurns, maxTurnsCatastrophe, inactiveTurns;

    [SerializeField]
    ConstructionCost homeCost, factoryCost, foodCost, repairCost;

    int actionCount, currentTurn;
    bool actionMenu = false;
    int turnsToCatastrophe = -1;

    int wood, water, ore;
    int constantContamination;
    int contamination { get; set; }
    int stability = 100;

    // Estabilidad -> float
    // Afectada por: Desastre Natural (-) ocurre, Contaminación (-) ft, Tributo (+-) ocurre

    // Pantalla de final de turno es la del tributo
    // Después sale una de "resumen" con # de semana y si ocurrió una catástrofe

    // La catástrofe ocurrirá en un # de turnos

    private void Awake() {
        uiGameManager.SetBuildText(homeCost.Text, factoryCost.Text, foodCost.Text);
        UpdateAllUI();
        constantContamination = 0;
    }

    public void StartedTransition(CanvasID canvas) {
        if(canvas == CanvasID.INGAME) 
            StartGame();
        else if(canvas == CanvasID.PRESENTATION)
            tileMap.StartGrid();
    }

    void StartGame() {
        actionCount = 1;
        currentTurn = 1;
        wood = water = ore = 0;
        constantContamination = 0;
        contamination = 0;
        tileMap.StartGrid();
        uiGameManager.Initialize();
        stability = 100;
        UpdateAllUI();
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
            case 2: uiGameManager.RepairMenu(repairCost.CanBuild(wood, ore, water), repairCost.Text);
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
        tileMap.ClearTileSelection();
        ActionRealized();
    }

    void SubstractBuildCost(ConstructionCost cost) {
        wood -= cost.wood;
        water -= cost.water;
        ore -= cost.ore;
    }

    public void Collect() {
        int matCollected = tileMap.CollectFromSelected(out int contamination);
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
        wood -= repairCost.wood;
        water -= repairCost.water;
        ore -= repairCost.ore;

        tileMap.RepairSelected();

        tileMap.ClearTileSelection();
        UpdateAllUI();
        ActionRealized();
    }

    public void SkipTurn() {
        EndTurn();
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

    public void StartNextTurn() {
        UpdateAllUI();
        uiGameManager.DesactivateTurnChangeUI();
    }

    public void ApplyTribute() {
        stability = uiGameManager.GetTributeData(out int giveWood, out int giveWater, out int giveOre);
        wood -= giveWood;
        water -= giveWater;
        ore -= giveOre;
        int catastrophe = CatastropheActions();
        tileMap.GridTurnCheck(ref wood, ref ore, ref water);
        uiGameManager.TurnChangeUI(catastrophe, stability);
        UpdateAllUI();
    }

    int CalculateCatastropheToStructures() => (int)Mathf.Floor(currentTurn / 10f) + 1;
    

    void UpdateAllUI() =>
        uiGameManager.UpdateAll(currentTurn, actionCount, actionsPerTurn, wood, water, ore, stability);

    void ActionRealized() {
        uiGameManager.DesactivateMenus();
        actionCount++;
        if (actionCount > actionsPerTurn) {
            EndTurn();
            return;
        }
        UpdateAllUI();
    }

    void EndTurn() {
        actionCount = 1;
        currentTurn++;
        contamination = constantContamination + tileMap.CountContamination();
        if (currentTurn > totalTurns) {
            EndGame();
            return;
        }
        uiGameManager.ShowTributeMenu(3, 3, 3, wood, water, ore, stability);
        UpdateAllUI();
    }

    int CatastropheActions() {
        turnsToCatastrophe--;
        if (turnsToCatastrophe == 0) {
            int catastrophe = ChooseCatastrophe();
            stability -= currentTurn;
            tileMap.ApplyCatastrophe(catastrophe, CalculateCatastropheToStructures(), inactiveTurns);
            uiGameManager.CatastrohpeIcon(false);
            turnsToCatastrophe = -1;
            return catastrophe;
        }
        else if (turnsToCatastrophe < 0) {
            if (CatastropheHappens()) {
                turnsToCatastrophe = maxTurnsCatastrophe;
                uiGameManager.CatastrohpeIcon(true);
            }
        }
        return 0;
    }

    // Change with formula to calculate Catastrophe probability
    bool CatastropheHappens() => UnityEngine.Random.Range(0, 101) <= contamination;

    int ChooseCatastrophe() => UnityEngine.Random.Range(1, 4);

    void EndGame() {
        uIStateManager.StartUITransition(CanvasID.ENDING, 2, 10);
        endingUI.SetEndingState(stability > 0);
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