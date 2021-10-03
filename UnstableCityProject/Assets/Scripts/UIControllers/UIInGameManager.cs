using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameManager : MonoBehaviour
{
    [SerializeField]
    GameObject buildMenu, destroyMenu, repairMenu;

    [SerializeField]
    Text turn, action, wood, water, ore;

    [SerializeField]
    Button homeButton, factoryButton, foodButton;

    [SerializeField]
    Text homeCost, factoryCost, foodCost;

    public void DesactivateMenus() {
        buildMenu.SetActive(false);
        destroyMenu.SetActive(false);
        repairMenu.SetActive(false);
    }

    public void SetBuildText(string home, string factory, string food) {
        homeCost.text = home;
        factoryCost.text = factory;
        foodCost.text = food;
    }

    public void BuildMenu(bool home, bool factory, bool food) {
        homeButton.interactable = home;
        factoryButton.interactable = factory;
        foodButton.interactable = food;
        buildMenu.SetActive(true);
    }

    public void CollectMenu() {
        // Por ahora no tiene
    }

    public void DestroyMenu() {
        destroyMenu.SetActive(true);
    }

    public void RepairMenu() {
        repairMenu.SetActive(true);
    }

    public void UpdateAll(int turn, int action, int maxAction, int wood, int water, int ore) {
        TurnNumber(turn);
        ActionNumber(action, maxAction);
        WoodNumber(wood);
        WaterNumber(water);
        OreNumber(ore);
    }

    public void TurnNumber(int value) {
        turn.text = value.ToString();
    }

    public void ActionNumber(int action, int max) {
        this.action.text = action.ToString() + "/" + max.ToString();
    }

    public void WoodNumber(int value) {
        wood.text = value.ToString();
    }

    public void WaterNumber(int value) {
        water.text = value.ToString();
    }

    public void OreNumber(int value) {
        ore.text = value.ToString();
    }
}
