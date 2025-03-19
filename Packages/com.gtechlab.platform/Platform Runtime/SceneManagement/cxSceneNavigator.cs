using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cxSceneNavigator : MonoSingleton<cxSceneNavigator> {
    public enum cxLoadStatusType {
        Undef,
        Ready,
        Pending,
        Loading,
        Loaded,
        Cancelled,
        Error
    }

    public struct LoadStatus {
        public cxLoadStatusType status;
        public object sceneInfo;
        public float progress;
        public string error;
    }

    public delegate Task<bool> AskToDownloadDelegate (int bytes, object sceneInfo);

    private Scene baseScene;
    private Scene? activeScene;
    private string activeSceneURL;
    private object sceneInfo;

    private BehaviorSubject<LoadStatus> loadStatus;

    public IObservable<LoadStatus> LoadStatusAsObservable => loadStatus.AsObservable ();
    public LoadStatus LoadStatusInfo => loadStatus.Value;
    public Scene? ActiveScene => activeScene;
    public object SceneInfo => sceneInfo;


    protected override void Awake () {
        base.Awake();
        loadStatus = new BehaviorSubject<LoadStatus> (new LoadStatus () {
            status = cxLoadStatusType.Undef,
                progress = 0
        });

        //Note. DontDestroyOnLoad Scene으로 설정됨..-.-???
        //baseScene = gameObject.scene;
        var baseScene = SceneManager.GetActiveScene();
        if(baseScene.IsValid()) {
            cxLog.Log("cxSceneNavigator base scene:"+baseScene.name);
        }
    }

    public void PushScene (string sceneURL, object sceneInfo = null, AskToDownloadDelegate askToDownload = null) {
        LoadScene (sceneURL, sceneInfo, askToDownload, LoadSceneMode.Additive);
    }

    public void ReplaceScene (string sceneURL, object sceneInfo = null, AskToDownloadDelegate askToDownload = null) {
        LoadScene (sceneURL, sceneInfo, askToDownload, LoadSceneMode.Single);
    }

    public void PopScene () {
        UnloadScene ();
    }

    async void UnloadScene () {
        if (activeScene.HasValue) {
            if (activeScene.Value.IsValid () && activeScene.Value.isLoaded) {
                await SceneManager.UnloadSceneAsync (activeScene.Value);
            }

            bool resource = cxResourceNaming.IsResource(activeSceneURL, out string resouceId , out string __);
            if(resource){
                cxResourceBundleLoader.Instance.ReleaseBundle(resouceId);
            }
            
            activeScene = null;  
        }
    }

    async void LoadScene (string sceneURL, object sceneInfo, AskToDownloadDelegate askToDownload, LoadSceneMode loadSceneMode) {

        try {
            loadStatus.OnNext (new LoadStatus () {
                status = cxLoadStatusType.Ready,
                    progress = 0,
                    sceneInfo = sceneInfo
            });

            var (requiredDownload, size) = await cxUniversalResourceLoader.Instance.IsRequireDownload (sceneURL);
            if (requiredDownload) {
                if (askToDownload != null) {
                    var ok = await askToDownload (size, sceneInfo);
                    if (!ok) {
                        loadStatus.OnNext (new LoadStatus () {
                            status = cxLoadStatusType.Cancelled,
                                progress = 0,
                                sceneInfo = sceneInfo
                        });
                        return;
                    }
                }
            }

            if (activeScene.HasValue) {
                if (activeScene.Value.IsValid () && activeScene.Value.isLoaded) {
                    await SceneManager.UnloadSceneAsync (activeScene.Value);
                }

                activeScene = null;
                this.sceneInfo = null;
            }

            this.sceneInfo = sceneInfo;

            var scene = await cxUniversalResourceLoader.Instance.LoadScene (sceneURL,
                loadSceneMode,
                (float p) => {
                    loadStatus.OnNext (new LoadStatus () {
                        status = cxLoadStatusType.Loading,
                            progress = p,
                            sceneInfo = sceneInfo
                    });
                });

            if (!scene.IsValid ())
                throw new Exception ("Scene Load Failed, invalid scene");

            activeSceneURL = sceneURL;


            SetActiveScene (scene);

            loadStatus.OnNext (new LoadStatus () {
                status = cxLoadStatusType.Loaded,
                    progress = 1,
                    sceneInfo = sceneInfo
            });

        } catch (Exception ex) {
            Debug.LogException (ex);

            loadStatus.OnNext (new LoadStatus () {
                status = cxLoadStatusType.Error,
                    progress = 1,
                    sceneInfo = sceneInfo,
                    error = ex.Message
            });
        }
    }

    void SetActiveScene (Scene scene) {

        activeScene = scene;
        SceneManager.SetActiveScene (scene);

#if UNITY_EDITOR
        var objs = scene.GetRootGameObjects ();
        foreach (var obj in objs) {
            cxAssetBundleUtil.FixShadersForEditor (obj);
        }
        cxAssetBundleUtil.ReplaceShaderForEditor (RenderSettings.skybox);
#endif
        //LightProbes.TetrahedralizeAsync();
    }
}