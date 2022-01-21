using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEffectUtil : MonoBehaviour {

    public enum UIEffectFunc
    {
        MovingFromLeft,
        MovingFromRight,
        MovingFromUp,
        MovingFromDown,

        MovingFastFromLeft,
        MovingFastFromRight,
        MovingFastFromUp,
        MovingFastFromDown,

        MovingBounceFromLeft,
        MovingBounceFromRight,
        MovingBounceFromUp,
        MovingBounceFromDown,


        ScaleX,
        ScaleY,
        ScaleXY,

        ScaleBounceX,
        ScaleBounceY,
        ScaleBounceXY,

        Stamp,
    }

    public enum MovingDir
    {
        Left,
        Right,
        Up, 
        Down,
    }

    //public UIEffectParam m_param;

    public UIEffectFunc m_funcType = UIEffectFunc.ScaleXY;
    public float m_delay = 0.0f;
   // public Transform m_movingAnchor;
    
    //public bool m_fast = false;
    //public TweenFunc m_tweenFunc = TweenFunc.easeInQuart;
    //public float m_time = 1.5f;
    //public float m_delay = 0.0f;
   //public MovingDir m_movingAxis;

    Vector3 initPosition;
    Vector3 initScale;

    void Awake()
    {
        initPosition = transform.localPosition;
        initScale = transform.localScale;
    }

    public void PlayUIEffect(bool show)
    {
        PlayUIEffect(gameObject, m_funcType, m_delay, initPosition, initScale);
    }

    public static void PlayUIEffect(GameObject gameObject, UIEffectFunc funType, float delay, Vector3 initPosition, Vector3 initScale)
    {
        switch (funType)
        {
            case UIEffectFunc.MovingFromLeft:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Left, gameObject, initPosition, delay, TweenFunc.easeOutQuart, 1.5f);
                }
                break;
             case UIEffectFunc.MovingFromRight:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Right, gameObject, initPosition, delay, TweenFunc.easeOutQuart, 1.5f);
                }
                break;
             case UIEffectFunc.MovingFromUp:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Up, gameObject, initPosition, delay, TweenFunc.easeOutQuart, 1.5f);
                }
                break;
             case UIEffectFunc.MovingFromDown:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Down, gameObject, initPosition, delay, TweenFunc.easeOutQuart, 1.5f);
                }
                break;
             case UIEffectFunc.MovingFastFromLeft:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Left, gameObject, initPosition, delay, TweenFunc.easeInQuart, 1.0f);
                }
                break;
             case UIEffectFunc.MovingFastFromRight:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Right, gameObject, initPosition, delay, TweenFunc.easeInQuart, 1.0f);
                }
                break;
             case UIEffectFunc.MovingFastFromUp:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Up, gameObject, initPosition, delay, TweenFunc.easeInQuart, 1.0f);
                }
                break;
             case UIEffectFunc.MovingFastFromDown:
                {
                    UIEffectUtil.ShowFrom(MovingDir.Down, gameObject, initPosition, delay, TweenFunc.easeInQuart, 1.0f);
                }
                break;
            case UIEffectFunc.ScaleX:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, true, false, TweenFunc.easeInQuart);
                }
                break;
            case UIEffectFunc.ScaleY:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, false, true, TweenFunc.easeInQuart);
                }
                break;
            case UIEffectFunc.ScaleXY:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, true, true, TweenFunc.easeInQuart);
                }
                break;
            case UIEffectFunc.Stamp:
                {
                    UIEffectUtil.Stamp(gameObject, delay, initScale);
                }
                break;
        }
    }


    /// <summary>
    /// /////////////////////////////////////////////////////
    /// </summary>
    static UIEffectUtil script;

    static void Init()
    {
        if (script == null)
        {
            var obj = new GameObject("UIEffectUtil");
            script = obj.AddComponent<UIEffectUtil>();

            GameObject.DontDestroyOnLoad(script);
        }
    }


    public static void Stamp(GameObject target, float delay )
    {
        Init();

        script.StartCoroutine(script.DoStamp(target, delay, new Vector3(1,1,1)));
    }


    /*********************************************
      * Timer
      * *******************************************/
    /*
    public delegate bool TimerCallback(UILabel target);
    public static Coroutine PlayTimeText(UILabel target, float checkT, TimerCallback callback)
    {
        target.StopAllCoroutines();
        return target.StartCoroutine(DoPlayTimeText(target, checkT, callback));
    }
    public static void StopTimeText(UILabel target)
    {
        target.StopAllCoroutines();
    }

    static IEnumerator DoPlayTimeText(UILabel target, float checkT, TimerCallback callback)
    {
        bool repeat = false;
        do
        {
            repeat = callback(target);
            yield return new WaitForSeconds(checkT);
        }
        while (repeat);
    }
    */

    /*
    public static Coroutine PlayTimeText(UILabel target, float remainedT, UIFillProgressBar gageBar=null, float totalTime=0)
    {
        return target.StartCoroutine(DoPlayTimeText(target, remainedT, gageBar, totalTime));
    }

    static IEnumerator DoPlayTimeText(UILabel target, float remainedT, UIFillProgressBar gageBar, float totalTime)
    {
        System.DateTime startTime = System.DateTime.Now;
        System.DateTime endTime = startTime.AddMinutes(remainedT);
        string text = string.Empty;
        float r = 1.0f;

        do
        {
            r = (float) (endTime - System.DateTime.Now).TotalMinutes;

            text = "00:00:00";// IconUtil.RemainedQuestTime(r);
            target.text = text;

            if (gageBar != null)
            {
                float p = 1.0f - r / totalTime;
                p = Mathf.Clamp01(p) * 100;

                gageBar.SetValueCurrent((int) p);
            }

            yield return new WaitForSeconds(1.0f);
        }
        while ( r > 0 );
    }
    */

    /*********************************************
      * Scale
      * *******************************************/

    public static void ShowScaleVBounce(GameObject target, bool autoActive = false, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, true, delay, autoActive, false, true));
    }

    public static void HideScaleVBounce(GameObject target, bool autoActive = false, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, false, delay, autoActive, false, true));
    }
    
    public static void ShowScaleV(GameObject target, bool autoActive = false, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, true, delay, autoActive, false, true, TweenFunc.linear, 0.3f));
    }

    public static void HideScaleV(GameObject target, bool autoActive = false, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, false, delay, autoActive, false, true, TweenFunc.easeOutQuad));
    }

    /*********************************************
     * Loop Rotate,Shake
     * *******************************************/

    public static void Shake(GameObject target, Vector2 shakeScale, float delay = 0.0f)
    {
        Init();

        script.StartCoroutine(script.DoShake(target, shakeScale, delay));
    }

    public static void Rotation(GameObject target, float delay = 0.0f, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 0.5f)
    {
        Init();

        script.StartCoroutine(script.DoRotate(target, delay, tfunc, duration));
    }

    //public static void LoopRotation(GameObject target, float delay = 0.0f, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 0.5f)
    //{
    //    Init();

    //    script.StartCoroutine(script.DoLoopRotate(target, delay, tfunc, duration));
    //}

    /*********************************************
      * Stamp
      * *******************************************/

    public static void Stamp(GameObject target, float delay, Vector3 initScale, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 1.5f)
    {
        Init();

        script.StartCoroutine(script.DoStamp(target, delay, initScale, tfunc, duration));
    }

    /*********************************************
      * Scale
      * *******************************************/
    public static void ShowScale(GameObject target, bool autoActive = false, float delay = 0.0f, bool xAxis=true, bool yAxis=true, TweenFunc tweener= TweenFunc.easeOutBounce, float duration=0.5f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, true, delay, autoActive, xAxis, yAxis, tweener, duration));
    }

    public static void HideScale(GameObject target, bool autoActive = false, float delay = 0.0f, bool xAxis = true, bool yAxis = true, TweenFunc tweener = TweenFunc.easeOutBounce, float duration=0.5f)
    {
        Init();
        script.StartCoroutine(script.DoShowScale(target, false, delay, autoActive, xAxis, yAxis, tweener, duration));
    }

    /*********************************************
    * Alpha
    * *******************************************/
    
    public static void ShowAlpha(UIPanel target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f)
    {
        Init();

        script.StartCoroutine(script.DoShowAlpha(target, true, autoActive, delay, tfunc, duration));
    }

    public static void HideAlpha(UIPanel target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f)
    {
        Init();

        script.StartCoroutine(script.DoShowAlpha(target, false, autoActive, delay,tfunc, duration));
    }
    
    
    /*********************************************
     * From/To
     * *******************************************/
    public static void ShowFrom(MovingDir dirIndex, GameObject target, Vector3 initPosition , float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, float cycleTime = 1.5f)
    {
        Init();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right)? 0 :  1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down)? -1 : 1;

        Vector3 to = target.transform.localPosition;
        Vector3 from = target.transform.localPosition;

        to[axis] = initPosition[axis];
        from[axis] = initPosition[axis];

        to[axis] = initPosition[axis];
        from[axis] = to[axis] + 800 * dir;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, tweenFunc, cycleTime));
    }


    public static void HideTo(MovingDir dirIndex, GameObject target, Vector3 initPosition, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, float duration = 1.5f)
    {
        Init();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right) ? 0 : 1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down) ? -1 : 1;


        Vector3 to = target.transform.localPosition;
        Vector3 from = target.transform.localPosition;

        to[axis] = initPosition[axis];
        from[axis] = initPosition[axis];

        from[axis] = initPosition[axis];
        to[axis] = from[axis] - 800 * dir;

        script.StartCoroutine(script.DoHideMoving(target, from, to, delay, true, tweenFunc, duration));
    }

    /*
    public static void ShowFrom(MovingDir dirIndex, GameObject target, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, Transform anchor = null, float cycleTime = 1.5f)
    {
        Init();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right)? 0 :  1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down)? -1 : 1;

        Vector3 to = target.transform.localPosition;
        Vector3 from = target.transform.localPosition;

        to[axis] = anchor != null ? anchor.localPosition[axis] : target.transform.localPosition[axis];
        from[axis] = anchor != null ? anchor.localPosition[axis] : target.transform.localPosition[axis];

        to[axis] = anchor != null ? anchor.localPosition[axis] : 0;
        from[axis] = to[axis] + 800 * dir;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, tweenFunc, cycleTime));
    }
    */


    public static void ShowFromLeft(GameObject target, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, Transform anchor=null)
    {
        Init();

        Vector3 to = anchor != null ? anchor.localPosition : target.transform.localPosition;
        Vector3 from = anchor != null ? anchor.localPosition : target.transform.localPosition;

        to.x = anchor != null ? anchor.localPosition.x : 0;
        from.x = to.x - 800;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, tweenFunc));
    }


    public static void ShowFromRight(GameObject target, Transform anchor = null, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutQuad, float duration = 0.5f)
    {
        Init();

        Vector3 to = anchor != null ? anchor.localPosition : target.transform.localPosition;
        Vector3 from = anchor != null ? anchor.localPosition : target.transform.localPosition;

        to.x = anchor != null ? anchor.localPosition.x : 0;
        from.x = to.x + 800;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, tweenFunc, duration));
    }


    public static void ShowFromUp(GameObject target, float delay = 0.0f)
    {
        Init();

        Vector3 from = target.transform.localPosition;
        Vector3 to = target.transform.localPosition;

        from.y = 800;
        to.y = 0;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, TweenFunc.easeInQuad));
    }

    public static void HideToLeft(GameObject target, float delay , bool autoActive = false)
    {
        Init();

        Vector3 from = target.transform.localPosition;
        Vector3 to = target.transform.localPosition;

        from.x = 0;
        to.x = -800;

        script.StartCoroutine(script.DoHideMoving(target, from, to, delay, autoActive));
    }


    public static void IncValue(Text target, int initValue, int nextValue, string textFormat, float delay = 0.0f)
    {
        Init();

        script.StartCoroutine(script.DoIncValue(target, initValue,nextValue, textFormat, TweenFunc.linear, delay));
    }

