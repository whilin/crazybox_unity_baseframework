using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxScriptableSingleton<T> : ScriptableObject where T : cxScriptableSingleton<T> {
    private static T _instance;
    public static T Instance {
        get {
            if(_instance == null) {
                T[] asset = Resources.LoadAll<T>("");
                if(asset == null || asset.Length < 1) {
                    throw new System.Exception($"Could not find any singleton scriptable(${typeof(T)}) object in the resources");
                }else if(asset.Length > 1) {
                    Debug.LogWarning($"Multiple instance of the singleton scriptable(${typeof(T)}) object in the resources");
                }

                _instance = asset[0];
            }

             return _instance;
        }
    }
}
