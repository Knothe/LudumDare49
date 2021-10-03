using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] CanvasGroup start;
    [SerializeField] CanvasGroup inGame;
    [SerializeField] CanvasGroup ending;
    [SerializeField] CanvasGroup credits;

    Transition transition;

    CanvasID currentCanvas;
    CanvasID nextCanvas;
    
    private void Awake() {
        inGame.alpha = 0;
        inGame.gameObject.SetActive(false);
        ending.alpha = 0;
        ending.gameObject.SetActive(false);
        credits.alpha = 0;
        credits.gameObject.SetActive(false);
        transition = new Transition(cam);
        currentCanvas = CanvasID.START;
    }

    private void Update() {
        if (transition.Update())
            EndTransition();
    }

    public void StartGame() {
        
    }

    public void EndGame() {

    }

    void EndTransition() {
        transition.EndTransition();
        currentCanvas = nextCanvas;
    }

    public void StartUITransition(UITransitionData data) {
        float camNextSize = (data.camNextSize < 1) ? cam.orthographicSize : data.camNextSize; // Si el valor es menor a 1, se mantiene el tamaño
        nextCanvas = data.canvasObjective;
        transition.Set(GetCanvas(currentCanvas), GetCanvas(nextCanvas), data.transitionTime, camNextSize);
    }

    CanvasGroup GetCanvas(CanvasID canvas) {
        switch (canvas) {
            case CanvasID.START:
                return start;
            case CanvasID.INGAME:
                return inGame;
            case CanvasID.ENDING:
                return ending;
        }
        return credits; // Need one at the end
    }

    class Transition {
        public CanvasGroup from;
        public CanvasGroup to;
        public float transitionTime;
        public bool isInTransition { get; private set; }

        float time;
        Camera cam;
        Vector2 camTransition;

        public Transition(Camera cam) {
            this.cam = cam;
            camTransition = Vector2.zero;
        }

        public void Set(CanvasGroup from, CanvasGroup to, float transitionTime, float camNextSize) {
            time = 0;
            this.from = from;
            this.to = to;
            this.transitionTime = transitionTime;

            camTransition.x = cam.orthographicSize;
            camTransition.y = camNextSize;

            from.interactable = false;
            to.interactable = false;
            to.gameObject.SetActive(true);
            to.alpha = 0;

            isInTransition = true;

        }

        public bool Update() {
            if (!isInTransition)
                return false;
            time += Time.deltaTime;
            float v = time / transitionTime;
            from.alpha = Mathf.SmoothStep(1, 0, v);
            to.alpha = 1 - from.alpha;
            cam.orthographicSize = Mathf.SmoothStep(camTransition.x, camTransition.y, v);
            isInTransition = !(time >= transitionTime);
            return !isInTransition;
        }

        public void EndTransition() {
            from.gameObject.SetActive(false);
            to.alpha = 1;
            from.alpha = 0;
            cam.orthographicSize = camTransition.y;
            to.interactable = true;
        }
    }

}
public enum CanvasID { START, INGAME, ENDING, CREDITS };
