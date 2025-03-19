using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/************************************************
 * 
 * Frame 관리를 위한 기본 클래스
 * 
 * **********************************************/

public abstract partial class cxUIFrame : MonoBehaviour {

    public enum ShowOption {
        ShowImmedately,
        ShowFadeIn,
        ShowFadeOut,
        SlideLeft,
        SlideRight,
        SlideUP,
        SlideDown,
        ShowTween,
    }

    public enum HideOption {
        HideImmediately,
        HideFadeOut,
        SlideLeft,
        SlideRight,
        SlideUP,
        SlideDown,
        HideTween,
    }

    public enum FrameType {
        Frame,
        Dialog,
        Aux,
    }

    [System.Serializable]
    public class ManagedPanel {
        public CanvasGroup panel;

        [HideInInspector]
        public bool bAlways = false;
        [HideInInspector]
        public bool bStart = false;

        public ShowOption showOption = ShowOption.ShowImmedately;
        public float showTime = 1.0f;

        public HideOption hideOption = HideOption.HideImmediately;
        public float hideTime = 1.0f;

        [HideInInspector]
        public float remainedSmoothTime = 0.0f;
    }

    [SerializeField]
    protected FrameType m_frameType;
    [SerializeField]
    protected ManagedPanel[] m_managedPanels;
    private bool m_show = false;
    public bool IsActive { get { return m_show; } }

    private List<cxUIFrame> popupFrames = new List<cxUIFrame> ();

    private Subject<object> onCloseEvent = new Subject<object>();

    public IObservable<object> OnCloseOnceAsObservale
    {
        get { return onCloseEvent.AsObservable().Take(1); }
    }

    public bool IsDialog { get => m_frameType == FrameType.Dialog; }

    public cxUIFrame ActivePopup {
        get {
            if (popupFrames.Count > 0)
                return popupFrames[popupFrames.Count - 1];
            else
                return null;
        }
    }

    // Use this for initialization
    void Awake () {
        m_show = false;

        foreach (ManagedPanel p in m_managedPanels) {
            if (p.panel == null)
                continue;

            var rt = p.panel.GetComponent<RectTransform> ();
            rt.anchorMin = new Vector2 (0.0f, 0.0f);
            rt.anchorMax = new Vector2 (1.0f, 1.0f);
            rt.anchoredPosition = Vector2.zero;

            p.panel.gameObject.SetActive (false);
        }

        gameObject.SetActive (false);

        OnInit ();
    }

    [ContextMenu ("ShowInEditor")]
    void ShowInEditor () {
        Show ();
    }

    [ContextMenu("HideInEditor")]
    void HideInEditor()
    {
        Hide();
    }

    public void Replace (cxUIContext context, object showParam) {
        bool alreadyShow = m_show;

        if (!alreadyShow) {
            Navigate (this, false);
            Show ();
            OnActivated (showParam);
        }
    }

    public void Push (cxUIContext context, object showParam) {
        bool alreadyShow = m_show;

        if (!alreadyShow) {
            Navigate (this, true);
            Show ();
            OnActivated (showParam);
        }
    }

    public void PushPopup (cxUIContext context, object showParam) {
        bool alreadyShow = m_show;

        if (!alreadyShow) {
            Navigate (this, true);
            Show ();
            OnActivated (showParam);
        }
    }

    public void Pop () {
        OnDeactivated ();
        Hide ();

        onCloseEvent.OnNext(null);

        foreach (var f in popupFrames) {
            f.Close ();
        }
        popupFrames.Clear ();

        PopNavigate (this);
    }

    protected void PopResult(object result)
    {
        OnDeactivated();
        Hide();

        onCloseEvent.OnNext(result);

        foreach (var f in popupFrames)
        {
            f.Close();
        }
        popupFrames.Clear();

        PopNavigate(this);
    }

    //네비게이션 하이아키 변경 없이 무조건 닫기
    private void Close () {
        Hide ();
        foreach (var f in popupFrames)
            f.Close ();
        popupFrames.Clear ();

        OnDeactivated ();
    }

