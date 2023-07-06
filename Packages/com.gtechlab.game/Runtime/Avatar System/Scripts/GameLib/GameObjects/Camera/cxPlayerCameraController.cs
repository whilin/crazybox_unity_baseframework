using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
[DefaultExecutionOrder (10)]
public class cxPlayerCameraController : MonoBehaviour {

    public enum CameraFollowMode {
        FixedViewFollow,
        BackwardFollow,
        OrbitFollow
    }

    public Vector2 cameraOrbitXRange = new Vector2 (0, 45.0f);
    public float orbitSpeed = 1.0f;

    public Vector2 cameraDistanceRange = new Vector2 (1.0f, 5.0f);
    public float distanceSpeed = 0.1f;

    public float cameraUP = 0.6f;

    public float cameraSphereRadius = 0.1f;
    public LayerMask cameraCollisionMask;

    [Header ("Runtime Value")]
    public float cameraDistance = 1.0f;
    public float cameraOrbitX = 45;
    public float cameraOrbitY = 0;

    private Vector3? prevPostion;
    private Transform target;
    private CameraFollowMode followMode;
    private Transform lookAtTarget;

    public void Follow (Transform target, CameraFollowMode followMode) {
        this.target = target;
        this.followMode = followMode;
        lookAtTarget = null;
    }

    public void ResetCamera () {
        //transform.position = target.position - target.forward * cameraDistance;
        if (followMode == CameraFollowMode.OrbitFollow) {

            cameraDistance = Mathf.Clamp(cameraDistance , cameraDistanceRange.x , cameraDistanceRange.y);

            var cameraView = target.forward;// target.position - transform.position);
            var viewRot = Quaternion.LookRotation (cameraView, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);
            cameraView = viewRot * Vector3.forward;
            var cameraPos = target.position - cameraView * cameraDistance;
            transform.rotation = viewRot;
            transform.position = cameraPos;
        }
    }

    public void LookAt (Camera camera) {
        if (camera) {
            lookAtTarget = camera.transform;
            SetLookAtCamera (lookAtTarget);
        } else {
            lookAtTarget = null;
        }
    }

    void SetLookAtCamera (Transform lookAtTarget) {
        transform.position = lookAtTarget.position;
        transform.rotation = lookAtTarget.rotation;
    }

    void LateUpdate () {
        UpdateCamera (Time.deltaTime);
    }

    void UpdateCamera (float deltaTime) {
        if (lookAtTarget) {
            transform.position = lookAtTarget.position;
            transform.rotation = lookAtTarget.rotation;
        } else if (target != null) {

            if (followMode == CameraFollowMode.FixedViewFollow) {
                UpdateOrbit ();

                var cameraPos = target.position - transform.forward * cameraDistance;

                int layer = cameraCollisionMask.value; // 1 << LayerMask.NameToLayer ("Default");
                float smoothTime = 0.0f;

                var physicsScene = target.gameObject.scene.GetPhysicsScene ();
                if (physicsScene.SphereCast (target.position, cameraSphereRadius, -transform.forward, out RaycastHit hit, cameraDistance, layer)) {
                    cameraPos = hit.point + transform.forward * cameraSphereRadius;
                }

                //Note. 이종옥, 기능 제거 
                // if (Input.GetKeyDown (KeyCode.T)) {
                //     cameraDistanceRange = new Vector2 (0f, 0f);
                // } else if (Input.GetKeyDown (KeyCode.U)) {
                //     cameraDistanceRange = new Vector2 (1.0f, 5.0f);
                // }

                transform.rotation = Quaternion.Euler (cameraOrbitX, cameraOrbitY, 0);
                transform.position = new Vector3 (cameraPos.x, cameraPos.y + cameraUP, cameraPos.z);
            } else if (followMode == CameraFollowMode.BackwardFollow) {
                UpdateOrbit ();

                var viewRot = Quaternion.LookRotation (target.forward, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);

                var cemeraNextView = viewRot * Vector3.forward;
                var cameraNextPos = target.position - cemeraNextView * cameraDistance;

                // Direct Set
                // transform.position = cameraNextPos;
                // transform.rotation = viewRot;// Quaternion.LookRotation(cemeraNextView2, Vector3.up) *  Quaternion.Euler(-cameraOrbitX,0,0);

                Vector3 vel = Vector3.zero;
                float smoothTime = 1.0f * deltaTime;
                var cameraPos = Vector3.SmoothDamp (transform.position, cameraNextPos, ref vel, smoothTime, float.MaxValue);
                var cameraView = Vector3.SmoothDamp (transform.forward, cemeraNextView, ref vel, smoothTime, float.MaxValue);

                int layer = cameraCollisionMask.value; // 1 << LayerMask.NameToLayer ("Default");
                var physicsScene = target.gameObject.scene.GetPhysicsScene ();
                if (physicsScene.SphereCast (target.position, cameraSphereRadius, -cameraView, out RaycastHit hit, cameraDistance, layer)) {
                    cameraPos = hit.point + transform.forward * cameraSphereRadius;
                }

                transform.position = cameraPos;
                transform.rotation = Quaternion.LookRotation (cameraView, Vector3.up); //*  Quaternion.Euler(-cameraOrbitX,0,0);
            } else if (followMode == CameraFollowMode.OrbitFollow) {

                UpdateOrbit ();

                var cameraView = (target.position - transform.position);
                cameraView.y = 0;
                cameraView.Normalize ();
                var viewRot = Quaternion.LookRotation (cameraView, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);
                cameraView = viewRot * Vector3.forward;

                var cameraPos = target.position - cameraView * cameraDistance;

                int layer = cameraCollisionMask.value; // 1 << LayerMask.NameToLayer ("Default");
                float smoothTime = 0.0f;

                var physicsScene = target.gameObject.scene.GetPhysicsScene ();
                if (physicsScene.SphereCast (target.position, cameraSphereRadius, -cameraView, out RaycastHit hit, cameraDistance, layer)) {
                    cameraPos = hit.point + cameraView * cameraSphereRadius;
                }

                //Note. 이종옥, 기능 제거 
                // if (Input.GetKeyDown (KeyCode.T)) {
                //     cameraDistanceRange = new Vector2 (0f, 0f);
                // } else if (Input.GetKeyDown (KeyCode.U)) {
                //     cameraDistanceRange = new Vector2 (1.0f, 5.0f);
                // }

                transform.rotation = viewRot; // Quaternion.LookRotation(cameraView, Vector3.up);
                transform.position = cameraPos;

            }
        }
    }

