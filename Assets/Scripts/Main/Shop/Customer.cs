using System;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour {

    public enum State {
        Entering,
        Queuing,
        Purchasing,
        Exiting
    }

    private State state;
    public SkeletonAnimation skeleton;
    public float moveSpeed;
    public new Animator animator;

    private Transform exitWaypoint;
    private Transform enterWaypoint;
    private Transform currentWaypoint;
    
    private static readonly int Buy = Animator.StringToHash("Buy");

    private void Start() {
        //Get the store exit waypoint.
        // exitWaypoint = FindAnyObjectByType<StoreManager>().ActiveStore.EnterExitWaypoint;
    }


    private void Update() {
        switch (state) {
            case State.Entering:
                currentWaypoint = enterWaypoint;
                break;
            case State.Queuing:

                break;
            case State.Purchasing:

                break;
            case State.Exiting:

                break;
        }
        
    }

    void FixedUpdate() {
        MoveTowards(currentWaypoint, Time.fixedDeltaTime);
    }

    private void MoveTowards(Transform waypoint, float dt) {
        Vector3 displacement = (transform.position - waypoint.position);
        
        if (displacement.magnitude > moveSpeed * dt) {
            transform.position += displacement.normalized * moveSpeed * dt;
            skeleton.AnimationName = "Walk";
            animator.SetBool(Buy, false);
        }
        else {
            transform.position = waypoint.position;
            skeleton.AnimationName = "Idle";
            
            //If the AI has arrived at the exit location.
            // if (waypoint.CompareTag("ExitPoint") && Vector2.Distance(transform.position, waypoint.position) < moveSpeed * Time.deltaTime) {
            //     // Destroy properly.
            //     Destroy(gameObject);
            // }
            
            //Once the AI is done their purchase, they're sent to this waypoint, then when they reach it, they'll be sent to the exit.
            // if (waypoint.CompareTag("AwayFromCounterPoint")) {
            //     SetDestination(exitWaypoint);
            //     FlipSkeletonSprite();
            // }

        }
            
    }

    public void SetDestination(Transform newWaypoint) {
        currentWaypoint = newWaypoint;
    }

    private void FlipSkeletonSprite() {
        skeleton.transform.localScale = new Vector3(-skeleton.transform.localScale.x, 0.1f, 1f);
    }
    
}