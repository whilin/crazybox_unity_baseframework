using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum cxStartUpEnvId {
    Undef,
    DEV,
    BETA,
    RELEASE,
}


[CreateAssetMenu (fileName = "StartupEnvTable", menuName = "G-Tech Lab/Create Startup env table", order = 0)]
public class cxStartupEnvTable : ScriptableObject {
    public cxStartUpEnvId startUpEnv;

    private cxStartupEnvSetting GetActiveSetting () {
        var list = Resources.LoadAll<cxStartupEnvSetting> ("AppEnvironment");
        var list2 = new List<cxStartupEnvSetting> (list);

        var setting = list2.Find (q => q.settingId == startUpEnv);
        if(!setting )
            throw new Exception("mcApplication Setting not found:"+startUpEnv);

        return setting;
    }

    public static cxStartupEnvSetting LoadActiveSetting(){
        var table = Resources.Load<cxStartupEnvTable> ("AppEnvironment/StartupEnvTable");

          if(!table )
            throw new Exception("mcApplicationSetupTable not found");
        
        return table.GetActiveSetting();
    }


    public static T LoadActiveSetting<T>() where T : cxStartupEnvSetting {
        var table = Resources.Load<cxStartupEnvTable> ("AppEnvironment/StartupEnvTable");

          if(!table )
            throw new Exception("mcApplicationSetupTable not found");
        
        return table.GetActiveSetting() as T;
    }


#if UNITY_EDITOR
    public static void SetActiveSetting(cxStartUpEnvId settingId){
        var table = Resources.Load<cxStartupEnvTable> ("AppEnvironment/StartupEnvironment");
        
        var list = Resources.LoadAll<cxStartupEnvSetting> ("StartupEnvTable");
        var list2 = new List<cxStartupEnvSetting> (list);

        if(list2.Exists(q => q.settingId == settingId)){
            table.startUpEnv = settingId;
        } else {
            throw new Exception("cxAppStartupEnvironment Setting not found:"+settingId);
        }

        EditorUtility.SetDirty(table);
        AssetDatabase.SaveAssets();
    }
#endif

}