/*
    public static void IncProgress(UIFillProgressBar target, int initValue, int nextValue, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoIncProgress(target, initValue, nextValue,delay));
    }

    public static void IncProgress(UIRepeatProgressBar target, int initValue, int nextValue, float delay = 0.0f)
    {
        Init();
        script.StartCoroutine(script.DoIncProgress(target, initValue, nextValue, delay));
    }
*/

    IEnumerator DoStamp(GameObject target, float delay, Vector3 initScale, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration =1.5f)
    {
        if (delay > 0)
        {
            target.SetActive(false);
            yield return new WaitForSeconds(delay);
            target.SetActive(true);
        }

        target.transform.localScale = new Vector3(5.0f * initScale.x, 5.0f * initScale.y, 1.0f);
        TweenUtil t1 = new TweenUtil(duration);

        t1.Trigger();
        while (t1.On)
        {
            float sx = t1.FloatValue(tfunc, 5.0f * initScale.x, initScale.x);
            float sy = t1.FloatValue(tfunc, 5.0f * initScale.y, initScale.y);
            target.transform.localScale = new Vector3(sx, sy, 1.0f);

            yield return null;
        }
    }

    IEnumerator DoShake(GameObject target, Vector2 shakeScale, float delay, TweenFunc tfunc = TweenFunc.easeInQuad, float duration = 1.5f)
    {
       
        TweenUtil t1 = new TweenUtil(0.5f, 4, TweenLoopType.PingPong);
        TweenUtil t2 = new TweenUtil(2.0f, 1, TweenLoopType.PingPong);

        if (delay > 0)
            yield return new WaitForSeconds(delay);


        t1.Trigger();
        t2.Trigger();

        Vector3 initPos = target.transform.localPosition;

       
        if (t1.On)
        {
            float x = t1.FloatValue(TweenFunc.linear, -shakeScale.x, shakeScale.x);
            float y = t1.FloatValue(TweenFunc.linear, -shakeScale.y, shakeScale.y);

            float scale = t2.FloatValue(tfunc, 1.0f, 0.0f);
            x *= scale;
            y *= scale;

            Vector3 pos = new Vector3(initPos.x + x, initPos.y + y, initPos.z);
            target.transform.localPosition = pos;
            yield return null;
        }

        target.transform.localPosition = initPos;
    }

    IEnumerator DoIncValue(Text target, int initValue, int nextValue, string textFormat, TweenFunc tween = TweenFunc.easeInOutExpo, float delay = 0.0f)
    {
        target.text = string.Format(textFormat, initValue);

        if (delay > 0)
            yield return new WaitForSeconds(delay);


        TweenUtil tw = new TweenUtil(2.0f);
        tw.Trigger();

        do
        {
            yield return null;

            int v = (int)tw.FloatValue(tween, initValue, nextValue);
            target.text = string.Format(textFormat, v);
        }
        while (tw.On);

        target.text = string.Format(textFormat, nextValue);
    }

