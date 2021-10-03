using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {
    [SerializeField]
    TileMapManager mapManager;

    Vector2 mousePos => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void Update() {
        ReadMouseInput(0);

    }

    void ReadMouseInput(int i) {
        if (Input.GetMouseButtonDown(i) && !IsMouseOverUIWithIgnores()) {
            mapManager.OnClick(i, mousePos);
        }
    }

    private bool IsMouseOverUIWithIgnores() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++) {
            if (raycastResultList[i].gameObject.CompareTag("IgnoreMouseClick")) {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultList.Count > 0;
    }
}
