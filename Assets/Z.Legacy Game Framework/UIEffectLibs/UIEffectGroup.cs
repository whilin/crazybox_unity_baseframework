using UnityEngine;
using System.Collections;

public class UIEffectGroup: MonoBehaviour {

    [System.Serializable]
    public class EffectComp
    {
        public GameObject target;
        public UIEffectUtil.UIEffectFunc func;
        public float delay;

        [HideInInspector]
        public Vector3 initPosition;
         [HideInInspector]
        public Vector3 initScale;
    }

    public bool autoPlay=true;
    public UIEffectUtil [] effectUIList;

    public EffectComp[] procedureEffectList;

    bool init = false;

    void Awake()
    {
        foreach (var e in procedureEffectList)
        {
            e.initPosition = e.target.transform.localPosition;
            e.initScale = e.target.transform.localScale;
        }
    }

    void Start()
    {
        if (autoPlay)
        {
            foreach (var e in effectUIList)
                e.PlayUIEffect(true);

            foreach (var e2 in procedureEffectList)
                UIEffectUtil.PlayUIEffect(e2.target, e2.func, e2.delay, e2.initPosition, e2.initScale);
        }

        init = true;
    }

    void OnEnable()
    {
        if (init)
        {
            if (autoPlay)
            {
                foreach (var e in effectUIList)
                    e.PlayUIEffect(true);

                foreach (var e2 in procedureEffectList)
                    UIEffectUtil.PlayUIEffect(e2.target, e2.func, e2.delay, e2.initPosition, e2.initScale);
            }
        }
    }

    public void PlayUIEffect(bool show)
    {
        foreach (var e in effectUIList)
            e.PlayUIEffect(show);

        foreach (var e2 in procedureEffectList)
            UIEffectUtil.PlayUIEffect(e2.target, e2.func, e2.delay, e2.initPosition, e2.initScale);
    }
}
