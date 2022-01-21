using UnityEngine;
using System.Collections;

public class TimeLimitObj : MonoBehaviour {

	float fTime;
	public float fLimitTime = 3.0f;
	
	// Use this for initialization
	void Start () {
		fTime = Time.time;
	}
	
	public void SetLimitTime(float time)
	{
		fLimitTime = time;
		fTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if( fLimitTime != 0 && (Time.time-fTime) > fLimitTime )
		{
			Destroy(gameObject);
		}
	}
}
