using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class cxFxPlayer : MonoBehaviour
{

    public enum EmitDir
    {
        ToSky,
        ToObjectFoward
    }

    public enum FxType
    {
        Particle,
        Audio,
        TextMesh,
        AnimationPlay,
        TimelinePlay,
        Object,

        Active,
        Inactive,
        EventCall,
        SelfDestroy,

        HapticFx,
    }

    [System.Serializable]
    public class FxUnit
    {
        public float delay;
        public FxType fxType;

        public bool attachTo;
        public Vector3 attachOffset;

        public bool instance;

        public EmitDir emitDir;
        public GameObject prefabFx;

        public GameObject prefabTextFx;
        public AudioClip audioClip;

        [Header("Timeline Player")]
        public PlayableDirector timelinePlayer;
        public PlayableAsset timelineAsset;

        [Header("Animator Player")]
        public Animator animator;
        public string animationName;

        public GameObject activeTarget;

        public string eventFuncName; 

        //[Header("Haptic Fx")]
        //public afHapticFxType hapticFx;


        [HideInInspector]
        public GameObject managedParticleFxObj;

        [HideInInspector]
        public GameObject managedTextFxObj;
        [HideInInspector]
        public TMPro.TMP_Text[] managedTexts;
    }

    [System.Serializable]
    public class FxComposer
    {
        public string composerName;
        public int fxPointId;
        public bool managed;

        [HideInInspector]
        public int composerNameToHash;

        //public Transform fxPoint;

        public List<FxUnit> fxList;


    }

    public enum FxPointDir
    {
        ObjectForward,
        ObjectBackward,
        ObjectUp,
        ObjectDown,
        ObjectRight,
        ObjectLeft,
    }

    /*
    public class ManagedFx
    {
        public FxComposer fxComposer;
        public FxUnit fxUnit;

		public List<GameObject> particleFxObj;
		public List<GameObject> textFxObj;
		public TMPro.TMP_Text[] texts;
	}
	*/

    [System.Serializable]
    public class FxPointT
    {
        public string pointName;
        public FxPointDir fowardDir;
        //public int pointId;

        public Transform fxParentT;
        public Transform fxPoint;
    }


    public float m_modelScale = 1.0f;

    public Transform m_modelRoot;
    public Animator m_fxAnimator;

   // public PlayableDirector m_timelinePlayer;

    public List<FxPointT> m_pointList;
    public FxComposer[] m_composerList;

    AudioSource m_audioSource;

    MecanimStateMachine m_mecanimSM;

    void Awake()
    {
        m_audioSource = transform.root.GetComponentInChildren<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < m_composerList.Length; i++)
        {
            m_composerList[i].composerNameToHash =
                                 Animator.StringToHash(m_composerList[i].composerName);
        }

        if (m_fxAnimator != null)
            m_mecanimSM = new MecanimStateMachine(m_fxAnimator, 0, OnAnimatorStateMachine);

    }

    public void SetAnimator(Animator ani)
    {
        if (ani == null)
        {
            m_fxAnimator = null;
            m_mecanimSM = null;
        }
        else
        {
            m_fxAnimator = ani;
            m_mecanimSM = new MecanimStateMachine(m_fxAnimator, 0, OnAnimatorStateMachine);
        }
    }
    public void StopAll()
    {
        for (int i = 0; i < m_composerList.Length; i++)
        {
            var fxComposer = m_composerList[i];

            for (int k = 0; k < fxComposer.fxList.Count; k++)
            {
                var fx = fxComposer.fxList[k];
                if (fx.managedParticleFxObj != null)
                {
                    GameObject.Destroy(fx.managedParticleFxObj);
                    fx.managedParticleFxObj = null;
                }
            }
        }
    }

    private void Update()
    {
        if (m_mecanimSM != null)
            m_mecanimSM.UpdateStateMachine();
    }

    public string FindHashNameToComposer(int hashName)
    {
        for (int i = 0; i < m_composerList.Length; i++)
            if (m_composerList[i].composerNameToHash == hashName)
                return m_composerList[i].composerName;

        return null;
    }

    [ContextMenu("Setup AttachPoint")]
    void SetupAttachPoint()
    {
        Transform root = m_modelRoot;

        foreach (var p in m_pointList)
        {
          //  p.fxParentT = KettaBodySetup.FindChild(root, p.pointName);

            if (p.fxParentT == null)
            {
                Debug.LogWarning("SetupAttachPoint:" + p.pointName);
            }
            else
            {
                if (p.fxPoint != null &&
                   p.fxPoint.name.StartsWith("fxPoint_id"))
                {
                    GameObject.DestroyImmediate(p.fxPoint.gameObject);
                    p.fxPoint = null;
                }

                var point = new GameObject("fxPoint_id_" + p.pointName);

                point.transform.SetParent(p.fxParentT);

                p.fxPoint = point.transform;
                p.fxPoint.localPosition = Vector3.zero;

                Quaternion rot = Quaternion.identity;

                if (p.fowardDir == FxPointDir.ObjectForward)
                    p.fxPoint.rotation = Quaternion.LookRotation(root.forward);
                else if (p.fowardDir == FxPointDir.ObjectBackward)
                    p.fxPoint.rotation = Quaternion.LookRotation(-root.forward);
                else if (p.fowardDir == FxPointDir.ObjectLeft)
                    p.fxPoint.rotation = Quaternion.LookRotation(-root.right);
                else if (p.fowardDir == FxPointDir.ObjectRight)
                    p.fxPoint.rotation = Quaternion.LookRotation(root.right);
                else if (p.fowardDir == FxPointDir.ObjectUp)
                    p.fxPoint.rotation = Quaternion.LookRotation(root.up);
                else if (p.fowardDir == FxPointDir.ObjectDown)
                    p.fxPoint.rotation = Quaternion.LookRotation(-root.up);
            }
        }
    }

    void PlayInEditor(int i)
    {
        if (Application.isPlaying)
            if (i < m_composerList.Length)
                PlayFx(m_composerList[i], string.Empty);
    }

    FxComposer Find(string fxComposerName)
    {
        for (int i = 0; i < m_composerList.Length; i++)
        {
            if (m_composerList[i].composerName == fxComposerName)
            {
                return m_composerList[i];

            }
        }
        Debug.LogWarning("FxPlayer.Play not found:" + fxComposerName);

        return null;
    }

    public void StopPlay(FxComposer fxComposer)
    {
        for (int i = 0; i < fxComposer.fxList.Count; i++)
        {
            var fx = fxComposer.fxList[i];
            if (fx.managedParticleFxObj != null)
            {
                var psList = fx.managedParticleFxObj.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in psList)
                    ps.Stop();

                var lights = fx.managedParticleFxObj.GetComponentsInChildren<Light>();
                foreach (var l in lights)
                    l.enabled = false;

                if (fx.fxType == FxType.Object)
                {
                    Destroy(fx.managedParticleFxObj);
                    fx.managedParticleFxObj = null;
                }

                if(fx.fxType == FxType.Active || fx.fxType == FxType.Inactive)
                    fx.activeTarget.SetActive(fx.fxType != FxType.Active);
            }
        }
    }

    public class FxPosition
    {
        public Vector3 pos;
        public Vector3 dir;
    }

    public FxComposer Play(string fxComposerName, string text, Vector3 pos, Vector3 dir)
    {
        FxPosition customPos = new FxPosition
        {
            pos = pos,
            dir = dir,
        };


        var composer = Find(fxComposerName);
        if (composer != null)
        {
            PlayFx(composer, text, customPos);
            return composer;
        }
        else
            return null;
    }


    public FxComposer Play(string fxComposerName, string text)
    {
        var composer = Find(fxComposerName);
        if (composer != null)
        {
            PlayFx(composer, text, null);
            return composer;
        }
        else
            return null;
    }

    public FxComposer Play(string fxComposerName)
    {
        var composer = Find(fxComposerName);
        if (composer != null)
        {
            PlayFx(composer, string.Empty, null);
            return composer;
        }
        else
        {
            return null;
        }
    }

    public void Stop(string fxComposerName)
    {
        var composer = Find(fxComposerName);
        if (composer != null)
        {
            StopPlay(composer);
        }
    }

    void PlayFx(FxComposer fxComposer, string text, FxPosition fxPosition = null)
    {
        for (int i = 0; i < fxComposer.fxList.Count; i++)
            StartCoroutine(CoPlayUnit(fxComposer, fxComposer.fxList[i], text, fxPosition));
    }

    IEnumerator CoPlayUnit(FxComposer fxComposer, FxUnit fxUnit, string text, FxPosition customPos)
    {
        if (fxUnit.delay > 0)
            yield return new WaitForSeconds(fxUnit.delay);

        if (fxUnit.fxType == FxType.Particle || fxUnit.fxType == FxType.Object)
            if (fxUnit.prefabFx != null)
                PlayParticle(fxComposer, fxUnit, customPos);

        if (fxUnit.fxType == FxType.TextMesh)
            if (fxUnit.prefabTextFx != null)
                PlayText(fxComposer, fxUnit, text);

        if (fxUnit.fxType == FxType.Audio)
            if (fxUnit.audioClip != null)
                PlaySound(fxUnit.audioClip);

        if (fxUnit.fxType == FxType.TimelinePlay)
        {
            if (fxUnit.timelineAsset && fxUnit.timelinePlayer != null)
                fxUnit.timelinePlayer.Play(fxUnit.timelineAsset);
        }

        if (fxUnit.fxType == FxType.AnimationPlay)
        {
            if (!string.IsNullOrEmpty(fxUnit.animationName) && fxUnit.animator != null)
                fxUnit.animator.Play(fxUnit.animationName, 0,0);
        }

        if(fxUnit.fxType == FxType.Active || fxUnit.fxType == FxType.Inactive)
            fxUnit.activeTarget.SetActive(fxUnit.fxType == FxType.Active);

        // if(fxUnit.fxType ==FxType.EventCall && fxUnit.eventFunc!=null)
        //      fxUnit.eventFunc.Invoke();
   
        if(fxUnit.fxType ==FxType.EventCall && fxUnit.activeTarget!=null)
            fxUnit.activeTarget.SendMessage(fxUnit.eventFuncName, SendMessageOptions.RequireReceiver);

        if(fxUnit.fxType == FxType.SelfDestroy)
            GameObject.Destroy(gameObject);

        //if(fxUnit.fxType == FxType.HapticFx)
        //    afHapticFx.PlayHaptic(fxUnit.hapticFx);
    }

    void PlaySound(AudioClip clip)
    {
        if (m_audioSource != null)
            m_audioSource.PlayOneShot(clip);
    }


    Transform GetAttachPoint(FxComposer composer)
    {
        if (composer.fxPointId < 0 || composer.fxPointId >= m_pointList.Count)
        {
            Debug.LogWarning("GetAttachPoint attach not found:" + composer.fxPointId);
            return transform;
        }
        else
        {
            var point = m_pointList[composer.fxPointId];
            return point.fxPoint;
        }
    }

    void PlayText(FxComposer comper, FxUnit fxUnit, string text)
    {
        GameObject fx;
        TMPro.TMP_Text[] texts;

        var attachT = GetAttachPoint(comper);
        Quaternion rot = attachT.rotation;

        if (fxUnit.emitDir == EmitDir.ToSky)
            rot = Quaternion.LookRotation(-Vector3.up);
        else if (fxUnit.emitDir == EmitDir.ToObjectFoward)
            rot = Quaternion.LookRotation(-attachT.forward, Vector3.up);//.rotation;

        Vector3 pos = attachT.position + rot * fxUnit.attachOffset * m_modelScale;

        if (fxUnit.managedTextFxObj == null)
        {
            fx = GameObject.Instantiate(fxUnit.prefabTextFx, pos, rot) as GameObject;
            // fx.transform.localScale = fx.transform.localScale * m_modelScale;
            fx.transform.localScale = transform.localScale * m_modelScale;

            if (fxUnit.attachTo)
                fx.transform.SetParent(GetAttachPoint(comper), true);

            texts = fx.GetComponentsInChildren<TMPro.TMP_Text>();

            if (!fxUnit.instance)
            {
                fxUnit.managedTextFxObj = fx;
                fxUnit.managedTexts = texts;
            }
        }
        else
        {
            fx = fxUnit.managedTextFxObj;
            texts = fxUnit.managedTexts;
        }

        foreach (var t in texts)
            t.text = text;
    }


    void PlayParticle(FxComposer comper, FxUnit fxUnit, FxPosition customPos)
    {
        GameObject ps;

        var attachT = GetAttachPoint(comper);
        Quaternion rot = attachT.rotation;

        if (fxUnit.emitDir == EmitDir.ToSky)
            rot = Quaternion.LookRotation(Vector3.up);
        else if (fxUnit.emitDir == EmitDir.ToObjectFoward)
            rot = attachT.rotation;

        Vector3 pos = attachT.position + rot * fxUnit.attachOffset * m_modelScale;

        if (customPos != null)
        {
            rot = Quaternion.LookRotation(customPos.dir);
            pos = customPos.pos;
        }

        if (fxUnit.managedParticleFxObj == null)
        {
            ps = GameObject.Instantiate(fxUnit.prefabFx, pos, rot) as GameObject;

            //ps.transform.localScale = ps.transform.localScale * m_modelScale;
            ps.transform.localScale = transform.localScale * m_modelScale;

            if (fxUnit.attachTo)
                ps.transform.SetParent(attachT, true);

            if (!fxUnit.instance)
                fxUnit.managedParticleFxObj = ps;
        }
        else
        {
            ps = fxUnit.managedParticleFxObj;
            ps.transform.position = pos;
            ps.transform.rotation = rot;
        }

        var psList = ps.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in psList)
            p.Play(true);

        var lights = ps.GetComponentsInChildren<Light>();
        foreach (var l in lights)
            l.enabled = true;


        //foreach (var p in psList)
        //{
        //    p.transform.localScale = Vector3.one; 
        //   // p.Play(true);
        //}
    }

    /////

    cxFxPlayer.FxComposer m_activeMotionfxComposer = null;

    bool OnAnimatorStateMachine(int stateId, FSMMsg msg, int param1, object param2)
    {
        if (msg == FSMMsg.OnEnter)
        {
            m_activeMotionfxComposer = null;

            string composerName = FindHashNameToComposer(stateId);
            if (!string.IsNullOrEmpty(composerName))
            {
                Debug.Log("OnAnimatorStateMachine fxPlay:" + composerName);

                var c = Play(composerName);

                if (c.managed)
                    m_activeMotionfxComposer = c;
            }
        }
        else if (msg == FSMMsg.OnExit)
        {
            if (m_activeMotionfxComposer != null)
            {
                StopPlay(m_activeMotionfxComposer);

                Debug.Log("OnAnimatorStateMachine fxStop:" + m_activeMotionfxComposer.composerName);
            }

            m_activeMotionfxComposer = null;
        }
        else
            return false;

        return true;
    }
}
