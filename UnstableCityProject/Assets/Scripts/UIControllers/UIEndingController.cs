using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndingController : MonoBehaviour
{
    [SerializeField]
    GameObject win, lose;

    [SerializeField]
    Image panel;

    [SerializeField]
    Color losePanel, winPanel;

    public void SetEndingState(bool b) {
        panel.color = (b) ? winPanel : losePanel;
        win.SetActive(b);
        lose.SetActive(!b);
    }
}
