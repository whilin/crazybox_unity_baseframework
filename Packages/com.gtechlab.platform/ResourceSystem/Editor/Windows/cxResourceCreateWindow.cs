#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class cxResourceCreateWindow : EditorWindow {

    public class ResourceInputModel {
        public string resourceId;
        public int version = 1;
        public string creatorId;
        public cxResourceType resourceType;
        public string path;

        public bool IsValid () {
            return !string.IsNullOrEmpty (path) &&
                !string.IsNullOrEmpty (resourceId) &&
                version > 0;
        }
    }

    private bool ok = false;
    private bool isScene = false;

    private UnityEvent<ResourceInputModel> onCloseCallback = new UnityEvent<ResourceInputModel> ();
    private ResourceInputModel resourceInput = new ResourceInputModel ();

    // [MenuItem ("Vaiv Dev/Test/ResourceCreateWindow Test")]
    // static void Test () {
    //     ResourceCreateWindow.Show (false, (resourceId) => {

    //     });
    // }

    public static void Show ( string path, bool isScene,Action<ResourceInputModel> callback) {

        var env = cxCreatorEnvironment.GetEnvironment();

        cxResourceCreateWindow window = EditorWindow.CreateInstance<cxResourceCreateWindow> ();
        window.isScene = isScene;
        window.resourceInput.resourceId = $"{env.CreatorDomain}.<id>";
        window.resourceInput.creatorId = env.CreatorId;
        window.resourceInput.resourceType = isScene ? cxResourceType.Scene : cxResourceType.Asset;

        int width = 300;
        int height = 400;

        window.position = new Rect (Screen.width / 2 - width >> 1, Screen.height / 2 - height >> 1, width, height);
        window.ShowUtility ();
        window.onCloseCallback.AddListener ((input) => {
            callback (input);
        });
    }

    void OnGUI () {
        GUILayout.Space (20);
        GUIStyle style = new GUIStyle () {
            fontSize = 16,
            alignment = TextAnchor.LowerLeft,
        };
        style.normal.textColor = Color.white;

        EditorGUILayout.LabelField ("input resource id", style);
        resourceInput.resourceId = EditorGUILayout.TextField (resourceInput.resourceId);
        GUILayout.Space (20);

        EditorGUILayout.LabelField ("input resource version", style);
        resourceInput.version = Int32.Parse (EditorGUILayout.TextField (resourceInput.version.ToString ()));
        GUILayout.Space (20);

        EditorGUILayout.LabelField ("input creator id", style);
        resourceInput.creatorId = EditorGUILayout.TextField (resourceInput.creatorId);
        GUILayout.Space (20);

        /*
        EditorGUILayout.LabelField ("resource create path", style);
        EditorGUILayout.LabelField (GetDisplayPathName (resourceInput.path));
        if (GUILayout.Button ("select path")) {
            string path = EditorUtility.OpenFolderPanel ("Select Create Path", "Assets", "");
            if (!path.StartsWith (Application.dataPath)) {
                cxEditorWindowUtils.ShowErrorMessage ("", "경로가 올바르지 않습니다, Assets/ 아래 있는 폴더를 선택해주세요");
            } else {
                resourceInput.path = "Assets/" + path.Substring (Application.dataPath.Length);
            }
        }
*/

        GUILayout.Space (40);

        string label = "Create Resource Template";
        if (GUILayout.Button (label)) {
            ok = true;
            this.Close ();
        }
    }

    public static string GetDisplayPathName (string path) {
        if (string.IsNullOrEmpty (path))
            return "<not set>";
        else
            return path;
    }

    void OnInspectorUpdate () {
        Repaint ();
    }

    private void OnDestroy () {
        onCloseCallback.Invoke (ok ? resourceInput : null);
    }
}

#endif