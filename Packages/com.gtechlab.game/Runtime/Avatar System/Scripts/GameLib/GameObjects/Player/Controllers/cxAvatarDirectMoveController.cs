using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class cxAvatarDirectMoveController : MonoBehaviour {
    [Header ("Player")]
    [Tooltip ("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip ("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip ("How fast the character turns to face movement direction")]
    [Range (0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip ("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Space (10)]
    [Tooltip ("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip ("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space (10)]
    [Tooltip ("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip ("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header ("Player Grounded")]
    [Tooltip ("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip ("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip ("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip ("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // [Header ("Cinemachine")]
    // [Tooltip ("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    // public GameObject CinemachineCameraTarget;
    [Tooltip ("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip ("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip ("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip ("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private Animator _animator;
    private CharacterController _controller;
    private cxAvatarLocalStateController _stateController;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    private void Awake () {
        _hasAnimator = TryGetComponent (out _animator);
        _controller = GetComponent<CharacterController> ();
        _stateController = GetComponent<cxAvatarLocalStateController> ();
    }

    private void Start () {

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update () {
        _stateController.stateOutput.Reset ();

        JumpAndGravity ();

        GroundedCheck ();

        Move ();

        _stateController.stateInput.Consumed ();
    }

    private void LateUpdate () {
        //CameraRotation();
        _stateController.stateInput.Consumed2 ();
    }

    private void GroundedCheck () {

        // set sphere position, with offset
        //Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        //Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        //Scene Physics로 변경해봄!
        var physicsScene = gameObject.scene.GetPhysicsScene ();

        //Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        ////Grounded = physicsScene.SphereCast( spherePosition, GroundedRadius,Vector3.down,  out RaycastHit hitInfo, GroundedOffset, GroundLayers, QueryTriggerInteraction.Collide);
        //Grounded = physicsScene.Raycast(spherePosition, Vector3.down, out RaycastHit hitInfo, -GroundedOffset, GroundLayers);

        //float rayLength = GroundedRadius + 0.001f;
        //float step = 0.1f;
        //Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + rayLength , transform.position.z);
        //Grounded = physicsScene.SphereCast(spherePosition, GroundedRadius, Vector3.down, out RaycastHit hitInfo, rayLength+0.1f, GroundLayers);

        //var prevGround = Grounded;

        Vector3 spherePosition = new Vector3 (transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Collider[] results = new Collider[100];
        Grounded = physicsScene.OverlapSphere (spherePosition, GroundedRadius, results, GroundLayers, QueryTriggerInteraction.Ignore) > 0;

        _stateController.stateOutput.grounded = Grounded;

        // update animator if using character
        // if (_hasAnimator) {
        //     _animator.SetBool (_animIDGrounded, Grounded);
        // }

        // if(!prevGround && Grounded){
        //     Debug.Log("Landing...");
        // }
    }
    
    public bool GroundedCheckForRemotePlayer () {

      
        //Scene Physics로 변경해봄!
        var physicsScene = gameObject.scene.GetPhysicsScene ();


        Vector3 spherePosition = new Vector3 (transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Collider[] results = new Collider[100];
        bool grounded = physicsScene.OverlapSphere (spherePosition, GroundedRadius, results, GroundLayers, QueryTriggerInteraction.Ignore) > 0;

       return grounded;
    }

    // private void CameraRotation () {

    //     // if there is an input and camera position is not fixed
    //     if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
    //         _cinemachineTargetYaw += input.look.x * Time.deltaTime;
    //         _cinemachineTargetPitch += input.look.y * Time.deltaTime;
    //     }

    //     // clamp our rotations so our values are limited 360 degrees
    //     _cinemachineTargetYaw = ClampAngle (_cinemachineTargetYaw, float.MinValue, float.MaxValue);
    //     _cinemachineTargetPitch = ClampAngle (_cinemachineTargetPitch, BottomClamp, TopClamp);

    //     // Cinemachine will follow this target
    //     CinemachineCameraTarget.transform.rotation = Quaternion.Euler (_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);

    // }

    private void Move () {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _stateController.stateInput.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_stateController.stateInput.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3 (_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp (currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round (_speed * 1000f) / 1000f;
        } else {
            _speed = targetSpeed;
        }
        _animationBlend = Mathf.Lerp (_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3 (_stateController.stateInput.move.x, 0, _stateController.stateInput.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_stateController.stateInput.move != Vector2.zero) {

            var playerCamera = cxAbstractSceneController.Instance.GetPlayerCamera();
            
            _targetRotation = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle (transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler (0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler (0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move (targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3 (0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        // if (_hasAnimator) {
        //     _animator.SetFloat (_animIDSpeed, _animationBlend);
        //     _animator.SetFloat (_animIDMotionSpeed, inputMagnitude);
        // }

        _stateController.stateOutput.speed = _speed;// Mathf.Lerp (0, SprintSpeed, _speed);
    }

    private void JumpAndGravity () {
        if (Grounded) {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            // if (_hasAnimator) {
            //     _animator.SetBool (_animIDJump, false);
            //     _animator.SetBool (_animIDFreeFall, false);
            // }

            //_stateController.input.jump = false;

            _stateController.stateOutput.jump = false;
            _stateController.stateOutput.freeFall = false;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f) {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_stateController.stateInput.jump && _jumpTimeoutDelta <= 0.0f) {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt (JumpHeight * -2f * Gravity);

                _stateController.stateOutput.jump = true;

                // update animator if using character
                // if (_hasAnimator) {
                //     _animator.SetBool (_animIDJump, true);
                // }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f) {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        } else {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f) {
                _fallTimeoutDelta -= Time.deltaTime;
            } else {
                // update animator if using character
                // if (_hasAnimator) {
                //     _animator.SetBool (_animIDFreeFall, true);
                // }

                _stateController.stateOutput.freeFall = true;
            }

            // if we are not grounded, do not jump
            _stateController.stateInput.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity) {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle (float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp (lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected () {
        Color transparentGreen = new Color (0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color (1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere (new Vector3 (transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}