/*
    IEnumerator DoIncProgress(UIFillProgressBar target, int initValue, int nextValue, float delay)
    {
        //target.text = string.Format(textFormat, initValue);
        target.SetValueCurrent(initValue);

        if (delay > 0)
            yield return new WaitForSeconds(delay);


        TweenUtil tw = new TweenUtil(2.0f);
        tw.Trigger();

        do
        {
            yield return null;

            int v = (int)tw.FloatValue(TweenFunc.easeInOutExpo, initValue, nextValue);
            target.SetValueCurrent(v);
        }
        while (tw.On);

        target.SetValueCurrent(nextValue);
    }
*/

/*

    IEnumerator DoIncProgress(UIRepeatProgressBar target, int initValue, int nextValue, float delay)
    {
        //target.text = string.Format(textFormat, initValue);
        target.SetValueCurrent(initValue);

        if (delay > 0)
            yield return new WaitForSeconds(delay);


        TweenUtil tw = new TweenUtil(2.0f);
        tw.Trigger();

        do
        {
            yield return null;

            int v = (int)tw.FloatValue(TweenFunc.easeInOutExpo, initValue, nextValue);
            target.SetValueCurrent(v);
        }
        while (tw.On);

        target.SetValueCurrent(nextValue);
    }
*/
    IEnumerator DoShowAlpha(UIPanel target, bool show, bool autoActive, float delay,TweenFunc tfunc=TweenFunc.linear, float duration = 0.3f)
    {
        if (autoActive)
            if (show)
                target.gameObject.SetActive(show);

        target.alpha = show ? 0.0f : 1.0f;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        TweenUtil tw = new TweenUtil(duration);
        tw.Trigger();

        float initValue = show ? 0.0f : 1.0f;
        float nextValue = show ? 1.0f : 0.0f;
        do
        {
            yield return null;

            float v = tw.FloatValue(tfunc, initValue, nextValue);
            target.alpha = v;
        }
        while (tw.On);

        target.alpha =nextValue;

        if (autoActive)
            if (!show)
                target.gameObject.SetActive(show);

    }


    IEnumerator DoShowScale(GameObject target, bool show, float delay, bool autoActive,bool x, bool y, TweenFunc fun=TweenFunc.easeOutBounce, float duration = 0.5f)
    {
        Vector3 initValue = new Vector3( x ? (show ? 0.0f : 1.0f) : 1.0f, y ? (show ? 0.0f : 1.0f) : 1.0f, 1.0f);
        Vector3 nextValue = new Vector3( x ? (show ? 1.0f : 0.0f) : 1.0f, y ? (show ? 1.0f : 0.0f) : 1.0f, 1.0f);

        if (autoActive)
            if (show)
                target.SetActive(show);
        
        target.transform.localScale = initValue;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        TweenUtil tw = new TweenUtil(duration);
        tw.Trigger();

        do
        {
            yield return null;

            var v = tw.VectorValue(fun, initValue, nextValue);
            target.transform.localScale = v;
        }
        while (tw.On);

        target.transform.localScale = nextValue;

        if (autoActive)
            if (!show)
                target.SetActive(show);
    }

    IEnumerator DoRotate(GameObject target, float delay, TweenFunc tf=TweenFunc.easeOutBounce, float duration=0.5f)
    {
        Vector3 initValue = new Vector3(0,0,0);
        Vector3 nextValue = new Vector3(0,0,360);

        target.transform.localRotation = Quaternion.AngleAxis(initValue.z, Vector3.forward);

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        TweenUtil tw = new TweenUtil(duration);
        tw.Trigger();

        do
        {
            yield return null;

            var v = tw.VectorValue(tf, initValue, nextValue);
            target.transform.localRotation = Quaternion.AngleAxis(v.z, Vector3.forward); //Quaternion.Euler(v);
        }
        while (tw.On);

        target.transform.localRotation = Quaternion.AngleAxis(nextValue.z, Vector3.forward);  //Quaternion.Euler(nextValue);
    }

    IEnumerator DoLoopRotate(GameObject target, float delay, TweenFunc tf = TweenFunc.easeOutBounce, float duration = 0.5f)
    {
        Vector3 initValue = new Vector3(0, 0, 0);
        Vector3 nextValue = new Vector3(0, 0, 360);

        target.transform.localRotation = Quaternion.AngleAxis(initValue.z, Vector3.forward);

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        TweenUtil tw = new TweenUtil(duration, 1000000, TweenLoopType.Loop);
        tw.Trigger();

        do
        {
            yield return null;

            var v = tw.VectorValue(tf, initValue, nextValue);
            target.transform.localRotation = Quaternion.AngleAxis(v.z, Vector3.forward); //Quaternion.Euler(v);
        }
        while (tw.On);

        target.transform.localRotation = Quaternion.AngleAxis(nextValue.z, Vector3.forward);  //Quaternion.Euler(nextValue);
    }

    IEnumerator DoShowMoving(GameObject target, Vector3 from, Vector3 to, float delay, TweenFunc tf= TweenFunc.easeOutBounce, float cycleTime = 1.5f)
    {
        if (delay > 0)
        {
            target.SetActive(false);
            yield return new WaitForSeconds(delay);
            target.SetActive(true);
        }

        target.transform.localPosition = from;
        TweenUtil t1 = new TweenUtil(cycleTime);
        t1.Trigger();

        while (t1.On)
        {
            Vector3 p = t1.VectorValue(tf, from, to);
            target.transform.localPosition = p;

            yield return null;
        }

        target.transform.localPosition = to;
    }

    IEnumerator DoHideMoving(GameObject target, Vector3 from, Vector3 to, float delay, bool autoActive, TweenFunc tween = TweenFunc.easeOutBounce, float duration=1.5f)
    {
        if (delay > 0)
        {
            target.SetActive(false);
            yield return new WaitForSeconds(delay);
            target.SetActive(true);
        }

        target.transform.localPosition = from;
        TweenUtil t1 = new TweenUtil(duration);
        t1.Trigger();

        while (t1.On)
        {
            Vector3 p = t1.VectorValue(tween, from, to);
            target.transform.localPosition = p;

            yield return null;
        }

        target.transform.localPosition = to;

        if (autoActive)
            target.SetActive(false);
    }
    
}
