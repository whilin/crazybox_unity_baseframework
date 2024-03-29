﻿using UnityEngine;
using System.Collections;

public class cxSelfDestroy : MonoBehaviour
{
    [SerializeField]
    float delay = 0f;

	void Start ()
    {
        StartCoroutine(Destroy());
	}

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
