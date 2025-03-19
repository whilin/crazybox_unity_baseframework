using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cxUISound : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
    public enum PlayType {
        PlayOnClick,
        PlayOnPressed,
        PlayOnEnable
    }

    public PlayType playType = PlayType.PlayOnClick;
    public AudioClip clip;

    void OnEnable () {
        if (playType == PlayType.PlayOnEnable)
            Play ();
    }

    public void Play () {
        if (clip) {
            if( Camera.main == null) {
                int k=0;
            } else {
                var pos = Camera.main.transform.position + Camera.main.transform.forward;
                AudioSource.PlayClipAtPoint (clip, pos);
            }
        }
    }

    void IPointerClickHandler.OnPointerClick (PointerEventData eventData) {
        if (playType == PlayType.PlayOnClick)
            Play ();
    }

    void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
        if (playType == PlayType.PlayOnPressed) {
            var source =  GetComponent<AudioSource> ();
            if(source) {
                source.clip = clip;
                source.loop = true;
                source.Play();
            } else {
                Debug.LogWarning($"cxUISound  audio source not found : {gameObject.name}");
            }
        }
    }

    void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
        if (playType == PlayType.PlayOnPressed) {
            var source =  GetComponent<AudioSource> ();
            if(source) {
              source.Stop();
            }
        }
    }
}