using UnityEngine;
using System.Collections;

public class UIScoreText : MonoBehaviour {

    public UILabel title;

    public GameObject scoreEffect;
    public UILabel scoreText;
    public string frontTag;
    public string backTag;

    public int deltaValue = 5;

    //public EnergyBar scoreStar;

    /*
    public UIShakeText bonusScore;
    */
    

    int curScore = 0;
    int targetScore = 0;

    /*
    int lastLv = 0;
    float lastTime = 0.0f;
    int lastScore = 0;
    */

	// Use this for initialization
	void Start () 
    {
        NGUITools.SetActive(scoreEffect, true);

        // if (scoreStar != null)
        // {
        //     scoreStar.SetValueCurrent(0);
        //     NGUITools.SetActive(scoreStar.gameObject, true);
        // }
	}

    /*
    public void AddBonusScore(string text, int lv)
    {
        //Skip.....
        if ((lastTime + 3.0f) > Time.time)
            if (lastLv > lv)
                return;

        lastLv = lv;
        lastTime = Time.time;

        bonusScore.FireImpact(text);
    }
    */

    public void AddScore(int newScore)
    {
        int starPrev = 0;// QuestManager.Instance.CalcScoreToStar(targetScore);
        int starCur = 0;// QuestManager.Instance.CalcScoreToStar(newScore);
       
        if (newScore == 0)
        {
            targetScore = newScore;
            curScore = newScore;

            scoreText.text = frontTag+"0"+backTag;
            NGUITools.SetActive(scoreEffect, true);
        }
        else if (newScore > targetScore)
        {
            NGUITools.SetActive(scoreEffect, true);

            targetScore = newScore;
            StartCoroutine(DoEffect());
        }

        // if (scoreStar != null)
        //     scoreStar.SetValueCurrent(starCur * 100);
    }

    public void SetScore(string score, int prog=0)
    {
        scoreText.text = score;

        // if (scoreStar != null)
        //     scoreStar.SetValueCurrent(prog);

        StartCoroutine(DoTextEffect());
    }

    // bool doScaleEffect = false;
    TweenUtil t = new TweenUtil(0.3f, 1);

    IEnumerator DoEffect()
    {
        scoreText.color = Color.yellow;

        do
        {
            curScore += deltaValue;
            scoreText.text = frontTag + curScore + backTag;

            yield return new WaitForSeconds(0.02f);

        } while (curScore < targetScore);

        //UIEffectUtil.Shake(scoreEffect, new Vector2(20, 0));

        /*
        int initValue = curScore;
        int nextValue = targetScore;

        Tweener tw = new Tweener(2.0f);
        tw.Trigger();

        do
        {
            yield return null;

            int v = (int)tw.FloatValue(TweenFunc.linear, initValue, nextValue);
            score.text = frontTag + v + backTag;
        }
        while (tw.On);
        */

        curScore = targetScore;
        scoreText.text = frontTag + curScore + backTag;

        TweenUtil tw = new TweenUtil(0.3f);
        tw.Trigger();
        do
        {
            scoreText.color = tw.ColorValue(TweenFunc.easeOutQuad, Color.yellow, Color.white);
            yield return null;
        }
        while (tw.On);

        

        /*
        t.Trigger();
        yield return new WaitForSeconds(0.3f);

        scoreEffect.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        score.color = Color.white;
        */
    }

    IEnumerator DoTextEffect()
    {
        scoreText.color = Color.yellow;
      
        TweenUtil tw = new TweenUtil(0.3f, 10);
        tw.Trigger();
        do
        {
            scoreText.color = tw.ColorValue(TweenFunc.easeOutQuad, Color.yellow, Color.white);
            yield return null;
        }
        while (tw.On);
    }

    /*
    void Update()
    {
        if (t.On)
        {
            float scale = t.FloatValue(TweenFunc.easeOutQuad, 1.0f, 1.5f);
            scoreEffect.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
     */ 
}
