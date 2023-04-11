using UnityEngine;
using UnityEditor;
using System.IO;
 
public class cxPackageVersionEditor : EditorWindow
{
    static string version="v2.0.0";
 
    [MenuItem("Tools/G-Tech Lab/CX Package/To Git CX Package")]
    static void Init()
    {
        cxPackageVersionEditor window = CreateInstance<cxPackageVersionEditor>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowUtility();
    }
 
    void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Change the version and click Update.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        version = EditorGUILayout.TextField("Version: ", version);
        GUILayout.Space(60);
        EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update")) {
                cxPackageManager.ToGitUPM(version);
                Close();
            }
            if (GUILayout.Button("Cancel")) Close();
        EditorGUILayout.EndHorizontal();
    }
}
 