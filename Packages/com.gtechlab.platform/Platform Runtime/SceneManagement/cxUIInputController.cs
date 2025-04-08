using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder (-99)]
public class cxUIInputController : MonoSingleton<cxUIInputController> {

    [SerializeField]
    private bool clickAtLongPress = true;

    private GameObject focused {  get;  set; }

    public Vector2? ScreenClickedThisFrame { get; private set; }
    public Vector2? PressedDragDeltaThisFrame { get; private set; }
    public Vector2? ScrollDeltaThisFrame { get; private set; }

    public bool IsLeftButton => mouseButton == 0;
    public bool IsRightButton => mouseButton == 1;
    public bool IsLongPress => pressTime > 1.0f;

    Vector3? beginPosition;
    Vector3? lastPosition;
    bool mouseDragged;
    int mouseButton;
    float downTime;
    float pressTime;

    public bool IsFocused => focused != null || IsAnyInputFieldFocused ();
    public bool HasFocus(GameObject gObject) => focused == gObject ||  EventSystem.current.currentSelectedGameObject == gObject;

    public void AcquireFocus (GameObject go) {
        focused = go;
    }

    public void ReleaseFocus (GameObject go) {
        if (focused == go)
            focused = null;
    }

    public bool IsAnyInputFieldFocused () {
        var selectedObject = EventSystem.current.currentSelectedGameObject;
        return selectedObject != null && (
            selectedObject.GetComponent<InputField> () != null ||
            selectedObject.GetComponent<TMP_InputField> () != null
        );
    }

    void Update () {
        PressedDragDeltaThisFrame = null;
        ScrollDeltaThisFrame = null;
        ScreenClickedThisFrame = null;

        if (focused != null) {
            beginPosition = null;
            lastPosition = null;
            mouseButton = -1;
            pressTime = 0;
            return;
        }

        if (OnMouseDown) {
            if (!cxUISystemUtil.IsMousePointerOnUI (0)) {
                beginPosition = Input.mousePosition;
                lastPosition = beginPosition;
                mouseButton = MouseButtonID;
                mouseDragged = false;
                downTime = Time.realtimeSinceStartup;
                pressTime = 0;
            }
        }

        if (beginPosition.HasValue) {
            PressedDragDeltaThisFrame = Input.mousePosition - lastPosition.Value;
            lastPosition = Input.mousePosition;
            mouseDragged = mouseDragged || PressedDragDeltaThisFrame.Value.sqrMagnitude > float.Epsilon;
        }

        bool release = false;

        if (OnMouseUp) {
            if (!cxUISystemUtil.IsMousePointerOnUI (0)) {
                if (!mouseDragged) {
                    ScreenClickedThisFrame = Input.mousePosition;
                    pressTime = Time.realtimeSinceStartup - downTime;
                }
            }

            release = true;
        }

        ScrollDeltaThisFrame = Input.mouseScrollDelta;

        if(clickAtLongPress && beginPosition.HasValue && !mouseDragged) {
            pressTime = Time.realtimeSinceStartup - downTime;
            if(pressTime > 1) {
                ScreenClickedThisFrame = Input.mousePosition;
                release = true;
            }
        }

        if (release) {
            beginPosition = null;
            lastPosition = null;
            // mouseButton = -1;
        }
    }

    bool OnMouseDown => Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1);
    bool OnMouseUp => Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1);
    int MouseButtonID => Input.GetMouseButton (0) ? 0 : Input.GetMouseButton (1) ? 1 : -1;

}