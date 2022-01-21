using UnityEngine;
using System.Collections;

public class UIShakeText : MonoBehaviour {

    public GameObject root;
    public UILabel front;
    public UILabel back;

    public bool hideAuto;
    public float hideDuration=2.0f;

    //public float shakeScale;
    //public float duration;
    //public TweenPosition frontPos;
    //public TweenAlpha frontAlpha;
    //public TweenAlpha backAlpha;

    // Use this for initialization
    void Start()
    {
        NGUITools.SetActive(root, !hideAuto);
    }

    public void FireImpact(string text)
    {
        NGUITools.SetActive(root, true);

        front.text = text;
        back.text = text;

        UITweener[] ts = front.GetComponents<UITweener>();
        foreach (UITweener t in ts)
        {
           // t.Reset();
            t.Play(true);
        }

        ts = back.GetComponents<UITweener>();
        foreach (UITweener t in ts)
        {
           // t.Reset();
            t.Play(true);
        }

        /*
        frontPos.from = new Vector3(-shakeScale, 0,0);
        frontPos.to = new Vector3(shakeScale, 0,0);
        frontPos.duration = duration;
        frontAlpha.duration = duration;

        backAlpha.duration = duration * 2;

        frontPos.Reset();
        frontAlpha.Reset();
        backAlpha.Reset();

        frontPos.Play(true);
        frontAlpha.Play(true);
        backAlpha.Play(true);
         */
    }

    IEnumerator HideAuto()
    {
        yield return new WaitForSeconds(hideDuration);
        NGUITools.SetActive(root, false);
    }
}
