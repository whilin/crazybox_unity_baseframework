using UnityEngine;
using System.Collections;

public class MovingEffect : PooledObject
{
    /*
    private ParticleEmitter[] emitters;
    private ParticleRenderer[] renderers;
    private ParticleAnimator[] animators;
    */

    override protected void Awake()
    {
        base.Awake();

        /*
        emitters = GetComponentsInChildren<ParticleEmitter>();
        renderers = GetComponentsInChildren<ParticleRenderer>();
        animators = GetComponentsInChildren<ParticleAnimator>();
        */
    }

    override public void OnPoolCreate()
    {
        base.OnPoolCreate();
        /*
        foreach (ParticleEmitter emitter in emitters)
        {
            emitter.enabled = true;
            emitter.emit = true;
        }

        foreach (ParticleRenderer r in renderers)
            r.enabled = true;
            */
    }

    public override void OnPoolDestroy()
    {
        /*
        base.OnPoolDestroy();
        foreach (ParticleEmitter emitter in emitters)
            emitter.enabled = false;
        
        foreach (ParticleRenderer r in renderers)
            r.enabled = false;

        */
    }


    public void StartEffect(Vector3 start, Vector3 end, float velPerSec)
    {
        var m_path = new Vector3[2];
        m_path[0] = start;
        m_path[1] = end;

        float dist = Vector3.Distance(start, end);
        float flyTime = dist / velPerSec;

        Hashtable args = new Hashtable();
        args.Add("name", "MovingEffect"+ Time.time);
        args.Add("time", flyTime);
        args.Add("path", m_path);
        args.Add("orienttopath", true);
        args.Add("easetype", iTween.EaseType.linear);//easeInOutQuad);
        args.Add("oncomplete", "OnFlyComplete");
        args.Add("oncompleteparams",0);
        iTween.MoveTo(gameObject, args);
    }

    public void StopEffect()
    {
        StartCoroutine(WaitForCompletion());
    }

    void OnFlyComplete(int param)
    {
        /*
        var emit = GetComponent<ParticleEmitter>();
        if (emit != null)
            emit.emit = false;

        GameObjectPoolManager.PoolDestroy(gameObject, 1.0f);
         */

        StartCoroutine(WaitForCompletion());
    }


    IEnumerator WaitForCompletion()
    {
        //var emit = GetComponent<ParticleEmitter>();
        //if (emit != null)
            //emit.emit = false;

        yield return new WaitForSeconds(2.0f);

        GameObjectPoolManager.PoolDestroy(gameObject);
    }

}
