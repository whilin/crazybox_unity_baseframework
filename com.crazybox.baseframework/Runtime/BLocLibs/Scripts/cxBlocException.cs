using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum NetResult
{
    Ok = 1,
    Unknown = 0,
    UserNotFound = -10,
    AuthFailed = -11,
}


public class cxBlocException : Exception
{
    int m_code;

    public int Code { get => m_code; }

    // string m_message;

    public cxBlocException(int code, string message) : base(message)
    {
        m_code = code;
    }

    public override string ToString() => base.Message;
}