using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cxUIButtonSound : MonoBehaviour, IPointerClickHandler
{
    public AudioClip ClickSound;

    void Start()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
    }

    public void PlayClickSound()
    {
        if(ClickSound !=null)
        {
            var pos = Camera.main.transform.position + Camera.main.transform.forward;
            AudioSource.PlayClipAtPoint(ClickSound, pos);
        }
    }

}
