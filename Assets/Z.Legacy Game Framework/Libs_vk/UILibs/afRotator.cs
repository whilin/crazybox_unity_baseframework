using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class afRotator : MonoBehaviour {

	public enum Axis
	{
		Y,
		Z,
		X,
	}

	public Axis axis = Axis.Y;
	public float speed;

	private float rotataion;

	// Update is called once per frame
	void FixedUpdate () 
	{
		rotataion += speed;

		Quaternion rotEular = Quaternion.identity;
		if(axis == Axis.Y)
			rotEular = Quaternion.Euler(0f,rotataion,0f);
		else if(axis == Axis.X)
			rotEular = Quaternion.Euler(rotataion,0f, 0f);
		else
			rotEular = Quaternion.Euler(0f, 0f,rotataion);
		
		gameObject.transform.localRotation = rotEular;
	}
}
