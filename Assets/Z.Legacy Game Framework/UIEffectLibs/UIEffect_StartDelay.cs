using UnityEngine;
using System.Collections;

public class UIEffect_StartDelay : MonoBehaviour {

	public float fDelayTime;
	public bool bScale = true;
	public Vector3 vecScale = Vector3.zero;
	public bool bAlpha;
	public float fAlphaValue;

	float fStartTime;

	void Awake()
	{
		fStartTime = Time.time;

		if( bScale )
			transform.localScale = vecScale;
	}

	// Use this for initialization
	void Start () {
		fStartTime = Time.time;
		transform.localScale = vecScale;
	}
	
	// Update is called once per frame
	void Update () {
		if( Time.time < (fStartTime+fDelayTime) )
		{
			if( bAlpha )
			{
				if( gameObject.GetComponent<UIPanel>() )
				{
					//gameObject.GetComponent<UIPanel>().SetAlphaRecursive(fAlphaValue, false);
					Debug.LogError("StartDelay Scale");
				}
			}

			if( bScale )
			{
				transform.localScale = vecScale;
			}
		}
	}
}
