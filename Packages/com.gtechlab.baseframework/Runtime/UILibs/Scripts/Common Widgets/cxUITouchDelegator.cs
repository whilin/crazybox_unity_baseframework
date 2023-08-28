using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UITouchEvent : UnityEvent  {}

public class cxUITouchDelegator : MonoBehaviour , IPointerDownHandler, IPointerClickHandler , IPointerUpHandler
{
    public readonly UITouchEvent onPointerClick = new UITouchEvent();
    public readonly UITouchEvent onPointerDown = new UITouchEvent();
    public readonly UITouchEvent onPointerUp = new UITouchEvent();


    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick.Invoke();
    }
}
