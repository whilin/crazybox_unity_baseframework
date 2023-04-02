using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class cxSoundEffect : PooledObject // MonoBehaviour
{
    private AudioSource soundSource;
    public AudioClip[] soundClips;

    private float effectLength = 1f;
    private bool m_poolAfterComplete = true;

    override protected void Awake()
	{
        base.Awake();

		soundSource = GetComponent<AudioSource> ();
	}
	
	override public void OnPoolCreate()
	{
        base.OnPoolCreate();

		m_poolAfterComplete = true;
		StartEffect();
	}
	/*
    public virtual void ResetEffect ()
    {
    	if(m_poolAfterComplete)
			GameObjectPoolManager.PoolDestroy(gameObject);
		else 
		    Destroy(gameObject);
    }
    */

    public virtual void StartEffect ()
    {
		AudioClip clip = soundClips[Random.Range(0,soundClips.Length)];
		if(clip !=null)
		{
			effectLength = clip.length;

            //if (DuelSystemConfig.userConfig == null || DuelSystemConfig.userConfig.effect)
        	    soundSource.PlayOneShot(clip);
		}

        if (m_poolAfterComplete)
            GameObjectPoolManager.PoolDestroy(gameObject, effectLength);
        else
            Destroy(gameObject, effectLength);

        //StartCoroutine(WaitForCompletion());
    }

    /*
    public IEnumerator WaitForCompletion ()
    {
        //Wait for the effect to complete itself
        yield return new WaitForSeconds(effectLength);

        //Reset the now completed effect
        ResetEffect();
    }
     */
}
