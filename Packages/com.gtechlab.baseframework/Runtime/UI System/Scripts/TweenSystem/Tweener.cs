using UnityEngine;
using System.Collections;

public enum TweenFunc
{
    ONOFF,
    POW_FAST,
    POW_SLOW,
    POW32_FAST,
    POW32_SLOW,

    easeInQuad,
    easeOutQuad,
    easeInOutQuad,
    easeInCubic,
    easeOutCubic,
    easeInOutCubic,
    easeInQuart,
    easeOutQuart,
    easeInOutQuart,
    easeInQuint,
    easeOutQuint,
    easeInOutQuint,
    easeInSine,
    easeOutSine,
    easeInOutSine,
    easeInExpo,
    easeOutExpo,
    easeInOutExpo,
    easeInCirc,
    easeOutCirc,
    easeInOutCirc,
    linear,
    spring,
    /* GFX47 MOD START */
    //bounce,
    easeInBounce,
    easeOutBounce,
    easeInOutBounce,
    /* GFX47 MOD END */
    easeInBack,
    easeOutBack,
    easeInOutBack,
    /* GFX47 MOD START */
    //elastic,
    easeInElastic,
    easeOutElastic,
    easeInOutElastic,
    /* GFX47 MOD END */
    punch
};


/****************************************************
 * 
 * 
 * **************************************************/

public enum TweenLoopType
{
    Loop,
    PingPong,
    Once
}

[System.Serializable]
public class TweenUtil
{
    public float m_cycleTime;
    public float m_cycleRepeat;
    public TweenLoopType m_loopType;
    public TweenFunc m_tweenFunc;

    private bool m_on = false;
    private float m_triggerTime;

    private bool use_realtime = false;

    public bool On { get { return m_on; } }

    //private float m_remainedCycleTime;
    //private float m_remainedCycleRepeat;
    public TweenUtil()
    {

    }
     public TweenUtil(float cycleTime,  bool use_realtime)
    {
        m_loopType = TweenLoopType.Loop;
        m_cycleTime = cycleTime;
        m_cycleRepeat = 1;
        this.use_realtime = use_realtime;

        Trigger(false);
    }

    public TweenUtil(float cycleTime, int cycleRepeat = 1, TweenLoopType loopType = TweenLoopType.Loop, bool use_realtime=false)
    {
        m_loopType = loopType;
        m_cycleTime = cycleTime;
        m_cycleRepeat = cycleRepeat;
        this.use_realtime = use_realtime;

        Trigger(false);
    }

    public void SetTweener(float cycleTime, int cycleRepeat = 1, TweenLoopType loopType = TweenLoopType.Loop,bool use_realtime=false)
    {
        m_loopType = loopType;
        m_cycleTime = cycleTime;
        m_cycleRepeat = cycleRepeat;
        this.use_realtime = use_realtime;
    }

    public void Trigger(bool on = true)
    {
        if (on)
        {
            m_on = true;
            m_triggerTime = Time.time;
        }
        else
        {
            m_on = false;
            m_triggerTime = 0;
        }
    }

    public void Trigger(float cycleTime, int cycleRepeat = 1, TweenLoopType loopType = TweenLoopType.Loop)
    {
        m_loopType = loopType;
        m_cycleTime = cycleTime;
        m_cycleRepeat = cycleRepeat;

        Trigger(true);
    }

    public void SetCycleTime(float cycleTime)
    {
        m_cycleTime = cycleTime;
    }
    public void SetRepeat(int repeat)
    {
        m_cycleRepeat = repeat;
    }
    public void SetLoopType(TweenLoopType loopType)
    {
        m_loopType = loopType;
    }


    public float FloatValue(TweenFunc funcType, float fromValue, float toValue)
    {
        float val = toValue;

        if (m_on)
        {
            float normalizedTime = update_time();
            val = Func(funcType, fromValue, toValue, normalizedTime);
        }

        return val;
    }

    public Vector3 VectorValue(TweenFunc funcType, Vector3 fromValue, Vector3 toValue)
    {
        Vector3 val = toValue;

        if (m_on)
        {
            float normalizedTime = update_time();
            val.x = Func(funcType, fromValue.x, toValue.x, normalizedTime);
            val.y = Func(funcType, fromValue.y, toValue.y, normalizedTime);
            val.z = Func(funcType, fromValue.z, toValue.z, normalizedTime);
        }

        return val;
    }

    public Color ColorValue(TweenFunc funcType, Color fromValue, Color toValue)
    {
        Color val = toValue;

        if (m_on)
        {
            float normalizedTime = update_time();
            val.r = Func(funcType, fromValue.r, toValue.r, normalizedTime);
            val.g = Func(funcType, fromValue.g, toValue.g, normalizedTime);
            val.b = Func(funcType, fromValue.b, toValue.b, normalizedTime);
            val.a = Func(funcType, fromValue.a, toValue.a, normalizedTime);
        }

        return val;
    }

