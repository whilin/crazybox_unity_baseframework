using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class cxNetPacket { }

public enum NetRequestState
{
    Pending,
    Completed,
    Error
}