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

        targetStopTime = 0;
    }

    public void ResetCamera () {
        //transform.position = target.position - target.forward * cameraDistance;
        if (followMode == CameraFollowMode.OrbitFollow) {

            cameraDistance = Mathf.Clamp (cameraDistance, cameraDistanceRange.x, cameraDistanceRange.y);

            var cameraView = target.forward; // target.position - transform.position);
            var viewRot = Quaternion.LookRotation (cameraView, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);
            cameraView = viewRot * Vector3.forward;
            var cameraPos = target.position - cameraView * cameraDistance;
            transform.rotation = viewRot;
            transform.position = cameraPos;
        }
    }

    public void ResetCameraSmooth () {
        targetStopTime = 10.0f;
    }

    void GetResetPosition (out Vector3 resetPos, out Quaternion resetRot) {
        cameraDistance = Mathf.Clamp (cameraDistance, cameraDistanceRange.x, cameraDistanceRange.y);

        var cameraView = target.forward; // target.position - transform.position);
        var viewRot = Quaternion.LookRotation (cameraView, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);
        cameraView = viewRot * Vector3.forward;
        var cameraPos = target.position - cameraView * cameraDistance;
        resetRot = viewRot;
        resetPos = cameraPos;
    }

    /*
        IEnumerator SmoothReposition(){
            float duration = 1.0f;
            float n =0 ;
            float t = Time.time;
            do {
                yield return null;
                n = (Time.time - t) / duration;
                transform.position = Vector3.SmoothDamp(transform.position, ,)
            } while( n < 1);

        }
    */

    public void LookAt (Camera camera) {
        if (camera) {
            lookAtTarget = camera.transform;
            SetLookAtCamera (lookAtTarget);
        } else {
            lookAtTarget = null;
            targetStopTime -= 2.0f;
        }
    }

    void SetLookAtCamera (Transform lookAtTarget) {
        transform.position = lookAtTarget.position;
        transform.rotation = lookAtTarget.rotation;
    }

    void LateUpdate () {
        UpdateCamera (Time.deltaTime);
    }

    Vector3 prevTargetPosition = Vector3.zero;
    Vector3 prevTargetRotation = Vector3.zero;

    float targetStopTime = 0;

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

                /*
                                UpdateOrbit ();

                                float moveDelta = Vector3.Distance (prevTargetPosition, target.position);
                                float rotDelta = Vector3.Distance (prevTargetRotation, target.rotation.eulerAngles);
                                if (moveDelta < 0.0001f && rotDelta < 0.0001f) {
                                    // if (targetStopTime == 0) targetStopTime = Time.time;
                                    targetStopTime += Time.deltaTime;
                                } else {
                                    targetStopTime = 0;
                                }

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

                                if ((targetStopTime) > 0.1f) {

                                    GetResetPosition (out Vector3 resetPos, out Quaternion resetRot);

                                    Vector3 currentVel = Vector3.zero;
                                    float smoothResetTime = 20.0f;
                                    transform.position = Vector3.SmoothDamp (transform.position, resetPos, ref currentVel, smoothResetTime * Time.deltaTime, float.MaxValue);
                                    //transform.rotation = Quaternion.Lerp()
                                }

                                prevTargetPosition = target.position;
                                prevTargetRotation = target.rotation.eulerAngles;
                            }
                    */

                UpdateOrbit ();

                float moveDelta = Vector3.Distance (prevTargetPosition, target.position);
                float rotDelta = Vector3.Distance (prevTargetRotation, target.rotation.eulerAngles);
                if (moveDelta < 0.0001f && rotDelta < 0.0001f) {
                    // if (targetStopTime == 0) targetStopTime = Time.time;
                    targetStopTime += Time.deltaTime;
                } else {
                    targetStopTime = 0;
                }

                Vector3 cameraView = Vector3.zero;
                Quaternion viewRot = Quaternion.identity;

                if ((targetStopTime) > 0.1f) {

                    GetResetPosition (out Vector3 resetPos, out Quaternion resetRot);

                    // Vector3 currentVel = Vector3.zero;
                    // float smoothResetTime = 20.0f;
                    // var viewRotEular = Vector3.SmoothDamp (transform.rotation.eulerAngles, resetRot.eulerAngles, ref currentVel, smoothResetTime * Time.deltaTime, float.MaxValue);

                    float n = (targetStopTime - 0.1f) / 5.0f;
                    float t = n * n * n;

                    var curRot = transform.rotation.eulerAngles;
                    var targetRot = resetRot.eulerAngles;
                    Vector3 viewRotEular = Vector3.zero;

                    viewRotEular.x = Mathf.LerpAngle (curRot.x, targetRot.x, t);
                    viewRotEular.y = Mathf.LerpAngle (curRot.y, targetRot.y, t);
                    viewRotEular.z = Mathf.LerpAngle (curRot.z, targetRot.z, t);

                    viewRot = Quaternion.Euler (viewRotEular);
                } else {

                    cameraView = (target.position - transform.position);
                    cameraView.y = 0;
                    cameraView.Normalize ();

                    viewRot = Quaternion.LookRotation (cameraView, Vector3.up) * Quaternion.Euler (cameraOrbitX, 0, 0);
                }

                cameraView = viewRot * Vector3.forward;

                var cameraPos = target.position - cameraView * cameraDistance;

                int layer = cameraCollisionMask.value; // 1 << LayerMask.NameToLayer ("Default");
                float smoothTime = 0.0f;

                var physicsScene = target.gameObject.scene.GetPhysicsScene ();
                if (physicsScene.SphereCast (target.position, cameraSphereRadius, -cameraView, out RaycastHit hit, cameraDistance, layer)) {
                    cameraPos = hit.point + cameraView * cameraSphereRadius;
                }

                transform.rotation = viewRot;
                transform.position = cameraPos;

                prevTargetPosition = target.position;
                prevTargetRotation = target.rotation.eulerAngles;
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