using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReservedGameMsg
{
    public static int OnBack    = 10001;
    public static int OnClose   = 10002;
}

public static class afGameEventSystem {//}: MonoSingleton<afGameEventSystem> {

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
