using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetBundleSystem {
    public const string BaseBundlePath = "AssetBundles";
    public const string TableFileName = "bundle_table.json";
}

public class AssetBundleDesc {
    public string bundle;
    public string hash;
    public List<string> scenes;
    public List<string> assets;
}

[Serializable]
public class AssetDesc {
    public string name;
    public string bundle;
}

[CreateAssetMenu (fileName = "AssetBundle Table", menuName = "ModooBike/Create AssetBundle Table", order = 1)]
public class AssetBundleTable : ScriptableObject {
    public List<AssetDesc> list;

    public AssetDesc Find (string name) {
        return list.Find (q => q.name == name);
    }
}