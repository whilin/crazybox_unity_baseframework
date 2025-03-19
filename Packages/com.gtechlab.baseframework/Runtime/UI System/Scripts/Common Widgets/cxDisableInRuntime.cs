using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-1000)]
public class cxDisableInRuntime : MonoBehaviour 
{
    void Awake()
    {
        gameObject.SetActive(false);   
    }
}
