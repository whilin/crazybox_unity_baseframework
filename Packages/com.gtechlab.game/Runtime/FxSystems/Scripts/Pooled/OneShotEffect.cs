using UnityEngine;
using System.Collections;

public class OneShotEffect : PooledObject { //MonoBehaviour {

    /*
	private ParticleEmitter [] emitters;
    private ParticleRenderer[] renderers;
    private ParticleAnimator[] animators;
    */

    private ParticleSystem [] particleSystems;

	public bool m_oneshot=true;
	public float m_overtime = 1f;

    public ParticleSystem[] prefabParticleSystems;
    public GameObject m_prefabFxSound;

    public bool test = false;

	override protected void Awake()
	{
        base.Awake();
        /*
		emitters = GetComponentsInChildren<ParticleEmitter>();
        renderers = GetComponentsInChildren<ParticleRenderer>();
        animators = GetComponentsInChildren<ParticleAnimator>();
        */

        //particleSystem = GetComponent<ParticleSystem>();

        int i = 0;
        
        particleSystems = new ParticleSystem[prefabParticleSystems.Length];
        foreach (var p in prefabParticleSystems)
        {
            GameObject obj = GameObject.Instantiate(p.gameObject) as GameObject;
            particleSystems[i++] = obj.GetComponent<ParticleSystem>();
            obj.transform.SetParent(transform);
        }

        if (test)
            StartEffect();
	}
	
	override public void OnPoolCreate()
	{
        base.OnPoolCreate();

        //foreach (ParticleEmitter emitter in emitters)
        //    emitter.enabled = true;
        //foreach (ParticleRenderer r in renderers)
            //r.enabled = true;

		StartEffect();
	}

    public override void OnPoolDestroy()
    {
        if (particleSystems != null && particleSystems.Length > 0)
            foreach (var p in particleSystems)
            {
                p.Stop(true);
                p.Clear(true);
            }
        
        StopAllCoroutines();

        base.OnPoolDestroy();

        //foreach (ParticleEmitter emitter in emitters)
        //    emitter.enabled = false;
        //foreach (ParticleRenderer r in renderers)
            //r.enabled = false;
    }

	/*
	 * http://answers.unity3d.com/questions/46827/particle-emitter-to-emit-one-shot-on-specific-occa.html
	 * 
	 * "One-shot" doesn't mean the particles are emitted only one time, 
	 * it means the particles are emitted all at once instead of over time. 
	 * When all the particles run out of energy, they are emitted again, 
	 * and will continue as long as the particle system is active and 
	 * emit is set to true. 
	 * What you're probably looking for is particleEmitter.Emit() 
	 * (particleEmitter.emit should be false).
	 * 
	 * */
	
	void StartEffect()
	{

        if (particleSystems != null && particleSystems.Length > 0)
        {
            foreach (var p in particleSystems)
            {
                p.transform.position = transform.position;
                p.transform.rotation = transform.rotation;
                p.Play(true);
            }

            StartCoroutine(WaitForCompletion2());
        }
        else
        {
            if (m_oneshot)
            {
                //foreach (ParticleEmitter emitter in emitters)
                //{
                //    emitter.emit = false; //overtime 으로 분출되는 것을 정지 시킨다.
                //    emitter.Emit();
                
                //}

                StartCoroutine(WaitForOneshotCompletion());
            }
            else
            {
                //foreach (ParticleEmitter emitter in emitters)
                    //emitter.emit = true;

                StartCoroutine(WaitForOvertimeCompletion());
            }
        }

        if (m_prefabFxSound != null)
            GameObjectPoolManager.Spawn(m_prefabFxSound, transform.position, transform.rotation);
	}
	
	void ResetEffect()
	{
		//foreach(ParticleEmitter emitter in emitters)
			//emitter.ClearParticles();

        GameObjectPoolManager.PoolDestroy(gameObject);
	}
	
	public IEnumerator WaitForOvertimeCompletion ()
    {
        //Wait for the effect to complete itself
        yield return new WaitForSeconds(m_overtime);

        //Reset the now completed effect
        ResetEffect();
    }


    public IEnumerator WaitForOneshotCompletion()
    {
        bool live = false;
        do
        {
            //yield return new WaitForSeconds(1.0f);
            yield return null;

            live = false;

            foreach (var emitter in particleSystems)
                if (emitter.particleCount > 0)
                    live = true;
        } while (live);

        ResetEffect();
    }

    public IEnumerator WaitForCompletion2()
    {
        bool alive = false;

        do
        {
            alive = false;

            yield return new WaitForSeconds(1.0f);
            foreach (var p in particleSystems)
            {
                alive = p.IsAlive(true);
                if(alive)
                    break;
            }

        } while (alive);

        GameObjectPoolManager.PoolDestroy(gameObject);
    }


    //void Update()
    //{	
    //    if(m_oneshot)
    //    {
    //        bool live = false;
    //        foreach(ParticleEmitter emitter in emitters)
    //        {
    //            if(emitter.particleCount > 0)
    //            {
    //                live = true;
    //                break;
    //            }
    //        }
			
    //        if(!live)
    //            ResetEffect();
    //    }
    //}
}
