using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public sealed class cxAvatarNaviAgentMoveController : MonoBehaviour {
    private NavMeshAgent navAgent;
    private Animator animator;

    private bool moving;
    private Vector2 smoothDeltaPosition;
    private Vector2 velocity;

    public float stopDistance = 0.1f;

    public bool IsStop => !moving;

    private cxAvatarLocalStateController _stateController;

    private void Awake () {
        navAgent = GetComponent<NavMeshAgent> ();
        animator = GetComponent<Animator> ();
        _stateController = GetComponent<cxAvatarLocalStateController> ();
    }

    private void Start () {
        moving = false;
        navAgent.updatePosition = false;
    }

    private void OnEnable () {
        navAgent.enabled = true;
    }

    private void OnDisable () {
        navAgent.enabled = false;
    }

    public bool MoveTo (Vector3 pos) {

        if (NavMesh.SamplePosition (pos, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas)) {
            navAgent.isStopped = false;
            navAgent.updatePosition = false;
            navAgent.SetDestination (navHit.position);
            moving = true;
            return true;
        }

        return false;
    }

    public void Stop () {
        moving = false;
        navAgent.isStopped = true;
        // animator.SetFloat ("Speed", 0);
        navAgent.updatePosition = true;
    }

    // Update is called once per frame
    void Update () {
        _stateController.stateOutput.Reset ();

        if (moving) {
            UpdateNavi ();

            float d = Vector3.Distance (navAgent.destination, transform.position);
            if (d < stopDistance) {
                Stop ();
            }
        }
    }

    void UpdateNavi () {
        Vector3 worldDeltaPosition = navAgent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);

        /*
                    // Low-pass filter the deltaMove
                    float smooth = Mathf.Min (1.0f, Time.deltaTime / 0.15f);
                    smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

                    // Update velocity if time advances
                    if (Time.deltaTime > 1e-5f)
                        velocity = smoothDeltaPosition / Time.deltaTime;
        */

        velocity = deltaPosition / Time.deltaTime;

        //velocity = worldDeltaPosition / Time.deltaTime;

        _stateController.stateOutput.speed = velocity.magnitude;

        //animator.SetFloat ("Speed", velocity.magnitude);
        //animator.SetFloat ("MotionSpeed", velocity.magnitude > 0.01f ? 1 : 0);
        //animator.SetFloat ("MotionSpeed", 1.0f);

        //bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        //// Update animation parameters
        //anim.SetBool("move", shouldMove);
        //anim.SetFloat("velx", velocity.x);
        //anim.SetFloat("vely", velocity.y);

        // GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;

        transform.position = navAgent.nextPosition;

        //  var targetForward = naviMeshAgent.nextPosition - transform.position;
        //  var targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        //  transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);

    }

    public void ResetNavMeshAgent () {
        navAgent.enabled = false;
        StartCoroutine (WaitSetNavMeshAgent ());
    }
    IEnumerator WaitSetNavMeshAgent () {
        yield return new WaitForSeconds (0.1f);
        navAgent.enabled = true;
    }
}