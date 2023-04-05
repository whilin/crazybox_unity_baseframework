#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;

public abstract class cxResourceCreator {

    [MenuItem ("Assets/G-Tech Lab/Creator Scene Resource", true, 0)]
    public static bool CreatorSceneResourceVaild () {
        return IsAssetAFolder (Selection.activeObject);
    }

    [MenuItem ("Assets/G-Tech Lab/Creator Scene Resource", false, 0)]
    public static void CreatorSceneResource () {
        CreatorResource (true);
    }

    [MenuItem ("Assets/G-Tech Lab/Creator Asset Resource", true, 0)]
    public static bool CreatorAssetResourceVaild () {
        return IsAssetAFolder (Selection.activeObject);
    }

    [MenuItem ("Assets/G-Tech Lab/Creator Asset Resource", false, 0)]
    public static void CreatorAssetResource () {
        CreatorResource (false);
    }

    public static void CreatorResource (bool scene) {
        try {
            var path = AssetDatabase.GetAssetPath (Selection.activeObject.GetInstanceID ());

            cxResourceCreateWindow.Show (path, scene, (input) => {
                if (input != null)
                    CreateSceneResource (path, input);
            });
        } catch (Exception ex) {
            Debug.LogException (ex);
            cxEditorWindowUtils.ShowErrorMessage ("Build Resource Bundle", ex.Message);
        }
    }

    private static bool IsAssetAFolder (UnityEngine.Object obj) {
        string path = "";

        if (obj == null)
            return false;

        path = AssetDatabase.GetAssetPath (obj.GetInstanceID ());
        if (path.Length > 0)
            return Directory.Exists (path);

        return false;
    }

    static void CreateSceneResource (string basePath, cxResourceCreateWindow.ResourceInputModel input) {
        var env = cxCreatorEnvironment.GetEnvironment ();

        string resourcePath = basePath + "/" + input.resourceId;

        if (!Directory.Exists (resourcePath)) {
            Directory.CreateDirectory (resourcePath);
        }

        CreateResourceDescScript (resourcePath, input);

        if (env.sceneTemplate && input.resourceType == cxResourceType.Scene) {
            string sceneFileName = resourcePath + "/" + input.resourceId + ".unity";
            var result = SceneTemplateService.Instantiate (env.sceneTemplate, false, sceneFileName);
        }
    }

    static string CreateResourceDescScript (string resourcePath, cxResourceCreateWindow.ResourceInputModel input) {
        var desc = ScriptableObject.CreateInstance<cxResourceDefinition> ();
        desc.resourceId = input.resourceId;
        desc.resourceType = input.resourceType;
        desc.version = input.version;

        string path = resourcePath + "/" + input.resourceId + ".asset";
        AssetDatabase.CreateAsset (desc, path);
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();

        return path;
    }
}

#endif