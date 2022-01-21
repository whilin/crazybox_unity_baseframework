using UnityEngine;
using System.Collections;
//using Noriatti; 

public class UIEffect_Scale : MonoBehaviour {

	public iTween.EaseType EaseType;
	public float DelayTime, PlayTime;
	public bool Direction = true; // 0: From, 1: To
	public Vector3 sor, tar;
	public bool StartActive = true;
	
	public bool bEnd = false;

	// delegate funtion
	public GameObject eventReceiver;
	public string callWhenFinished;
	public delegate void OnFinished (UIEffect_Scale tween);
	public OnFinished onFinished;

	// Use this for initialization
	void Start () {
		if( StartActive )
			StartScale(Direction);
	}

	void Awake()
	{
		transform.localScale = sor;
	}

	void OnActivated()
	{
		if( StartActive )
			StartScale(Direction);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetCurScaleBySor()
	{
		transform.localScale = sor;
	}

	public void Reset()
	{
		transform.localScale = sor;
	}

	public void StartScale(bool dir)
	{
		Direction = dir;

		if( Direction )
			iTween.ScaleTo(gameObject,iTween.Hash("scale",tar,"delay", DelayTime, "time",PlayTime,"easetype",EaseType,"oncomplete","Complete_Scale"));
		else
			iTween.ScaleTo(gameObject,iTween.Hash("scale",sor,"delay", DelayTime, "time",PlayTime,"easetype",EaseType,"oncomplete","Complete_Scale"));
	}
	
	void Complete_Scale()
	{
		SetDelegateFuntion();
	}

	// delegate funtion
	void SetDelegateFuntion()
	{
		if (onFinished != null) onFinished(this);

		if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
		{
			eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
		}
	}
}
