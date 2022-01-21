using UnityEngine;
using System.Collections;

public class afObjectRandomRotator : MonoBehaviour {

    public float speed;
    private float yRotataion;
	
	// Update is called once per frame
	void FixedUpdate () 
    {
      //  yRotataion += speed;
        gameObject.transform.Rotate(gameObject.transform.up, speed, Space.Self);
	}
}
