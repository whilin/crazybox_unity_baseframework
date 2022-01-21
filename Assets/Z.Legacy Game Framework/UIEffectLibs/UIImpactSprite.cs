using UnityEngine;
using System.Collections;

public class UIImpactSprite : MonoBehaviour 
{
    public GameObject root;
    public UISprite front;
    public UISprite back;
    public float duration;

    private TweenUtil impactTweener = new TweenUtil(1.0f);
    private int width;
    private int height;

    // Use this for initialization
    void Start()
    {
        width = (int)front.transform.localScale.x;
        height = (int)front.transform.localScale.y;

        NGUITools.SetActive(root, false);
    }

    public bool ImpactOn()
    {
        return impactTweener.On;
    }

    public void FireImpact()
    {
        NGUITools.SetActive(root, true);
    }

    public void FireImpact(string spriteName, float duration, float scaleFactor=1.0f)
    {
        impactTweener.SetTweener(duration);
        impactTweener.Trigger();

        front.spriteName = spriteName;
        back.spriteName = spriteName;
        front.MakePixelPerfect();
        back.MakePixelPerfect();

        width = (int) (front.transform.localScale.x * scaleFactor);
        height = (int)(front.transform.localScale.y * scaleFactor);

        NGUITools.SetActive(root, true);
    }

    void Update()
    {
        if (!impactTweener.On)
            return;

        float s;
        float a;

        s = impactTweener.FloatValue(TweenFunc.POW32_FAST, 0.75f, 1.0f);
        a = impactTweener.FloatValue(TweenFunc.POW_SLOW, 1.0f, 0.0f);

        front.cachedTransform.localScale = new Vector3(width * s, height * s, 1.0f);
        front.alpha = a;

        s = impactTweener.FloatValue(TweenFunc.POW_FAST, 1.0f, 1.5f);
        a = impactTweener.FloatValue(TweenFunc.POW_FAST, 1.0f, 0.0f);

        back.cachedTransform.localScale = new Vector3(width * s, height * s, 1.0f);
        back.alpha = a;

        if (!impactTweener.On)
            NGUITools.SetActive(root, false);
    }
}
