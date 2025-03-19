using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UITouchEvent : UnityEvent { }

public class UITouchEventEx : UnityEvent<PointerEventData> {

}

public class cxUITouchDelegator : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler {

    public readonly UITouchEvent onPointerClick = new UITouchEvent ();
    public readonly UITouchEvent onPointerDown = new UITouchEvent ();
    public readonly UITouchEvent onPointerUp = new UITouchEvent ();
    public readonly UITouchEvent onPointerDoubleClick = new UITouchEvent (); // 더블 클릭 이벤트 추가

    // public readonly UITouchEventEx OnClick = new UITouchEventEx ();
    // public readonly UITouchEventEx OnDown = new UITouchEventEx ();
    // public readonly UITouchEventEx OnUp = new UITouchEventEx ();
    // public readonly UITouchEventEx OnDoubleClick = new UITouchEventEx (); // 더블

    public bool handleDoubleClickEvent = false;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    public bool passClickEventToParent = false;
    public bool passDoubleClickEventToParent = false;
    public bool passUpEventToParent = false;
    public bool passDownEventToParent = false;

    public void OnPointerDown (PointerEventData eventData) {
        if (passDownEventToParent) {
            PassDownEventToParent (eventData);
            return;
        }

        onPointerDown.Invoke ();
        //OnDown.Invoke (eventData);
    }

    public void OnPointerUp (PointerEventData eventData) {
        if (passUpEventToParent) {
            PassUpEventToParent (eventData);
            return;
        }

        onPointerUp.Invoke ();
        //OnUp.Invoke (eventData);
    }

    public void OnPointerClick (PointerEventData eventData) {

        if (handleDoubleClickEvent) {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;
            if (timeSinceLastClick <= doubleClickThreshold) {
                if (passDoubleClickEventToParent) {
                    PassClickEventToParent (eventData);
                    return;
                }

                onPointerDoubleClick.Invoke (); // 더블 클릭 이벤트 호출
                //OnDoubleClick.Invoke (eventData);
            } else {

                if (passClickEventToParent) {
                    PassClickEventToParent (eventData);
                    return;
                }

                onPointerClick.Invoke ();
                // OnClick.Invoke (eventData);
            }
        } else {
            if (passClickEventToParent) {
                PassClickEventToParent (eventData);
                return;
            }

            onPointerClick.Invoke ();
        }
    }

    public void PassDownEventToParent (PointerEventData eventData) {
        // 상위 오브젝로 이벤트를 전달
        GameObject parent = transform.parent ? transform.parent.gameObject : null;
        if (parent != null) {
            ExecuteEvents.ExecuteHierarchy (parent, eventData, ExecuteEvents.pointerDownHandler);
            //Debug.Log ("Event passed to: " + parent.name);
        }
    }

    public void PassUpEventToParent (PointerEventData eventData) {
        // 상위 오브젝로 이벤트를 전달
        GameObject parent = transform.parent ? transform.parent.gameObject : null;
        if (parent != null) {
            ExecuteEvents.ExecuteHierarchy (parent, eventData, ExecuteEvents.pointerUpHandler);
            //Debug.Log ("Event passed to: " + parent.name);
        }
    }

    public void PassClickEventToParent (PointerEventData eventData) {
        // 상위 오브젝로 이벤트를 전달
        GameObject parent = transform.parent ? transform.parent.gameObject : null;
        if (parent != null) {
            ExecuteEvents.ExecuteHierarchy (parent, eventData, ExecuteEvents.pointerClickHandler);
            // Debug.Log ("Event passed to: " + parent.name);
        }
    }
}