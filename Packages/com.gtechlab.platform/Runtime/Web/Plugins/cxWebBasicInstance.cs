using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

public class cxWebCallbackObject {
    public string eventName;
    public Dictionary<string, object> arguments;

    public T GetArgumentObject<T> () where T : class {
        if (arguments != null)
            return JObject.FromObject (arguments).ToObject<T> ();
        else
            return null;
    }
}

public class cxWebBasicInstance : MonoBehaviour {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void hello ();

    [DllImport ("__Internal")]
    private static extern void setFullScreen (bool on);

    [DllImport ("__Internal")]
    private static extern void closeWindow ();

    [DllImport ("__Internal")]
    private static extern string getCookie (string cname);

    [DllImport ("__Internal")]
    private static extern string setCookie (string cname, string cvalue, int expireDays);

    [DllImport ("__Internal")]
    private static extern void installCallback ();
#endif

    private Subject<cxWebCallbackObject> webEventCall = new Subject<cxWebCallbackObject> ();
    public IObservable<cxWebCallbackObject> webEventCallAsObservable => webEventCall.AsObservable ();

    private void Awake () {
        if (!gameObject.name.Equals ("cxWebBasicInstance")) {
            Debug.LogError ("cxWebBasicInstance must named with cxWebBasicInstance but " + gameObject.name);
        }

        InstallCallback ();
    }

    private void InstallCallback () {
#if UNITY_WEBGL && !UNITY_EDITOR
        installCallback ();
#endif
    }

    public static void Hello () {
#if UNITY_WEBGL && !UNITY_EDITOR
        hello ();
#endif
    }

    public static void SetFullScreen (bool on) {
#if UNITY_WEBGL && !UNITY_EDITOR
        setFullScreen (on);
#endif
    }

    public static void CloseWindow () {
#if UNITY_WEBGL && !UNITY_EDITOR
        closeWindow ();
#endif
    }

    public static string GetCookie (string cname) {

#if UNITY_WEBGL && !UNITY_EDITOR
        return getCookie (cname);
#else
        return string.Empty;
#endif
    }

    public static void SetCookie (string cname, string cvalue, int expireDays) {
#if UNITY_WEBGL && !UNITY_EDITOR
        setCookie (cname, cvalue, expireDays);
#else

#endif
    }

    /* Callback Call
    {
        eventName : "eventName",
        arguments : {

        } //json object
    }
    */

    void OnCallback (string jsonParameter) {

        var webObj = Newtonsoft.Json.JsonConvert.DeserializeObject<cxWebCallbackObject> (jsonParameter);
        if (string.IsNullOrEmpty (webObj.eventName)) {
            Debug.LogWarning ("cxWebBasicInstance Callback Format Error, eventName not found :" + jsonParameter);
            return;
        }

        cxLog.Log("cxWebBasicInstance Callback eventName:"+webObj.eventName);

        webEventCall.OnNext (webObj);
    }
}