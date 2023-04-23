using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cxAvatarViewStudio : MonoSingleton<cxAvatarViewStudio> {

    [SerializeField]
    private Camera avatarViewCamera;
    [SerializeField]
    private Transform standingPivot;
    [SerializeField]
    private Light viewLight;

    [SerializeField]
    private int renderTextureSize = 1024;

    [SerializeField]
    public LayerMask viewLayer;

    RenderTexture renderTexture;
    GameObject avatarObject;
    GameObject setupPrefab;

    override protected void Awake () {
        base.Awake ();

        avatarViewCamera.gameObject.SetActive (false);
        viewLight.gameObject.SetActive (false);
        avatarViewCamera.cullingMask = viewLayer;
        viewLight.cullingMask = viewLayer;
    }

    override protected void OnDestroy () {
       
        if (renderTexture != null)
            GameObject.Destroy (renderTexture);

        if (avatarObject != null)
            Destroy (avatarObject);

        base.OnDestroy ();
    }


    public RenderTexture StartStudio (GameObject setupPrefab, TAvatarEquipSetModel equipSet=null) {
        if (renderTexture == null)
            CreateRenderTexture ();
        
        if (avatarObject != null)
            Destroy (avatarObject);

        this.setupPrefab = setupPrefab;
        avatarViewCamera.gameObject.SetActive (true);
        viewLight.gameObject.SetActive (isLight);

        avatarObject = GameObject.Instantiate (setupPrefab, standingPivot.position, standingPivot.rotation);
        avatarObject.transform.SetParent (standingPivot, true);
        int layer = ToLayerNumber (viewLayer);

        if(equipSet!=null)
          SetAvatar (equipSet, true);

        return renderTexture;
    }

    public void ChangeEquipSet(TAvatarEquipSetModel equipSet){
         SetAvatar (equipSet, true);
    }

    public RenderTexture GetRenderingTexture(){
        return renderTexture;
    }

    public void StopStudio () {
        isRotation = false;
        avatarViewCamera.gameObject.SetActive (false);
        viewLight.gameObject.SetActive (false);
        setupPrefab = null;

        if (avatarObject != null)
            Destroy (avatarObject);
    }

    void CreateRenderTexture () {
        renderTexture = new RenderTexture (renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create ();
        avatarViewCamera.targetTexture = renderTexture;
    }

    public static int ToLayerNumber(LayerMask mask) {
        int layerNumber = 0;
        int layer = mask.value;
        while(layer > 1) {
            layer = layer >> 1;
            layerNumber++;
        }

        return layerNumber;
    }

     void SetAvatar (TAvatarEquipSetModel equipSet, bool reset) {
        // if (avatarObject != null)
        //     Destroy (avatarObject);

        // GameObject prefabAvartViewerObject =  await cxGetIt.Get<cxIGameItemBloc>().LoadAvatarViewerSetupPrefab();

        // await new WaitForUpdate();

        // avatarObject = GameObject.Instantiate (prefabAvartViewerObject, standingPivot.position, standingPivot.rotation);
        // avatarObject.transform.SetParent (standingPivot, true);

        int layer = ToLayerNumber (viewLayer);// LayerMask.NameToLayer ("AvatarView");

        if(reset)
            cxAvatarMeshAssembly.ClearSkinnedMeshs(avatarObject);

        cxAvatarMeshAssembly.Instance.AssemblyAvatar (avatarObject, equipSet, layer);
    }

    private Vector3? prevPostion;
    public float cameraOrbitY = 0;
    public float orbitSpeed = 1.0f;
    public bool isRotation = false;
    private bool isLight = true;
    private void Update () {
        if (isRotation)
        {
            UpdateOrbit();
        }
    }

    void UpdateOrbit () {
        if (Input.GetMouseButton (1)) {
            if (prevPostion.HasValue) {
                var delta = Input.mousePosition - prevPostion.Value;
                SetCamera (new Vector2 (delta.x, delta.y));
            }
            prevPostion = Input.mousePosition;
        } else {
            prevPostion = null;
        }
        standingPivot.rotation = Quaternion.Euler (0, -cameraOrbitY, 0);
    }

    public void SetCamera (Vector2 delta) {
        float invTime = (0.05f / Time.deltaTime);

        cameraOrbitY += delta.x * orbitSpeed * invTime;
        if (cameraOrbitY > 360) cameraOrbitY -= 360f;
        if (cameraOrbitY < -360) cameraOrbitY += 360f;
    }
    public void InitAvatarSudio(bool _isLight)
    {
        isLight = _isLight;
        isRotation = true;
        cameraOrbitY = 0;
        standingPivot.rotation = Quaternion.identity;
    }
}
