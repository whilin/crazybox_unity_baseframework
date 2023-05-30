using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class cxDefaultSceneController : cxAbstractSceneController {
    public List<cxRespawnPoint> respawnPoints;

    protected override void Awake () {
        base.Awake ();
        playerCamera = FindFirstObjectByType<cxPlayerCameraController> ();
    }

    private void Start () {

        cxGetIt.Get<cxIGamePlayerBloc> ().LoadStateAsObservable 
            .Where (state => state == cxTGameDataLoadState.Loaded)
            .Subscribe (state =>
                SpwanMyPlayerObject ()
            );

        cxGetIt.Get<cxIGamePlayerBloc> ().LoadStateAsObservable 
            .Where (state => state == cxTGameDataLoadState.Failed)
            .Subscribe (state => {

                }
            );
    }

    async void SpwanMyPlayerObject () {

        var myProfile = cxGetIt.Get<cxIGamePlayerBloc> ().CurrentGameProfile;
        var setupPrefab = await cxGetIt.Get<cxIGameItemBloc> ().LoadAvatarSetupPrefab ();
        var respawn = FindRandomRespawn ();
        if(respawn == null)
            throw new Exception("SpwanMyPlayerObject failed, spawn point not found");
            
        var playerObj = GameObject.Instantiate (setupPrefab, respawn.transform.position, respawn.transform.rotation);
        playerObject = playerObj.GetComponent<cxAbstractPlayerObject> ();
        playerObject.StartAsLocalPlayer(myProfile);
    }

    cxRespawnPoint FindRandomRespawn () {
        if (respawnPoints.Count == 0)
            return null;

        int idx = UnityEngine.Random.Range (0, respawnPoints.Count);
        return respawnPoints[idx];
    }
}