using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUITweenFx : MonoBehaviour
{

    [System.Serializable]
    public class RotateFx
    {
        public bool use;

        public TweenFunc tfunc = TweenFunc.linear;
        public TweenLoopType loopType = TweenLoopType.Loop;

        public float time = 0.3f;

        public int cycle = 0;

        public Vector3 from;
        public Vector3 to;

        [HideInInspector]
        public Coroutine coFunc;
    }

    [System.Serializable]
    public class ColorFx
    {
        public bool use;
        public TweenFunc tfunc = TweenFunc.linear;
        public TweenLoopType loopType;
        public float time = 0.3f;

        public int cycle;

        public Color from = Color.white;
        public Color to = Color.red;

        [HideInInspector]
        public Coroutine coFunc;
    }

    [System.Serializable]
    public class ScaleFx
    {
        public bool use;
        public TweenFunc tfunc = TweenFunc.linear;
        public TweenLoopType loopType;
        public float time = 0.3f;
        public int cycle;

        public Vector3 from = new Vector3(1, 1, 1);
        public Vector3 to = new Vector3(1.1f, 1.1f, 1.1f);

        [HideInInspector]
        public Coroutine coFunc;
    }

    [System.Serializable]
    public class FillFx
    {
        public bool use;
        public TweenFunc tfunc = TweenFunc.linear;
        public TweenLoopType loopType;
        public float time = 0.3f;
        public int cycle;

        public float from = 0;
        public float to = 1;

        [HideInInspector]
        public Coroutine coFunc;
    }

    [System.Serializable]
    public class MoveFx
    {
        public bool use;
        public TweenFunc tfunc = TweenFunc.linear;
        public TweenLoopType loopType;
        public float time = 0.3f;
        public int cycle;

        public Vector3 from = new Vector3(1, 1, 1);
        public Vector3 to = new Vector3(1.1f, 1.1f, 1.1f);

        [HideInInspector]
        public Coroutine coFunc;
    }

    public bool playAtStart;

    public RotateFx rotateFx;
    public ColorFx colorFx;
    public ScaleFx scaleFx;
    public FillFx fillFx;
    public MoveFx moveFx;

    private Text targetText;
    private Image targetImage;
    private Transform targetT;

	private CanvasGroup targetCanvas;

    public bool isPlaying { get ; private  set;}

    void Awake()
    {
        targetText = GetComponent<Text>();
        targetImage = GetComponent<Image>();
        targetT = transform;
		targetCanvas = GetComponent<CanvasGroup>();

    }
    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        if (playAtStart)
            Play();
    }

    void OnDisable()
    {
        Stop();
    }

    [ContextMenu("Play")]
    void PlayInEditor()
    {
        Play(true);
    }
   
    public void Play(bool reset= false)
    {
        Stop(reset);

        if (rotateFx.use && targetT != null)
            rotateFx.coFunc = StartCoroutine(PlayRotateFx());

        if (colorFx.use)
            colorFx.coFunc = StartCoroutine(PlayColorFx());

        if (scaleFx.use)
            scaleFx.coFunc = StartCoroutine(PlayScaleFx());

        if (fillFx.use)
            fillFx.coFunc = StartCoroutine(PlayFillFx());

         if (moveFx.use)
            moveFx.coFunc = StartCoroutine(PlayMoveFx());

         isPlaying = true;
    }

    public void Stop(bool reset = false)
    {
         isPlaying = false;
         
        if (rotateFx.coFunc != null)
            StopCoroutine(rotateFx.coFunc);

        if (colorFx.coFunc != null)
            StopCoroutine(colorFx.coFunc);

        if (scaleFx.coFunc != null)
            StopCoroutine(scaleFx.coFunc);

        if (fillFx.coFunc != null)
            StopCoroutine(fillFx.coFunc);

        if (moveFx.coFunc != null)
            StopCoroutine(moveFx.coFunc);

        rotateFx.coFunc = null;
        colorFx.coFunc = null;
        scaleFx.coFunc = null;
        fillFx.coFunc = null;
        moveFx.coFunc = null;


        if (reset)
        {
            if (rotateFx.use && targetT != null)
                targetT.localRotation = Quaternion.Euler(rotateFx.from);

            if (colorFx.use)
            {
                if (targetImage != null)
                    targetImage.color = colorFx.from;

                if (targetText != null)
                    targetText.color = colorFx.from;

				if(targetCanvas !=null)
					targetCanvas.alpha = colorFx.from.a;
            }

            if (scaleFx.use)
            {
                if (targetT != null)
                    targetT.localScale = scaleFx.from;
            }

            if (fillFx.use)
            {
                if (targetImage != null)
                    targetImage.fillAmount = fillFx.from;
            }

            if (moveFx.use)
            {
               if (targetT != null)
                    targetT.localPosition = moveFx.from;
            }
        }
    }

    IEnumerator PlayRotateFx()
    {
        TweenUtil t = new TweenUtil(rotateFx.time, rotateFx.cycle == 0 ? 100000 : rotateFx.cycle, rotateFx.loopType);
        t.Trigger();
        do
        {
            yield return null;
            t.SetCycleTime(rotateFx.time);
            t.SetLoopType(rotateFx.loopType);
            t.SetRepeat(rotateFx.cycle == 0 ? 100000 : rotateFx.cycle);

            var v = t.VectorValue(rotateFx.tfunc, rotateFx.from, rotateFx.to);
            targetT.localRotation = Quaternion.Euler(v);
        } while (t.On);
    }

    IEnumerator PlayColorFx()
    {
        TweenUtil t = new TweenUtil(colorFx.time, colorFx.cycle == 0 ? 100000 : colorFx.cycle, colorFx.loopType);
        t.Trigger();
        do
        {
            yield return null;

            t.SetCycleTime(colorFx.time);
            t.SetLoopType(colorFx.loopType);
            t.SetRepeat(colorFx.cycle == 0 ? 100000 : colorFx.cycle);

            var v = t.ColorValue(colorFx.tfunc, colorFx.from, colorFx.to);

            if (targetImage != null)
                targetImage.color = v;

            if (targetText != null)
                targetText.color = v;

			if(targetCanvas !=null)
				targetCanvas.alpha = v.a;
				
        } while (t.On);
    }


    IEnumerator PlayScaleFx()
    {
        TweenUtil t = new TweenUtil(scaleFx.time, scaleFx.cycle == 0 ? 100000 : scaleFx.cycle, scaleFx.loopType);
        t.Trigger();
        do
        {
            yield return null;

            t.SetCycleTime(scaleFx.time);
            t.SetLoopType(scaleFx.loopType);
            t.SetRepeat(scaleFx.cycle == 0 ? 100000 : scaleFx.cycle);

            var v = t.VectorValue(scaleFx.tfunc, scaleFx.from, scaleFx.to);

            if (targetT != null)
                targetT.localScale = v;
        } while (t.On);
    }

     IEnumerator PlayMoveFx()
    {
        TweenUtil t = new TweenUtil(scaleFx.time, scaleFx.cycle == 0 ? 100000 : scaleFx.cycle, scaleFx.loopType);
        t.Trigger();
        do
        {
            yield return null;

            t.SetCycleTime(moveFx.time);
            t.SetLoopType(moveFx.loopType);
            t.SetRepeat(moveFx.cycle == 0 ? 100000 : moveFx.cycle);

            var v = t.VectorValue(moveFx.tfunc, moveFx.from, moveFx.to);

            if (targetT != null)
                targetT.localPosition = v;
                
        } while (t.On);
    }

    IEnumerator PlayFillFx()
    {
        TweenUtil t = new TweenUtil(fillFx.time, fillFx.cycle == 0 ? 100000 : fillFx.cycle, fillFx.loopType);
        t.Trigger();
        do
        {
            yield return null;

            t.SetCycleTime(fillFx.time);
            t.SetLoopType(fillFx.loopType);
            t.SetRepeat(fillFx.cycle == 0 ? 100000 : fillFx.cycle);


            var v = t.FloatValue(fillFx.tfunc, fillFx.from, fillFx.to);

            if (targetImage != null)
                targetImage.fillAmount = v;

        } while (t.On);
    }
}
