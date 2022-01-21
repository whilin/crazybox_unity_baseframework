using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour 
{
    static int disableLayer = 0;

    Behaviour[] comps;
    Rigidbody rigid;

    int originalLayer;

    virtual protected void Awake()
    {
        disableLayer = LayerMask.NameToLayer("DisableObject");

        comps = GetComponentsInChildren<Behaviour>();
        
        originalLayer = gameObject.layer;
    }

    virtual public void OnPoolCreate()
    {
        foreach (Behaviour c in comps)
            c.enabled = true;

        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = false;
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = true;

        gameObject.layer = originalLayer;
    }

    virtual public void OnPoolDestroy()
    {
        foreach (Behaviour c in comps)
            c.enabled = false;
        
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = false;

        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = disableLayer;
    }
}
