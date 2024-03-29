using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class cxAbstractPlayerObject : MonoBehaviour {

    public Transform playerCameraTarget;
    public Transform nameTagAnchor;

    private bool isSandbox = false;
    private cxAvatarLocalStateController stateController;

    public TAvatarProfileModel avatarProfile {get ; private set;}
    public event Action<EmotionCode> onEmoticon;

    public static event Action<cxAbstractPlayerObject, string> OnMessage;

    protected virtual void Awake() {
        stateController = GetComponent<cxAvatarLocalStateController>();
    }

    protected virtual void Start () {
        // if (isSandbox) {
        //     localPlayerController.StartLocalPlayer ();
        // }
    }

    public void StartAsLocalPlayer(TAvatarProfileModel avatarProfile){
        this.avatarProfile = avatarProfile;

        SetupAvatar();
        SetupLocalPlayerController();
    }

    public void StartAsNpc(){

    }

    public void StartAsRemotePlayer(){

    }

    private void SetupAvatar () {
        cxAvatarMeshAssembly.Instance.AssemblyAvatar (gameObject, avatarProfile.equipSet);
    }

    private void SetupLocalPlayerController () {

        GetComponent<cxAvatarLocalStateController> ().enabled = true;
        stateController.StartLocalPlayer ();
       // cxAbstractSceneController.Instance.AcquireFocus(gameObject);
        
        /*

        cxGetIt.Get<mcClientConnectionBloc> ().ClientEventAsObservable
            .OfType<mcClientEvent, mcClientSwitchChannelReadyEvent> ()
            .Subscribe (state => {
                localPlayerController.Hold (true);
            }).AddTo (this);

        cxGetIt.Get<mcClientConnectionBloc> ().ClientEventAsObservable
            .OfType<mcClientEvent, mcClientSwitchChannelFailedEvent> ()
            .Subscribe (state => {
                localPlayerController.Hold (false);
            }).AddTo (this);
        cxGetIt.Get<mcClientConnectionBloc> ().ClientEventAsObservable
            .OfType<mcClientEvent, mcClientSwitchChannelCompletedEvent> ()
            .Subscribe (state => {
                localPlayerController.Hold (false);
            }).AddTo (this);
        */
    }

    private void OnDestroy() {
        
    }

    // /// <summary>
    // /// This is invoked on clients when the server has caused this object to be destroyed.
    // /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    // /// </summary>
    // public override void OnStopClient () {
    //     Debug.LogFormat ("[Player] OnStopClient playerId:{0}", syncUserProfile.userKey);
    //     cxGetIt.Get<mcClientConnectionBloc> ().ExitPlayer (this);
    // }

    // /// <summary>
    // /// Called when the local player object has been set up.
    // /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    // /// </summary>
    

    public void RpcChatReceive (string message) {
        OnMessage?.Invoke (this, message);
    }

    public void RpcShowEmoticon (EmotionCode emoId) {
        onEmoticon?.Invoke (emoId);
    }

    public abstract void ExecuteSpawn (cxAbstractPlayerObject playerController);
    public virtual void ExecuteLookAt (cxAbstractPlayerObject playerController, cxTrigger trigger) { }
    public virtual void ExecuteLookAtReleased (cxAbstractPlayerObject playerController) { }
    public virtual void ExecuteTouchCommand (cxTrigger trigger) { }
    public virtual void ExecuteTouchCommand (cxTrigger trigger, Vector3 touchPoint) { }

}