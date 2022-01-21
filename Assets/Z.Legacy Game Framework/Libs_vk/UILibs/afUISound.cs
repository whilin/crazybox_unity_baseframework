using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class afUISound : MonoBehaviour
{
    public AudioClip Sound;
	public bool playOnEnable=false;

	void OnEnable()
	{
		if(playOnEnable)
			PlaySound();
	}

    public void PlaySound()
    {
        var pos = Camera.main.transform.position + Camera.main.transform.forward;

        AudioSource.PlayClipAtPoint(Sound, pos);
    }

}
