using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class cxUISwitchControl : MonoBehaviour
{
    public cxUITouchDelegator toggleButton;
    public Graphic background;
    public RectTransform thumb;

    public Color onBackgroundColor = Color.gray;
    public Color onToggleColor = Color.black;

    public Color offBackgroundColor = Color.gray;
    public Color offToggleColor = Color.black;

    private float offsetX;
    public float duration = 1.0f;

    private bool isOn;

    public bool IsOn { get { return isOn; } set {
            SwitchTo(value);
        } }

    private Subject<bool> state = new Subject<bool>();

    public IObservable<bool> OnStateAsObservale
    {
        get { return state.AsObservable(); }
    }

    private void Awake()
    {
        offsetX = Mathf.Abs(thumb.anchoredPosition.x);
    }

    // Start is called before the first frame update
    void Start()
    {
        toggleButton.onPointerClick.AddListener(() => {
            SwitchTo(!isOn);
        });

        SwitchTo(isOn);
    }

    public void Set(bool on)
    {
        SwitchTo(on, true);
    }

    void SwitchTo(bool on, bool reset=false)
    {
        isOn = on;
       
        thumb.GetComponent<Graphic>().color = on ? onToggleColor : offToggleColor;
        background.color = on ? onBackgroundColor : offBackgroundColor;

        if (reset)
        {
            float x = on ? offsetX : -offsetX;
            Vector3 to = new Vector3(x, 0, 0);
            thumb.anchoredPosition3D = to;
        }
        else
        {
            state.OnNext(isOn);
            StartCoroutine(MoveTo(thumb, on ? offsetX : -offsetX));

        }
    }

    IEnumerator MoveTo(RectTransform target, float x)
    {
        TweenUtil t1 = new TweenUtil(duration);
        t1.Trigger();

        Vector3 from = target.anchoredPosition3D;
        Vector3 to = new Vector3(x, 0, 0);

        while (t1.On)
        {
            Vector3 p = t1.VectorValue(TweenFunc.linear, from, to);
            target.anchoredPosition3D = p;
            yield return null;
        }

        target.anchoredPosition3D = to;
    }
}
