using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class afUIButtonWidget : MonoBehaviour
{

    [System.Serializable]
    public class UIFillFx
    {
        public Image fillGage;



        public float fillFxTime = 1.0f;

        public Color fullColor = Color.green;
        public Color fillColor = Color.green;
        public Color emptyColor = Color.red;

        public Gradient gradient;

        public float emptyAmount=0.0f;
        public float fullAmount=1.0f;
        public cxUITweenFx emptyFx;

    
    }


    [System.Serializable]
    public class UIHighlightFx
    {
        public GameObject highlightObject;
        public cxUITweenFx highlightFx;
    }


    public Button button;
    public Image buttonIcon;
    public Text text;

    public UIFillFx fillFx;
    public UIHighlightFx highlightFx;

    Action m_callback;

    void Awake()
    {
        if (highlightFx.highlightObject != null)
            highlightFx.highlightObject.SetActive(false);

        if (fillFx.fillGage != null)
            fillFx.fillGage.fillAmount = 0;

        button.onClick.AddListener(OnClick);
    }

    public void AddClickListener(Action callback)
    {
        m_callback = callback;
    }

    void OnClick()
    {
        if (m_callback != null)
            m_callback();
    }

    public void PlayHighlight(bool play)
    {
        if (highlightFx.highlightObject != null)
            highlightFx.highlightObject.SetActive(play);

        if (highlightFx.highlightFx != null)
        {
            if (play)
                highlightFx.highlightFx.Play();
            else
                highlightFx.highlightFx.Stop(true);
        }
    }

    public bool isHightlightPlaying
    {
        get
        {
            if (highlightFx.highlightObject != null && highlightFx.highlightObject.activeSelf)
                return true;

            if (highlightFx.highlightFx != null && highlightFx.highlightFx.isPlaying)
                return true;

            return false;
        }
    }

    Coroutine coFillFx;

    public void PlayFillFx(float to)
    {
        if (fillFx.fillGage == null)
            return;

        if (coFillFx != null)
            StopCoroutine(coFillFx);

        coFillFx = StartCoroutine(ToFillFx(to));
    }

    public void Fill(float to)
    {
        if (coFillFx != null)
            StopCoroutine(coFillFx);

        SetFillValue(to);
    }

    IEnumerator ToFillFx(float to)
    {
        float from = fillFx.fillGage.fillAmount;
        float v = from;
        bool inc = to > from;
        bool end;
        float s = Time.time;
        float duration = fillFx.fillFxTime;

        do
        {
            yield return null;
            float t = (Time.time - s) / duration;

            v = TweenUtil.Func(TweenFunc.easeOutQuad, from, to, t);

            end = inc ? v >= to : v < to;
        }
        while (!end);

        SetFillValue(to);
    }

    void SetFillValue(float to)
    {
        fillFx.fillGage.fillAmount = to;
        fillFx.fillGage.color = fillFx.gradient.Evaluate(to);
    }

    // void SetFillValue(float to)
    // {
    //     Color fromColor, toColor;
    //     float ct =1.0f;
    
    //     // if (fillFx.emptyFx != null)
    //     // {
    //     //     if (to < fillFx.emptyAmount)
    //     //         fillFx.emptyFx.Play();
    //     //     else
    //     //         fillFx.emptyFx.Stop(true);
    //     // }

    //     if (to <= fillFx.emptyAmount)
    //     {
    //         // fromColor = fillFx.emptyColor;
    //         // toColor = fillFx.fillColor;

    //         // ct =fillFx.emptyAmount!=0? 
    //         //             1.0f - ((fillFx.emptyAmount - to) /  fillFx.emptyAmount) : 0;

    //         fromColor = fillFx.emptyColor;
    //         toColor = fillFx.emptyColor;

    //         ct = 0;

    //         if (fillFx.emptyFx != null&& !fillFx.emptyFx.isPlaying)
    //             fillFx.emptyFx.Play();
    //     }
    //     else if (to >= fillFx.fullAmount)
    //     {
    //         // fromColor = fillFx.fillColor;
    //         // toColor = fillFx.fullColor;

    //         // ct = (to-fillFx.fullAmount) /  fillFx.fullAmount;

    //         fromColor = fillFx.fullColor;
    //         toColor = fillFx.fullColor;

    //         ct = 1;

    //         if (fillFx.emptyFx != null&& fillFx.emptyFx.isPlaying)
    //             fillFx.emptyFx.Stop(true);
    //     }
    //     else
    //     {
    //         fromColor = fillFx.fillColor;
    //         toColor = fillFx.fillColor;
            
    //         // fromColor = fillFx.emptyColor;
    //         // toColor = fillFx.fullColor;
                        
    //         ct = (to-fillFx.emptyAmount) /  (fillFx.fullAmount-fillFx.emptyAmount);

    //         if (fillFx.emptyFx != null && fillFx.emptyFx.isPlaying)
    //             fillFx.emptyFx.Stop(true);
    //     }

    //     Color c = Color.white;
    //     c.r = TweenUtil.Func(TweenFunc.linear, fromColor.r, toColor.r, ct);
    //     c.g = TweenUtil.Func(TweenFunc.linear, fromColor.g, toColor.g, ct);
    //     c.b = TweenUtil.Func(TweenFunc.linear, fromColor.b, toColor.b, ct);
    //     c.a = TweenUtil.Func(TweenFunc.linear, fromColor.a, toColor.a, ct);

    //     fillFx.fillGage.fillAmount = to;
    //     fillFx.fillGage.color = c;
    // }
}
