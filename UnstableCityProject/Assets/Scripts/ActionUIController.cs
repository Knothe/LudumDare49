using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using data = StaticData;

public class ActionUIController : MonoBehaviour
{
    [SerializeField] Button recolect;
    [SerializeField] Button build;
    [SerializeField] Button destroy;
    [SerializeField] Button repair;
    [SerializeField] Slider healthSlider;

    private void Awake() {
        SetAll(false);
    }

    public void ShowMenu(int actions, Vector2 mousePos, float healthPercentage) {
        SetAll(true);
        recolect.interactable = data.CanRecolect(actions);
        build.interactable = data.CanBuild(actions);
        destroy.interactable = data.CanDestroy(actions);
        repair.interactable = data.CanRepair(actions);
        gameObject.transform.position = mousePos;
        healthSlider.value = healthPercentage;
        if (healthPercentage < 0)
            healthSlider.gameObject.SetActive(false);
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