    public void SetCamera (Vector2 delta) {
        float invTime = (1.0f / Time.deltaTime);

        cameraOrbitY += delta.x * orbitSpeed * invTime;
        if (cameraOrbitY > 360) cameraOrbitY -= 360f;
        if (cameraOrbitY < -360) cameraOrbitY += 360f;

        cameraOrbitX += delta.y * orbitSpeed * invTime;
        cameraOrbitX = Mathf.Clamp (cameraOrbitX, cameraOrbitXRange.x, cameraOrbitXRange.y);
    }

    void UpdateOrbit () {
        float invTime = (1.0f / Time.deltaTime);
        Vector2 delta = Vector2.zero;

        if (Input.GetMouseButton (1)) {
            if (prevPostion.HasValue) {

                delta = Input.mousePosition - prevPostion.Value;
                SetCamera (new Vector2 (delta.x, delta.y));
                /*
                //var delta2 = delta / Time.deltaTime;
                cameraOrbitY += delta.x * orbitSpeed * invTime;
                if (cameraOrbitY > 360) cameraOrbitY -= 360f;
                if (cameraOrbitY < -360) cameraOrbitY += 360f;

                cameraOrbitX += delta.y * orbitSpeed * invTime;
                cameraOrbitX = Mathf.Clamp (cameraOrbitX, cameraOrbitXRange.x, cameraOrbitXRange.y);
                */
            }
            prevPostion = Input.mousePosition;
        } else {
            prevPostion = null;
        }

        SetCamera (new Vector2 (delta.x, delta.y));
        Vector2 scroll = Input.mouseScrollDelta;
        cameraDistance += scroll.y * invTime * distanceSpeed;
        cameraDistance = Mathf.Clamp (cameraDistance, cameraDistanceRange.x, cameraDistanceRange.y);

    }
    /*
    public void SetZoomInOut () {
        if (cameraDistanceRange == new Vector2 (0f, 0f)) {
            cameraDistanceRange = new Vector2 (1.0f, 5.0f);
        } else {
            cameraDistanceRange = new Vector2 (0f, 0f);
        }
    }
    public void SetZoomIn () {
        cameraDistanceRange = new Vector2 (0f, 0f);
    }
    public void SetZoomOut () {
        cameraDistanceRange = new Vector2 (1.0f, 5.0f);
    }
    */

    public void SetInitCameraValue () {
        cameraDistance = 2.0f;
        cameraOrbitX = 35.0f;
        cameraOrbitY = 0.0f;
    }
}