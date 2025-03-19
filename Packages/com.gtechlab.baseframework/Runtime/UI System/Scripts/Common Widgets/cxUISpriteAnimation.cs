using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUISpriteAnimation : MonoBehaviour {

	public Sprite [] sprites;
	public float framePerSec=10;

	private int curFrame=0;
	private Image image;

	void Awake()
	{
		image =GetComponent<Image>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	Coroutine m_co;

	void OnEnable()
	{
		m_co = StartCoroutine(CoPlay());
	}

	void OnDisable()
	{
		if(m_co!=null)
			StopCoroutine(m_co);	
		m_co =null;
	}

	public void  Play() {
		m_co =StartCoroutine(CoPlay());
	}
	public void Stop() {

		if(m_co!=null)
			StopCoroutine(m_co);	
	}

	IEnumerator CoPlay()
	{
		curFrame=0;
		while(true)
		{

			if(curFrame >= sprites.Length)
				curFrame =0;

			image.sprite = sprites[curFrame];
			curFrame++;

			yield return new WaitForSeconds(1.0f/framePerSec);
		}
	}
}
