using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using data = StaticData;

public class ActionUIController : MonoBehaviour
{
    [SerializeField] Button recolect;
    [SerializeField] Button build;
    [SerializeField] Button destroy;
    [SerializeField] Button repair;
    [SerializeField] Slider healthSlider;

    [SerializeField] GameObject collectDescription;
    [SerializeField] Image materialType;

    [SerializeField] List<Sprite> materials;
    [SerializeField] TMP_Text quantity;

    private void Awake() {
        SetAll(false);
    }

    public void ShowMenu(int actions, Vector2 mousePos, float healthPercentage, int id) {
        SetAll(true);
        SetCollect(actions, id);
        build.interactable = data.CanBuild(actions);
        destroy.interactable = data.CanDestroy(actions);
        repair.interactable = data.CanRepair(actions);
        gameObject.transform.position = mousePos;
        healthSlider.value = healthPercentage;
        if (healthPercentage < 0)
            healthSlider.gameObject.SetActive(false);
    }

    void SetCollect(int actions, int id) {
        bool b = data.CanRecolect(actions);
        recolect.interactable = b;
        collectDescription.SetActive(b);
        if (b) {
            int q = (id >= 4) ? 1 : 2;
            int type = (id >= 4) ? id - 4 : id - 1;
            quantity.text = "+" + q;
            materialType.sprite = materials[type];
        }
    }

    public void Desactivate() {
        SetAll(false);
    }

    public void SetAll(bool v) {
        recolect.gameObject.SetActive(v);
        build.gameObject.SetActive(v);
        destroy.gameObject.SetActive(v);
        repair.gameObject.SetActive(v);
        healthSlider.gameObject.SetActive(v);
    }
}
