using UnityEngine;
using System.Collections;

public class UIImpactObject : MonoBehaviour {

    public UISprite front;
    public UISprite back;
    public UILabel label;

    public float duration=1;
    public float flyHeight = 200;

    private TweenUtil impactTweener = new TweenUtil(1.0f);
    private float scale;
    private Vector3 startPos;
    private Vector3 endPos;

    // Use this for initialization
    void Start()
    {
        scale = 1;
    }

    public void FireImpact()
    {
    }

    public void FireImpact(Vector3 impactPos, float damage)
    {
        impactTweener.SetTweener(duration);
        impactTweener.Trigger();

        scale = 1;

        startPos = impactPos;
        endPos = impactPos;
        endPos.y += flyHeight;

        label.text = string.Empty + ((int)damage);
        transform.position = startPos;
    }

    void Update()
    {
        if (!impactTweener.On)
            return;

        float s = impactTweener.FloatValue(TweenFunc.POW32_FAST, 0.75f, 1.0f);
        //float s = impactTweener.FloatValue(TweenFunc.easeInQuad, 1.0f, 0.5f);
        float a = impactTweener.FloatValue(TweenFunc.POW_SLOW, 1.0f, 0.0f);
        Vector3 pos = impactTweener.VectorValue(TweenFunc.easeInQuad, startPos, endPos);

        transform.position = pos;
        transform.localScale = new Vector3(scale * s, scale * s, 1.0f);
        
        front.alpha = a;
        label.alpha = a;

        if (!impactTweener.On)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
