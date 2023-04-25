using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAvatarConfig", menuName = "G-Tech Lab/Game/Create Default Avatar Config", order = 1)]
public class cxDefaultAvatarConfig : ScriptableObject
{
    public GameObject playerControllerSetup;
    public GameObject playerViewerSetup;

    //[AssetPath.Attribute(typeof(Object))]
    public string avatarMeshBasePath;
    public string avatarIconBasePath;
    //[AssetPath.Attribute(typeof(Object))]
    public string avatarDataTablePath;

    public TAvatarEquipSetModel defaultEquipSet;

    public static cxDefaultAvatarConfig GetAvatarConfig(){
        var configs = Resources.LoadAll<cxDefaultAvatarConfig>("");
        if(configs.Length == 0) {
            cxLog.Warning("cxDefaultAvatarConfig not found in at Resource /Path");
            return null;
        } else {
            return configs[0];
        }
    }
}