    private void Show (bool onlyShow = false) {
        bool alreadyShow = m_show;

        m_show = true;

        StopCoroutine ("_SmoothShow");
        StopCoroutine ("_SmoothHide");
        StopCoroutine ("_SlideHide");
        StopCoroutine ("_SlideShow");
        StopCoroutine ("_DelayDeactive");

        if (!alreadyShow)
            gameObject.SetActive (true);

        foreach (ManagedPanel p in m_managedPanels) {
            if (p.panel == null)
                continue;

            if (p.bStart == true && p.bAlways == true)
                continue;

            p.panel.gameObject.SetActive (true);

            if (p.showOption == ShowOption.ShowImmedately) {
                p.remainedSmoothTime = 0.0f;
            } else if (p.showOption == ShowOption.ShowFadeIn) {
                p.remainedSmoothTime = p.showTime;
                StartCoroutine (_SmoothShow (p));
            } else if (p.showOption == ShowOption.ShowTween) {
                var tweenPanel = p.panel.GetComponent<cxUITweenPanel> ();
                if (tweenPanel) {
                    p.remainedSmoothTime = tweenPanel.m_param.duration + tweenPanel.m_param.delay;
                    tweenPanel.Play ();
                } else {
                    Debug.LogWarning ("ShowTween has not afUITweenControl");
                }
            } else {
                p.remainedSmoothTime = p.showTime;
                StartCoroutine (_SlideShow (p));
            }

            p.bStart = true;
        }
    }

    public void Hide (bool onlyShow = false) {
        if (!m_show)
            return;

        StopCoroutine ("_SmoothShow");
        StopCoroutine ("_SmoothHide");
        StopCoroutine ("_SlideHide");
        StopCoroutine ("_SlideShow");
        StopCoroutine ("_DelayDeactive");

        m_show = false;

        if (!gameObject.activeSelf)
            return;

        float maxHideTime = 0.0f;

        foreach (ManagedPanel p in m_managedPanels) {
            if (p.panel == null)
                continue;

            if (p.bAlways == true)
                continue;

            if (p.hideOption == HideOption.HideImmediately) {
                p.panel.gameObject.SetActive (false);
            } else if (p.hideOption == HideOption.HideFadeOut) {
                p.remainedSmoothTime = p.hideTime;
                StartCoroutine (_SmoothHide (p));
            } else if (p.hideOption == HideOption.HideTween) {
                var tweenPanel = p.panel.GetComponent<cxUITweenPanel> ();
                if (tweenPanel) {
                    tweenPanel.Hide ();
                    p.hideTime = tweenPanel.m_hideParam.duration + tweenPanel.m_hideParam.delay;
                }
            } else {
                p.remainedSmoothTime = p.hideTime;
                StartCoroutine (_SlideHide (p));
            }

            if (p.hideTime > maxHideTime)
                maxHideTime = p.hideTime;
        }

        StartCoroutine (_DelayDeactive (maxHideTime));
    }

    IEnumerator _DelayDeactive (float maxHideTime) {
        yield return new WaitForSeconds (maxHideTime);
        gameObject.SetActive (false);
    }

    IEnumerator _SmoothShow (ManagedPanel p) {
        while (p.remainedSmoothTime > 0) {
            p.remainedSmoothTime -= Time.deltaTime;

            if (p.remainedSmoothTime <= 0) {
                p.remainedSmoothTime = 0;
            }

            if (p.showOption == ShowOption.ShowImmedately) {
                if (p.panel != null)
                    p.panel.alpha = 1.0f; // p.panel.SetAlphaRecursive(1.0f, false);
            }

            if (p.showOption == ShowOption.ShowFadeIn) {
                float alpha = p.remainedSmoothTime / p.showTime;
                if (p.panel != null)
                    p.panel.alpha = 1.0f - alpha; // p.panel.SetAlphaRecursive(1.0f - alpha, false);
            }

            if (p.showOption == ShowOption.ShowFadeOut) {
                float alpha = p.remainedSmoothTime / p.showTime;
                if (p.panel != null)
                    //  p.panel.SetAlphaRecursive(alpha - 1.0f, false);
                    p.panel.alpha = alpha - 1.0f;
            }

            yield return null;
        }
    }

