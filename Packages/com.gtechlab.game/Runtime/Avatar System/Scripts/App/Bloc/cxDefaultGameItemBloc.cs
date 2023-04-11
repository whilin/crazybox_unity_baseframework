using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class cxDefaultGameItemBloc : cxIGameItemBloc {
    BehaviorSubject<cxTGameDataLoadState> loadState = new BehaviorSubject<cxTGameDataLoadState> (cxTGameDataLoadState.Unloaded);
    public override IObservable<cxTGameDataLoadState> LoadStateAsObservable () => loadState.AsObservable ();

    List<TAvatarItemDescModel> avatarItemList = new List<TAvatarItemDescModel> ();

    public cxDefaultGameItemBloc () {
        LoadDataTable ();
    }

    async void LoadDataTable () {

        try {
            loadState.OnNext (cxTGameDataLoadState.Loading);

            var config = cxDefaultAvatarConfig.GetAvatarConfig ();

            var path = config.avatarDataTablePath;
            var texts = Resources.LoadAll<TextAsset> (path);

            if (texts.Length == 0)
                cxLog.Warning ("cxDefaultGameItemBloc LoadDataTable data table not found");

            foreach (var text in texts) {
                var itemList = JsonConvert.DeserializeObject<List<TAvatarItemDescModel>> (text.text);
                avatarItemList.AddRange (itemList);
            }

            cxLog.Log ("cxDefaultGameItemBloc LoadDataTable count:{0}", avatarItemList.Count);
            loadState.OnNext (cxTGameDataLoadState.Loaded);
        } catch (Exception ex) {
            cxLog.Exception (ex);
            loadState.OnNext (cxTGameDataLoadState.Failed);

        }
    }

    public override TAvatarItemDescModel FindAvatarItemDesc (int itemCode) {
        return avatarItemList.Find (q => q.itemCode == itemCode);
    }

    public override async Task<GameObject> LoadAvatarMeshPrefab (TAvatarItemDescModel desc) {
        var config = cxDefaultAvatarConfig.GetAvatarConfig ();
        var url = Path.Combine (config.avatarMeshBasePath, desc.resourceURL);
        var gameObject = await cxUniversalResourceLoader.Instance.LoadAsset<GameObject> (url);
        return gameObject;
    }

    public override async Task<GameObject> LoadAvatarSetupPrefab () {
        var config = cxDefaultAvatarConfig.GetAvatarConfig ();
        return config.playerControllerSetup;

    }

    public override async Task<GameObject> LoadAvatarViewerSetupPrefab () {
        var config = cxDefaultAvatarConfig.GetAvatarConfig ();
        return config.playerViewerSetup;
    }
}