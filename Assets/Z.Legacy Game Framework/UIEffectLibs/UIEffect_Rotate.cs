using UnityEngine;
using System.Collections;

public class UIEffect_Rotate : MonoBehaviour {
	
	public float fSpeed = 1.0f;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, 0,Time.deltaTime*fSpeed));
	}
}