    private float update_time()
    {
        float normalizedTime = 1.0f;
        float repeatCycle = m_loopType == TweenLoopType.Once ? 1 : m_cycleRepeat;

        if (m_on)
        {
            float curTime = use_realtime ? Time.realtimeSinceStartup : Time.time;
            float elapsedTime = curTime - m_triggerTime;

            int curCycle = (int)(elapsedTime / m_cycleTime) + 1;
            if (curCycle > repeatCycle)
            {
                Trigger(false);
            }
            else
            {
                normalizedTime = elapsedTime % m_cycleTime;
                normalizedTime /= m_cycleTime;

                if (normalizedTime >= 1.0f)
                {
                    Debug.Log("[" + Time.frameCount + "]" + " NormalizedTime=" + normalizedTime + ", Cycle=" + curCycle);
                }

                if (m_loopType == TweenLoopType.PingPong && (curCycle % 2) == 0)
                    normalizedTime = 1.0f - normalizedTime;
            }
        }

        return normalizedTime;
    }

    public static float Func(TweenFunc funcType, float fromValue, float toValue, float normalizedTime)
    {
        float f = 1.0f;
        switch (funcType)
        {
            case TweenFunc.ONOFF:
                f = update_onoff(normalizedTime);
                break;
            case TweenFunc.POW_FAST:
                f = update_pow_fast(normalizedTime, 8);
                break;
            case TweenFunc.POW_SLOW:
                f = update_pow_slow(normalizedTime, 8);
                break;
            case TweenFunc.POW32_FAST:
                f = update_pow_fast(normalizedTime, 32);
                break;
            case TweenFunc.POW32_SLOW:
                f = update_pow_slow(normalizedTime, 32);
                break;
            /* Ease Function */
            case TweenFunc.easeInQuad:
                f = easeInQuad(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutQuad:
                f = easeOutQuad(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutQuad:
                f = easeInOutQuad(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInCubic:
                f = easeInCubic(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutCubic:
                f = easeOutCubic(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutCubic:
                f = easeInOutCubic(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInQuart:
                f = easeInQuart(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutQuart:
                f = easeOutQuart(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutQuart:
                f = easeInOutQuart(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInQuint:
                f = easeInQuint(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutQuint:
                f = easeOutQuint(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutQuint:
                f = easeInOutQuint(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInSine:
                f = easeInSine(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutSine:
                f = easeOutSine(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutSine:
                f = easeInOutSine(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInExpo:
                f = easeInExpo(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutExpo:
                f = easeOutExpo(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutExpo:
                f = easeInOutExpo(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInCirc:
                f = easeInCirc(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutCirc:
                f = easeOutCirc(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutCirc:
                f = easeInOutCirc(0, 1, normalizedTime);
                break;
            case TweenFunc.linear:
                f = linear(0, 1, normalizedTime);
                break;
            case TweenFunc.spring:
                f = spring(0, 1, normalizedTime);
                break;
            /* GFX47 MOD START */
            //bounce,
            case TweenFunc.easeInBounce:
                f = easeInBounce(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutBounce:
                f = easeOutBounce(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutBounce:
                f = easeInOutBounce(0, 1, normalizedTime);
                break;
            /* GFX47 MOD END */
            case TweenFunc.easeInBack:
                f = easeInBack(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutBack:
                f = easeOutBack(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutBack:
                f = easeInOutBack(0, 1, normalizedTime);
                break;
            /* GFX47 MOD START */
            //elastic,
            case TweenFunc.easeInElastic:
                f = easeInElastic(0, 1, normalizedTime);
                break;
            case TweenFunc.easeOutElastic:
                f = easeOutElastic(0, 1, normalizedTime);
                break;
            case TweenFunc.easeInOutElastic:
                f = easeInOutElastic(0, 1, normalizedTime);
                break;
            /* GFX47 MOD END */
            case TweenFunc.punch:
                f = punch(1, normalizedTime);
                break;
            default:
                break;
        }

        float val = (toValue - fromValue) * f + fromValue;

        return val;
    }

    private static float update_onoff(float normalizedTime)
    {
        return (normalizedTime <= 0.5f) ? 1.0f : 0.0f;
    }

    private static float update_pow_fast(float normalizedTime, float pow)
    {
        float p = 1 - Mathf.Pow((normalizedTime - 1), pow);
        return p;
    }

    private static float update_pow_slow(float normalizedTime, float pow)
    {
        float p = Mathf.Pow(normalizedTime, pow);
        return p;
    }

    /******************************************************************
	 * 
	 * 
	 * ****************************************************************/
    private static float linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    private static float clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    private static float spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    private static float easeInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    private static float easeOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    private static float easeInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value + start;
        value--;
        return -end / 2 * (value * (value - 2) - 1) + start;
    }

    private static float easeInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    private static float easeOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    private static float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    private static float easeInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    private static float easeOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    private static float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    private static float easeInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    private static float easeOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    private static float easeInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value * value * value + 2) + start;
    }

    private static float easeInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
    }

    private static float easeOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
    }

    private static float easeInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
    }

    private static float easeInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
    }

    private static float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }

    private static float easeInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    private static float easeInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    private static float easeOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    private static float easeInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    /* GFX47 MOD START */
    private static float easeInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - easeOutBounce(0, end, d - value) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float bounce(float start, float end, float value){
    private static float easeOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    private static float easeInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d / 2) return easeInBounce(0, end, value * 2) * 0.5f + start;
        else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }
    /* GFX47 MOD END */

    private static float easeInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    private static float easeOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value / 1) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    private static float easeInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    private static float punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    /* GFX47 MOD START */
    private static float easeInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float elastic(float start, float end, float value){
    private static float easeOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    /* GFX47 MOD START */
    private static float easeInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d / 2) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
    /* GFX47 MOD END */

}