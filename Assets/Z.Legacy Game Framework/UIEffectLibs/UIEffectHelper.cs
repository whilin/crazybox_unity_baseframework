using UnityEngine;
using System.Collections;

public class UIEffectHelper : MonoBehaviour {

    public enum UIEffectType
    {
        ShowFromLeft,
        ShowFromRight,
        ShowFromUp,
        ShowFromDown,

        ShowScaleX,
        ShowScaleY,
        ShowScaleXY,
            
        ShowStamp,
        ShowAlpha,

        LoopRotate,
        LoopShake,

        Undef=100,
    }

    [System.Serializable]
    public class UIEffectParam
    {
        public UIEffectType type = UIEffectType.Undef;
        public GameObject anchor;
        public float delay;
        public TweenFunc tfunc = TweenFunc.easeOutQuad;
        public float duration=0.5f;
    }

    public bool m_auto = false; 
    public UIEffectParam m_param;
    public UIEffectParam m_hideParam;

    private Vector3 initPosition;
    private Vector3 initScale;
    private UIPanel initPanel;

    void Awake()
    {
        initPanel = GetComponent<UIPanel>();

        if (m_param.anchor != null)
        {
            initPosition = m_param.anchor.transform.localPosition;
            initScale = m_param.anchor.transform.localScale;
        }
        else
        {
            initPosition = transform.localPosition;
            initScale = transform.localScale;
        }
    }

    void OnEnable()
    {
        if (m_auto)
            Play();
    }

    void OnDisable()
    {
        
    }

    public void Play()
    {
        PlayUIEffect(gameObject, m_param, m_param.delay, initPosition, initScale, initPanel);
    }

    public void Play(float delay)
    {
        PlayUIEffect(gameObject, m_param, delay, initPosition, initScale, initPanel);
    }


    public void Play(UIEffectParam param)
    {
        PlayUIEffect(gameObject, param, param.delay, initPosition, initScale, initPanel);
    }

    public void Hide()
    {
        HideUIEffect(gameObject, m_hideParam, m_param.delay, initPosition, initScale, initPanel);
    }

    public static void Play(GameObject obj)
    {
        var comp = obj.GetComponent<UIEffectHelper>();
        if (comp != null)
            comp.Play();
    }

    public static void Hide(GameObject obj)
    {
        var comp = obj.GetComponent<UIEffectHelper>();
        if (comp != null)
            comp.Hide();
        else
            obj.SetActive(false);
    }

    public static void Play(GameObject obj, UIEffectParam param, float delay = 0.0f)
    {
        PlayUIEffect(obj, param, delay, Vector3.zero, Vector3.one,null);
    }

    /***************************************************************
     * 
     * Template
     * 
     * *************************************************************/

    /***************************************************************
     * 
     * Static !!
     * 
     * *************************************************************/
    private static void PlayUIEffect(GameObject gameObject, UIEffectParam param, float delay, 
                                        Vector3 initPosition, Vector3 initScale, UIPanel initPanel)
    {
        if (delay == 0)
            gameObject.SetActive(true);

        switch (param.type)
        {
            case UIEffectType.ShowFromLeft:
                {
                    UIEffectUtil.ShowFrom(UIEffectUtil.MovingDir.Left, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromRight:
                {
                    UIEffectUtil.ShowFrom(UIEffectUtil.MovingDir.Right, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromUp:
                {
                    UIEffectUtil.ShowFrom(UIEffectUtil.MovingDir.Up, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromDown:
                {
                    UIEffectUtil.ShowFrom(UIEffectUtil.MovingDir.Down, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleX:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, true, false, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleY:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, false, true, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleXY:
                {
                    UIEffectUtil.ShowScale(gameObject, true, delay, true, true, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowStamp:
                {
                    UIEffectUtil.Stamp(gameObject, delay, initScale, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowAlpha:
                {
                    UIEffectUtil.ShowAlpha(initPanel, true, delay, param.tfunc, param.duration);
                }
                break;
            //case UIEffectType.LoopRotate:
            //    {
            //        UIEffectUtil.LoopRotation(gameObject, delay, param.tfunc, param.duration);
            //    }
            //    break;
            //case UIEffectType.LoopShake:
            //    {
            //        UIEffectUtil.Shake(gameObject, new Vector2(1.2f, 1.2f));
            //    }
            //    break;
        }
    }

    private static void HideUIEffect(GameObject gameObject, UIEffectParam param, float delay,
                                       Vector3 initPosition, Vector3 initScale, UIPanel initPanel)
    {
        switch (param.type)
        {
            case UIEffectType.ShowFromLeft:
                {
                    UIEffectUtil.HideTo(UIEffectUtil.MovingDir.Left, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromRight:
                {
                    UIEffectUtil.HideTo(UIEffectUtil.MovingDir.Right, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromUp:
                {
                    UIEffectUtil.HideTo(UIEffectUtil.MovingDir.Up, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowFromDown:
                {
                    UIEffectUtil.HideTo(UIEffectUtil.MovingDir.Down, gameObject, initPosition, delay, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleX:
                {
                    UIEffectUtil.HideScale(gameObject, true, delay, true, false, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleY:
                {
                    UIEffectUtil.HideScale(gameObject, true, delay, false, true, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowScaleXY:
                {
                    UIEffectUtil.HideScale(gameObject, true, delay, true, true, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowStamp:
                {
                    //UIEffectUtil.Stamp(gameObject, delay, initScale, param.tfunc, param.duration);
                }
                break;
            case UIEffectType.ShowAlpha:
                {
                    UIEffectUtil.HideAlpha(initPanel, true, delay, param.tfunc, param.duration);
                }
                break;
            default:
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
