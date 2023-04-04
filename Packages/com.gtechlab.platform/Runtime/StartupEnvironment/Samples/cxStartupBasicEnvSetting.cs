using System;
using UnityEngine;


[CreateAssetMenu(fileName = "StartupBasicEnvSetting_envId", menuName = "G-Tech Lab/Create Basic StartUp Setting", order = 0)]
public class cxStartupBasicEnvSetting : cxStartupEnvSetting {
    
    public string backendServerURL;
    public cxAssetBundleManager.LoadOption bundleLoadOption;
    public string bundleLoadURL;
    public bool verboseLog;
}