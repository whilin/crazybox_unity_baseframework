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


    RenderTexture renderTexture;
    GameObject avatarObject;

    override protected void Awake () {
        base.Awake ();

        avatarViewCamera.gameObject.SetActive (false);
        viewLight.gameObject.SetActive (false);
    }

    override protected void OnDestroy () {
       
        if (renderTexture != null)
            GameObject.Destroy (renderTexture);

        if (avatarObject != null)
            Destroy (avatarObject);

        base.OnDestroy ();
    }

    void CreateRenderTexture () {
        renderTexture = new RenderTexture (renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create ();
        avatarViewCamera.targetTexture = renderTexture;
    }

    // public RenderTexture StartRendering (int avatarType) {
    //     if (renderTexture == null)
    //         CreateRenderTexture ();

    //     SetaAvatar (avatarType);

    //     avatarViewCamera.gameObject.SetActive (true);
    //     viewLight.gameObject.SetActive (isLight);

    //     return renderTexture;
    // }

    public RenderTexture StartRendering (TAvatarEquipSetModel equipSet) {
        if (renderTexture == null)
            CreateRenderTexture ();

        SetAvatar (equipSet);

        avatarViewCamera.gameObject.SetActive (true);
        viewLight.gameObject.SetActive (isLight);

        return renderTexture;
    }

    public void StopRendering () {
        isRotation = false;
        avatarViewCamera.gameObject.SetActive (false);
        viewLight.gameObject.SetActive (false);

        if (avatarObject != null)
            Destroy (avatarObject);
    }

     async void SetAvatar (TAvatarEquipSetModel equipSet) {
        if (avatarObject != null)
            Destroy (avatarObject);

        GameObject prefabAvartViewerObject =  await  cxGetIt.Get<cxIGameItemBloc>().LoadAvatarViewerSetupPrefab();

        await new WaitForUpdate();

        avatarObject = GameObject.Instantiate (prefabAvartViewerObject, standingPivot.position, standingPivot.rotation);
        avatarObject.transform.SetParent (standingPivot, true);

        int layer = LayerMask.NameToLayer ("AvatarView");

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
