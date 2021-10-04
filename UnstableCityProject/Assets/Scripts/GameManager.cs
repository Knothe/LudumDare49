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
    AudioManager audioManager;
    
    [SerializeField]
    UIEndingController endingUI;

    [SerializeField]
    TileMapManager tileMap;

    [SerializeField]
    int actionsPerTurn, totalTurns, maxTurnsCatastrophe, inactiveTurns;

    [SerializeField]
    ConstructionCost homeCost, factoryCost, foodCost, repairCost;

    [SerializeField]
    TributeValueChange tributeValueChange;

    int actionCount, currentTurn;
    bool actionMenu = false;
    int turnsToCatastrophe = -1;

    int wood, water, ore;
    int constantContamination;
    int contamination { get; set; }
    int stability;
    bool endingGame;

    // Estabilidad -> float
    // Afectada por: Desastre Natural (-) ocurre, Contaminación (-) ft, Tributo (+-) ocurre

    // Pantalla de final de turno es la del tributo
    // Después sale una de "resumen" con # de semana y si ocurrió una catástrofe

    // La catástrofe ocurrirá en un # de turnos

    private void Awake() {
        uiGameManager.SetBuildText(
            homeCost.wood, homeCost.water, homeCost.ore,
            factoryCost.wood, factoryCost.water, factoryCost.ore,
            foodCost.wood, foodCost.water, foodCost.ore);
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
        endingGame = false;
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
            case 2: uiGameManager.RepairMenu(repairCost.CanBuild(wood, ore, water), repairCost.wood, repairCost.water, repairCost.ore);
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
        int matCollected = tileMap.CollectFromSelected(out int contamination, out int quantity);
        constantContamination += contamination;
        if (matCollected == 0) {
            wood += quantity;
            audioManager.PlaySFX(AudioSFXClip.WOOD_CUT);
        }
        else if (matCollected == 1) {
            ore += quantity;
            audioManager.PlaySFX(AudioSFXClip.PICKAXE);
        }
        else if (matCollected == 2) {
            water += quantity;
            audioManager.PlaySFX(AudioSFXClip.WATER_DROP);
        }
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

    public void CloseMenu() {
        uiGameManager.DesactivateMenus();
        tileMap.ClearTileSelection();
        actionMenu = false;
    }

    public void ClosePayMenu() {
        uiGameManager.HideTributeMenu();
        actionMenu = false;
    }

    public void OpenPayMenu() {
        tileMap.ClearTileSelection();
        actionMenu = true;
        uiGameManager.ShowTributeMenu(tributeValueChange.GetTributes(currentTurn), wood, water, ore, stability, actionCount > actionsPerTurn);
        UpdateAllUI();
    }

    public void OnClick(int i, Vector2 mousePos) {
        if (actionCount > actionsPerTurn)
            return;
        if (actionMenu) {
            uiGameManager.DesactivateMenus();
            tileMap.ClearTileSelection();
            actionMenu = false;
        }
        else {
            if (tileMap.OnClick(i, mousePos))
                audioManager.PlaySFX(AudioSFXClip.POP);
        }
    }

    public void StartNextTurn() {
        UpdateAllUI();
        uiGameManager.DesactivateTurnChangeUI();
    }

    // Make here the turn change
    public void ApplyTribute() {
        actionCount = 1;
        currentTurn++;

        stability = uiGameManager.GetTributeData(out int giveWood, out int giveWater, out int giveOre);
        wood -= giveWood;
        water -= giveWater;
        ore -= giveOre;
        int catastrophe = CatastropheActions();
        if (currentTurn > totalTurns || stability <= 0) {
            EndGame();
            return;
        }
        audioManager.SwapTrack((stability >= 25) ? 0 : 1);
        tileMap.GridTurnCheck(ref wood, ref ore, ref water);
        uiGameManager.TurnChangeUI(catastrophe, currentTurn, stability);
        UpdateAllUI();
    }

    int CalculateCatastropheToStructures() => (int)Mathf.Floor(currentTurn / 10f) + 1;
    

    void UpdateAllUI() =>
        uiGameManager.UpdateAll(currentTurn, actionCount, actionsPerTurn, wood, water, ore,
            stability, tileMap.countWood, tileMap.countOre, tileMap.countWater);

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
        contamination = constantContamination + tileMap.CountContamination();
        OpenPayMenu();
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

    bool CatastropheHappens() => UnityEngine.Random.Range(0, 101) <= contamination;

    int ChooseCatastrophe() => UnityEngine.Random.Range(1, 4);

    void EndGame() {
        uiGameManager.DesactivateMenus();
        uIStateManager.StartUITransition(CanvasID.ENDING, 2, 3.5f);
        endingUI.SetEndingState(stability);
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

}

[Serializable]
public class TributeValueChange{
    [SerializeField]
    List<int> changeTributeTurn;

    int value = 1;
    int currentTribute = 0;

    public int GetTributes(int currentTurn) {
        int nextChange = (changeTributeTurn.Count <= currentTribute) ? -1 : changeTributeTurn[currentTribute];
        if (nextChange == currentTurn)
            value++;
        return value;
    }
}