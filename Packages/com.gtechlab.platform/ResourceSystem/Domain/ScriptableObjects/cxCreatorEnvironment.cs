using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (fileName = "CreatorEnvironment", menuName = "G-Tech Lab/Create Creator Environment", order = 0)]
public class cxCreatorEnvironment : ScriptableObject {

    public const string ResourceJsonFileName = "resource.json";

    [Tooltip("Project Base Path 기준")]
    public string BuildOutputPath = "./BundleBuild/";
    public string ResourceInfoServerUrl = "";
    public string ResourceUploadServerUrl = "";
    public string apiToken = "";
    
    public bool buildWebGL=false;
    public bool buildAndroid=false;
    public bool buildIOS=false;

    public static cxCreatorEnvironment GetEnvironment () {
        var envs = Resources.LoadAll<cxCreatorEnvironment> ("");
        if (envs.Length == 0) {
            throw new Exception ("CreatorEnvironment not found in Resources folder");
        } else {
            return envs[0];
        }
    }
}