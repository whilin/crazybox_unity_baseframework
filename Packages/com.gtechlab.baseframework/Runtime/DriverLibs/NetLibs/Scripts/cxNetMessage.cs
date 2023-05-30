using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class cxNetMessage {

 }

 [Serializable]
 public class cxNetResponse : cxNetMessage {
    public int resultCode;
 }

public enum NetRequestState
{
    Pending,
    Completed,
    Error
}