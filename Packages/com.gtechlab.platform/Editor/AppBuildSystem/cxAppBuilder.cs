using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu (fileName = "AppBuilder", menuName = "G-Tech Lab/Create App Builder")]
public class cxAppBuilder : ScriptableObject {

    public BuildTarget buildTarget;
    public cxStartUpEnvId startUpEnvId;
    public string mainScenePath;
    public List<string> contentScenePaths;

    [Header("Deploy Path based on Project Path")]
    public string deployBasePath = "Deploy";


    [MenuItem("Assets/G-Tech Lab/Build App", true)]
    public static bool ValidateBuild() {
        return Selection.activeObject is cxAppBuilder;
    }

    [MenuItem("Assets/G-Tech Lab/Build App")]
    public static void BuildApp() {
        var config = Selection.activeObject as cxAppBuilder;
        if (config == null) {
            UnityEngine.Debug.LogError("cxBuilderConfiguration 에셋을 선택해주세요.");
            return;
        }

        VersionUp();
        BuildPlayer(config);
    }

    [MenuItem ("Assets/G-Tech Lab/Open Build Folder")]
    public static void OpenBuildFolder () {
        var config = Selection.activeObject as cxAppBuilder;
        if (config == null) {
            UnityEngine.Debug.LogError("cxBuilderConfiguration 에셋을 선택해주세요.");
            return;
        }

        var deployPath = Path.Combine (Application.dataPath, "..", config.deployBasePath);
        OpenTerminalAtPath (deployPath);
    }
    
    private static void OpenTerminalAtPath (string folderPath) {
        // macOS에서 open 명령을 사용하여 터미널 열기
        ProcessStartInfo processInfo = new ProcessStartInfo ("open", $"-a Terminal {folderPath}");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;

        Process process = new Process ();
        process.StartInfo = processInfo;
        process.Start ();

        // 표준 출력과 오류를 읽어옵니다.
        string output = process.StandardOutput.ReadToEnd ();
        string error = process.StandardError.ReadToEnd ();

        process.WaitForExit ();
        process.Close ();

        // Unity Console에 출력 결과를 표시합니다.
        UnityEngine.Debug.Log ("Output: " + output);
        if (!string.IsNullOrEmpty (error)) {
            UnityEngine.Debug.LogError ("Error: " + error);
        }
    }
    static void VersionUp () {
        var version = PlayerSettings.bundleVersion;
        var nums = version.Split ('.');
        if (nums.Length != 3)
            throw new System.Exception ("illegal version format:" + version);

        int v1 = int.Parse (nums[0]);
        int v2 = int.Parse (nums[1]);
        int v3 = int.Parse (nums[2]);

        PlayerSettings.bundleVersion = string.Format ("{0}.{1}.{2}", v1, v2, v3 + 1);
    }

     static void BuildPlayer (cxAppBuilder configuration) {

        if(string.IsNullOrEmpty(configuration.mainScenePath)) {
            UnityEngine.Debug.LogError("메인 씬 경로가 설정되지 않았습니다.");
            return;
        }

        if(string.IsNullOrEmpty(configuration.deployBasePath)) {
            UnityEngine.Debug.LogError("배포 경로가 설정되지 않았습니다.");
            return;
        }

        BuildTargetGroup buildTargetGroup = BuildTargetGroup.WebGL;
        if(configuration.buildTarget == BuildTarget.WebGL) {
            buildTargetGroup = BuildTargetGroup.WebGL;
        } else if(configuration.buildTarget == BuildTarget.Android) {
            buildTargetGroup = BuildTargetGroup.Android;
        } else if(configuration.buildTarget == BuildTarget.iOS) {
            buildTargetGroup = BuildTargetGroup.iOS;
        } else if(configuration.buildTarget == BuildTarget.StandaloneWindows64) {
            buildTargetGroup = BuildTargetGroup.Standalone;
        } else if(configuration.buildTarget == BuildTarget.StandaloneLinux64) {
            buildTargetGroup = BuildTargetGroup.Standalone;
        } else if(configuration.buildTarget == BuildTarget.StandaloneOSX) {
            buildTargetGroup = BuildTargetGroup.Standalone;
        } else {
            UnityEngine.Debug.LogError("지원하지 않는 빌드 타겟입니다.");
            return;
        }

        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions ();

        List<string> scenes = new List<string> { configuration.mainScenePath };
        scenes.AddRange (configuration.contentScenePaths);
        buildPlayerOptions.scenes = scenes.ToArray ();

        cxStartupEnvTable.SetActiveSetting (configuration.startUpEnvId);

        EditorUserBuildSettings.SwitchActiveBuildTarget (buildTargetGroup, configuration.buildTarget);

        buildPlayerOptions.locationPathName = GetBuildPath (configuration, configuration.buildTarget);
        buildPlayerOptions.target = configuration.buildTarget;
        buildPlayerOptions.options = BuildOptions.None;

        if (!Directory.Exists (buildPlayerOptions.locationPathName)) {
            Directory.CreateDirectory (buildPlayerOptions.locationPathName);
        }

        BuildPipeline.BuildPlayer (buildPlayerOptions);
    }

    public static string GetBuildPath (cxAppBuilder configuration, BuildTarget buildTarget) {
        string basePath = Path.Combine (Application.dataPath, "..", configuration.deployBasePath, buildTarget.ToString ());
        return basePath;
    }
}