    IEnumerator _SmoothHide (ManagedPanel p) {
        while (p.remainedSmoothTime > 0) {
            p.remainedSmoothTime -= Time.deltaTime;
            if (p.remainedSmoothTime <= 0) {
                p.remainedSmoothTime = 0;
                //NGUITools.SetActive(p.panel.gameObject, false);
                p.panel.gameObject.SetActive (false);
            }

            if (p.hideOption == HideOption.HideFadeOut) {
                float alpha = p.remainedSmoothTime <= 0.0f ? 1.0f : p.remainedSmoothTime / p.hideTime;
                //p.panel.SetAlphaRecursive(alpha, false);
                p.panel.alpha = alpha;
            }

            if (p.hideOption == HideOption.HideImmediately) {
                //p.panel.SetAlphaRecursive(0, false);
                p.panel.alpha = 0;
            }

            yield return null;
        }

        p.panel.gameObject.SetActive (false);
    }

    IEnumerator _SlideShow (ManagedPanel p) {
        Vector3 startPos = Vector3.zero;
        float z = p.panel.transform.localPosition.z;

        if (p.showOption == ShowOption.SlideLeft)
            startPos.x = -1000;
        else if (p.showOption == ShowOption.SlideRight)
            startPos.x = 1000;
        else if (p.showOption == ShowOption.SlideUP)
            startPos.y = 700;
        else
            startPos.y = -700;

        while (p.remainedSmoothTime > 0) {
            p.remainedSmoothTime -= Time.deltaTime;
            float alpha = 1.0f - p.remainedSmoothTime / p.showTime;
            alpha = Mathf.Clamp01 (alpha);

            float x = TweenUtil.Func (TweenFunc.easeOutQuad, startPos.x, 0.0f, alpha);
            float y = TweenUtil.Func (TweenFunc.easeOutQuad, startPos.y, 0.0f, alpha);

            p.panel.transform.localPosition = new Vector3 (x, y, z);

            yield return null;
        }

        p.panel.transform.localPosition = new Vector3 (0, 0, z);

    }

    IEnumerator _SlideHide (ManagedPanel p) {
        Vector3 endPos = Vector3.zero;
        float z = p.panel.transform.localPosition.z;

        if (p.showOption == ShowOption.SlideLeft)
            endPos.x = -1000;
        else if (p.showOption == ShowOption.SlideRight)
            endPos.x = 1000;
        else if (p.showOption == ShowOption.SlideUP)
            endPos.y = 700;
        else
            endPos.y = -700;

        while (p.remainedSmoothTime > 0) {
            p.remainedSmoothTime -= Time.deltaTime;
            float alpha = 1.0f - p.remainedSmoothTime / p.showTime;
            alpha = Mathf.Clamp01 (alpha);

            float x = TweenUtil.Func (TweenFunc.easeInQuad, 0, endPos.x, alpha);
            float y = TweenUtil.Func (TweenFunc.easeInQuad, 0, endPos.y, alpha);

            p.panel.transform.localPosition = new Vector3 (x, y, z);

            yield return null;
        }

        p.panel.transform.localPosition = new Vector3 (endPos.x, endPos.y, z);
        p.panel.gameObject.SetActive (false);
    }

    void OnCancel () => Hide ();

    #region  LifeCycle Event

    protected abstract void OnInit (); //!< 최초 시작 시점에 호출된다. 			
    protected abstract void OnActivated (object showParam); //!< 시작후 Activated 될때 호출된다. OnInit 호출뒤에 곧바로 호출된다.
    protected abstract void OnDeactivated (); //!< Panel이 Disable될때 호출된다.
    protected virtual void OnBecomeInvisible () { }
    protected virtual void OnBecomeVisible () { }
    protected virtual void OnGameStateMessage(int msg, object msgParam) { }

    #endregion

}