using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRunTimeDemo : MonoBehaviour
{
    void Start()
    {
        SetupSystem();
        LoadResource1();
        LoadResource2();
    }

    void SetupSystem(){
        var env = cxStartupEnvTable.LoadActiveSetting<cxStartupBasicEnvSetting>();
        var repo = new cxResourceInfoApiRepository(new cxNetDriver(env.resourceServerURL));
        cxResourceBundleLoader.Create(repo, env.resourceLocationURL);
    }

    async void LoadResource1(){
        var gameObj = await cxUniversalResourceLoader.Instance.LoadAsset<GameObject>("resource://com.gtechlab.sample.01/Cylinder");
        await new WaitForUpdate();
        if(gameObj) {
            GameObject.Instantiate(gameObj);
        }
    }

  async void LoadResource2(){
           var scene = await cxUniversalResourceLoader.Instance.LoadScene("resource://com.gtechlab.sample.03/New Scene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
       
    }
}
