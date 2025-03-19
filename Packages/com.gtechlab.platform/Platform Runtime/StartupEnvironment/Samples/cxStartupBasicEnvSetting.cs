using System;
using UnityEngine;


[CreateAssetMenu(fileName = "StartupBasicEnvSetting_envId", menuName = "G-Tech Lab/Create Basic StartUp Setting", order = 0)]
public class cxStartupBasicEnvSetting : cxStartupEnvSetting {
    
    [Header("Basic App Backend Server")]
    public string backendServerURL;

    [Header("Resource & Bundle Server")]
    public cxAssetBundleManager.LoadOption bundleLoadOption;
    public string resourceServerURL;
    public string resourceLocationURL;

    [Header("Log Option")]
    public bool verboseLog;
}