using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIInGameManager : MonoBehaviour
{
    [SerializeField]
    GameObject buildMenu, destroyMenu, repairMenu, catastropheIcon;

    [SerializeField]
    Text turn, action, wood, water, ore, stability;

    [SerializeField]
    Button homeButton, factoryButton, foodButton, repairButton;

    [SerializeField]
    Text homeCost, factoryCost, foodCost, repairCost;

    [SerializeField]
    SumaryMenuData sumaryMenuData;

    [SerializeField]
    TributeMenuData tributeMenuData;

    private void Awake() {
        sumaryMenuData.Initialize();
        tributeMenuData.Initialize();
    }

    public void Initialize() {

    }

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

    public void RepairMenu(bool canRepair, string cost) {
        repairCost.text = cost;
        repairButton.interactable = canRepair;
        repairMenu.SetActive(true);
    }

    public void UpdateAll(int turn, int action, int maxAction, int wood, int water, int ore, int stability) {
        TurnNumber(turn);
        ActionNumber(action, maxAction);
        WoodNumber(wood);
        WaterNumber(water);
        OreNumber(ore);
        StabilityNumber(stability);
    }

    public void TurnChangeUI(int i, int stability) =>
        sumaryMenuData.ShowMenu(i, stability);

    public void DesactivateTurnChangeUI() =>
        sumaryMenuData.HideMenu();

    void TurnNumber(int value) {
        turn.text = value.ToString();
    }

    void ActionNumber(int action, int max) {
        this.action.text = action.ToString() + "/" + max.ToString();
    }

    void WoodNumber(int value) {
        wood.text = value.ToString();
    }

    void WaterNumber(int value) {
        water.text = value.ToString();
    }

    void OreNumber(int value) {
        ore.text = value.ToString();
    }

    void StabilityNumber(int value) => stability.text = value.ToString() + "%";

    public void ShowTributeMenu(int nWood, int nWater, int nOre, int hWood, int hWater, int hOre, int stability) =>
        tributeMenuData.ShowMenu(nWood, nWater, nOre, hWood, hWater, hOre, stability);
    
    public int GetTributeData(out int giveWood, out int giveWater, out int giveOre) =>
        tributeMenuData.ApplyTribute(out giveWood, out giveWater, out giveOre);
    
    public void AddTribute(int id) =>
        tributeMenuData.ModifyTribute(id, 1);
    public void SubstractTribute(int id) =>
        tributeMenuData.ModifyTribute(id, -1);

    public void CatastrohpeIcon(bool b) {
        catastropheIcon.SetActive(b);
    }
}

[Serializable]
public class SumaryMenuData {
    [SerializeField]
    GameObject sumaryMenu, hurricaneText, earthquakeText, floodText, noCatastropheText;

    [SerializeField]
    Text stabilityText;

    public void Initialize() {
        HideMenu();
    }

    public void ShowMenu(int i, int stability) {
        sumaryMenu.SetActive(true);
        stabilityText.text = stability.ToString();
        if (i == 0)
            noCatastropheText.SetActive(true);
        else if(i == 1) // Affects Wood
            hurricaneText.SetActive(true);
        else if(i == 2) // Affects Ore
            earthquakeText.SetActive(true);
        else if(i == 3) // Affects Water
            floodText.SetActive(true);
    }


    public void HideMenu() {
        sumaryMenu.SetActive(false);
        hurricaneText.SetActive(false);
        earthquakeText.SetActive(false);
        floodText.SetActive(false);
        noCatastropheText.SetActive(false);
    }
}

[Serializable]
public class TributeMenuData {
    // 0 -> Wood
    // 1 -> Ore
    // 2 -> Mine
    int needWood, needWater, needOre;
    int giveWood, giveWater, giveOre;
    int haveWood, haveWater, haveOre;

    int current, futureStability;

    [SerializeField]
    GameObject tributeMenu;

    [SerializeField]
    Text stabilityText;

    [SerializeField]
    TributeUI woodUI, waterUI, oreUI;

    public void Initialize() {
        tributeMenu.SetActive(false);
    }

    public int ApplyTribute(out int giveWood, out int giveWater, out int giveOre) {
        giveWood = this.giveWood;
        giveWater = this.giveWater;
        giveOre = this.giveOre;
        tributeMenu.SetActive(false);
        return futureStability;
    }

    public void ShowMenu(int nWood, int nWater, int nOre, int hWood, int hWater, int hOre, int stability) {
        tributeMenu.SetActive(true);
        current = stability;
        giveWood = giveWater = giveOre = 0;
        woodUI.needValue.text = nWood.ToString();
        waterUI.needValue.text = nWater.ToString();
        oreUI.needValue.text = nOre.ToString();
        needWater = nWater;
        needWood = nWood;
        needOre = nOre;
        haveWood = hWood;
        haveWater = hWater;
        haveOre = hOre;
        ModifyTribute(0, 0);
        ModifyTribute(1, 0);
        ModifyTribute(2, 0);
    }

    public void ModifyTribute(int id, int value) {
        if (id == 0)
            ModifyWood(value);
        else if (id == 1)
            ModifyOre(value);
        else
            ModifyWater(value);
        PredictStability();
    }

    void PredictStability() {
        int mod = StabilityMod(needWood, giveWood) +
            StabilityMod(needWater, giveWater) + StabilityMod(needOre, giveOre);
        futureStability = current + mod;
        stabilityText.text = futureStability.ToString();
    }

    int StabilityMod(int asked, int given) =>
        (given - asked) * 2;

    void ModifyWood(int mod) {
        giveWood += mod;
        woodUI.plus.interactable = giveWood < haveWood;
        woodUI.minus.interactable = giveWood > 0;
        woodUI.giveValue.text = giveWood.ToString();
    }

    void ModifyWater(int mod) {
        giveWater += mod;
        waterUI.plus.interactable = giveWater < haveWater;
        waterUI.minus.interactable = giveWater > 0;
        waterUI.giveValue.text = giveWater.ToString();
    }

    void ModifyOre(int mod) {
        giveOre += mod;
        oreUI.plus.interactable = giveOre < haveOre;
        oreUI.minus.interactable = giveOre > 0;
        oreUI.giveValue.text = giveOre.ToString();
    }

    [Serializable]
    class TributeUI {
        public Text needValue;
        public Text giveValue;
        public Button plus;
        public Button minus;
    }
}

