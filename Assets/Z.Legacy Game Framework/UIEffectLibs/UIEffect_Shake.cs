using UnityEngine;
using System.Collections;

public class UIEffect_Shake : MonoBehaviour 
{
	public bool bPlay;
	public float PlaySpeed = 1;
	public float PlayMinSize = 1.0f;
	public float PlayMaxSize = 1.2f;
	private float MoveTime = 0;

	float i = -90;
	float signheight = 1.0f; //( -1~ 1 )
	Vector3 scale;
	
	void Start()
	{
		scale = transform.localScale;
	}
	
	public void SetPlay(bool flag)
	{
		bPlay = flag;	
	}
	
	void Update()
	{
		if(!bPlay) return;

		Shake();
	}

	void Shake1()
	{
		float scale = Mathf.PingPong(MoveTime * PlaySpeed, 1) * (PlayMaxSize - PlayMinSize) + PlayMinSize;
		
		transform.localScale = new Vector3(scale, scale, scale);
		
		MoveTime += Time.deltaTime;
	}

	void Shake()
	{
		i += Time.deltaTime*(300.0f*PlaySpeed);
		
		if( i > 360.0f )
			i = 0;
		
		
		float j = ((Mathf.Sin(i*Mathf.Deg2Rad)+1.0f)/2.0f)*(PlayMaxSize-PlayMinSize)+PlayMinSize;

		if( j > PlayMaxSize ) j = PlayMaxSize;
		if( j < PlayMinSize ) j = PlayMinSize;

		//Debug.LogError(" i : "+ i +", "+"j : "+j);
		transform.localScale = new Vector3(j,j,scale.z);
	}

	public Vector3 GetScale()
	{
		return 	transform.localScale;
	}
}