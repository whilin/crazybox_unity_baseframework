using UnityEngine;
using System.Collections;

public class cxDisableInRuntime : MonoBehaviour 
{
    void Awake()
    {
        gameObject.SetActive(false);   
    }
}
