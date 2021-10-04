using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIInGameManager : MonoBehaviour
{
    [SerializeField]
    GameObject buildMenu, destroyMenu, repairMenu, catastropheIcon;

    [SerializeField]
    TMP_Text turn, wood, water, ore, stability;

    [SerializeField]
    List<Image> actionFill;

    [SerializeField]
    Image stabilityFill;

    [SerializeField]
    //TMP_Text homeCost, factoryCost, foodCost, repairCost;
    BuildCostUIData residentialCost, factoryCost, greenHouseCost, repairCost;

    [SerializeField]
    SumaryMenuData sumaryMenuData;

    [SerializeField]
    TributeMenuData tributeMenuData;

    [SerializeField]
    TMP_Text countWood, countOre, countWater;

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
        HideTributeMenu();
    }

    public void SetBuildText(int rWood, int rWater, int rOre, int fWood, int fWater, int fOre, int gWood, int gWater, int gOre) {
        residentialCost.SetValues(rWood, rWater, rOre);
        factoryCost.SetValues(fWood, fWater, fOre);
        greenHouseCost.SetValues(gWood, gWater, gOre);
    }

    public void BuildMenu(bool home, bool factory, bool food) {
        residentialCost.interactable = home;
        factoryCost.interactable = factory;
        greenHouseCost.interactable = food;
        buildMenu.SetActive(true);
    }

    public void CollectMenu() {
        // Por ahora no tiene
    }

    public void DestroyMenu() {
        destroyMenu.SetActive(true);
    }

    public void RepairMenu(bool canRepair, int wood, int water, int ore) {
        repairCost.SetValues(wood, water, ore);
        repairCost.interactable = canRepair;
        repairMenu.SetActive(true);
    }

    public void UpdateAll(int turn, int action, int maxAction, int wood, int water, int ore,
        int stability, int countWood, int countOre, int countWater, int maxTurn) {
        TurnNumber(turn, maxTurn);
        ActionNumber(action, maxAction);
        WoodNumber(wood);
        WaterNumber(water);
        OreNumber(ore);
        StabilityNumber(stability);
        this.countWood.text = "+" + countWood.ToString();
        this.countOre.text = "+" + countOre.ToString();
        this.countWater.text = "+" + countWater.ToString();
    }

    public void TurnChangeUI(int i, int week, int stability) =>
        sumaryMenuData.ShowMenu(i, week, stability);

    public void DesactivateTurnChangeUI() =>
        sumaryMenuData.HideMenu();

    void TurnNumber(int value, int maxTurn) {
        turn.text = value.ToString() + "/" + maxTurn;
    }

    void ActionNumber(int action, int max) {
        action--;
        bool state = false;
        for(int i = 0; i < actionFill.Count; i++) {
            if (i == action)
                state = true;
            actionFill[i].gameObject.SetActive(state);
        }
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

    void StabilityNumber(int value) {
        stability.text = value.ToString() + "%";
        stabilityFill.fillAmount = value / 100f;
    }

    public void HideTributeMenu() =>
        tributeMenuData.HideMenu();

    public void ShowTributeMenu(int need, int hWood, int hWater, int hOre, int stability, bool end) =>
        tributeMenuData.ShowMenu(need, need, need, hWood, hWater, hOre, stability, end);
    
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
    TMP_Text weekText, stabilityText;

    [SerializeField]
    Image stabilityFill;

    public void Initialize() {
        HideMenu();
    }

    public void ShowMenu(int i, int week, int stability) {
        sumaryMenu.SetActive(true);
        weekText.text = "Week " + week.ToString();
        stabilityText.text = stability.ToString() + "%";
        stabilityFill.fillAmount = stability/100f;
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
    GameObject tributeMenu, backPanel, closeButton, sureToPay;

    [SerializeField]
    TMP_Text stabilityText;

    [SerializeField]
    Image stabilityFill;

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

    public void HideMenu() {
        sureToPay.SetActive(false);
        tributeMenu.gameObject.SetActive(false);
    }

    public void ShowMenu(int nWood, int nWater, int nOre, int hWood, int hWater, int hOre, int stability, bool end) {
        sureToPay.SetActive(false);
        backPanel.SetActive(end);
        closeButton.SetActive(!end);
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
        else if (id == 2)
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
        stabilityFill.fillAmount = futureStability / 100f;
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
        public TMP_Text needValue;
        public TMP_Text giveValue;
        public Button plus;
        public Button minus;
    }
}

[Serializable]
public class BuildCostUIData {
    [SerializeField]
    TMP_Text wood, water, ore;

    [SerializeField]
    Button button;

    public bool interactable {
        set { button.interactable = value; }
    }
    
    public void SetValues(int wood, int water, int ore) {
        this.wood.text = wood.ToString();
        this.water.text = water.ToString();
        this.ore.text = ore.ToString();
    }
}