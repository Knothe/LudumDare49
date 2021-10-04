using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEndingController : MonoBehaviour
{
    [SerializeField]
    GameObject winText, loseText;

    [SerializeField]
    TMP_Text stabilityText;

    [SerializeField]
    Image stabilityFill;

    public void SetEndingState(int stability) {
        bool b = stability <= 0;
        winText.SetActive(!b);
        loseText.SetActive(b);
        stabilityText.text = stability.ToString();
        stabilityFill.fillAmount = stability / 100f;
    }
}
