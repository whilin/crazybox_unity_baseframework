using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class cxReservedGameMsg
{
    public static int OnBack    = 10001;
    public static int OnClose   = 10002;
}

[Obsolete()]
public static class cxGameEventSystem {//}: MonoSingleton<afGameEventSystem> {

    public delegate void HandleGameStateMessage(int msg, object msgParam);

    public static event HandleGameStateMessage GameStateMessageReceiver;

    public static void SendGameStateMessage(int msg, object msgParam = null)
    {
        GameStateMessageReceiver(msg, msgParam);
    }

    public static void SendGameMessage(int msg, object msgParam = null)
    {
        GameStateMessageReceiver(msg, msgParam);
    }

    public static void AddListener(HandleGameStateMessage handler)
    {
        GameStateMessageReceiver += handler;
    }
    
    public static void RemoveListener(HandleGameStateMessage handler)
    {
        GameStateMessageReceiver -= handler;
    }
}
