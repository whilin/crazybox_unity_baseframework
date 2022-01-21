using UnityEngine;
using System.Collections;

public class UIImpactText : MonoBehaviour
{
    public GameObject root;
    public UILabel front;
    public UILabel back;
    public float duration;
    public float glowMinScale = 1.0f;
    public float glowMaxScale = 1.5f;

    private TweenUtil impactTweener = new TweenUtil(1.0f);
    private int width;
    private int height;

    // Use this for initialization
    void Start()
    {
        if (front != null)
        {
            width = (int)front.transform.localScale.x;
            height = (int)front.transform.localScale.y;
        }
        if (back != null)
        {
            width = (int)back.transform.localScale.x;
            height = (int)back.transform.localScale.y;
        }

        NGUITools.SetActive(root, false);
    }

    public void FireImpact()
    {
        NGUITools.SetActive(root, true);
    }

    public void FireImpact(string text)
    {
        FireImpact(text, Color.white);
    }

    public void FireImpact(string text, Color c)
    {
        impactTweener.SetTweener(duration);
        impactTweener.Trigger();

        if (front != null)
        {
            front.text = text;
            front.color = c;
        }

        if (back != null)
        {
            back.text = text;
            back.color = c;
        }

        NGUITools.SetActive(root, true);
    }

    void Update()
    {
        if (!impactTweener.On)
            return;

        float s;
        float a;

        if (front != null)
        {
            s = impactTweener.FloatValue(TweenFunc.POW32_FAST, 0.75f, 1.0f);
            //a = impactTweener.FloatValue(TweenFunc.POW_SLOW, 1.0f, 0.0f);
            a = impactTweener.FloatValue(TweenFunc.easeInQuad, 1.0f, 0.0f);

            front.cachedTransform.localScale = new Vector3(width * s, height * s, 1.0f);
            front.alpha = a;

            s = impactTweener.FloatValue(TweenFunc.POW_FAST, glowMinScale, glowMaxScale);
            a = impactTweener.FloatValue(TweenFunc.POW_FAST, 1.0f, 0.0f);

            back.cachedTransform.localScale = new Vector3(width * s, height * s, 1.0f);
            back.alpha = a;
        }
        else //only back
        {
            s = impactTweener.FloatValue(TweenFunc.POW_FAST, glowMinScale, glowMaxScale);
            a = impactTweener.FloatValue(TweenFunc.easeOutQuad, 1.0f, 0.0f);

            back.cachedTransform.localScale = new Vector3(width * s, height * s, 1.0f);
            back.alpha = a;
        }

        if (!impactTweener.On)
            NGUITools.SetActive(root, false);
    }
}
