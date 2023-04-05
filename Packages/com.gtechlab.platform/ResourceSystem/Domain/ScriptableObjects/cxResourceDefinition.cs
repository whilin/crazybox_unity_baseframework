using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "put resource id", menuName = "G-Tech Lab/Create Resource Definition", order = 0)]
public class cxResourceDefinition : ScriptableObject {
    public string resourceId;
    public cxResourceType resourceType;
    public int version = 1;

    public cxResourceDescModel ToModel(){
        return new cxResourceDescModel(){
            resourceType = resourceType,
            resourceId = resourceId
        };
    }
}
