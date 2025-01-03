﻿// Unity.
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SneakerWorld.Graphics {

    using Input = UnityEngine.Input;

    /// <summary>
    /// A simple script to control the movement of the camera.
    /// </summary>
    public class CameraMoveHandler : MonoBehaviour {

        // Cache a reference to the camera.
        private Camera cam = null;

        // The ratio between pinching to zooming.
        [SerializeField]
        private float moveSpeed = 5f;  

        // The orthographic bounds of the zooming.
        [SerializeField]
        public Tilemap map;
        public Bounds moveBounds => map.localBounds;
        const float boundsLeeway = 0.5f; 
        
        // Caches the state of the zooming between frames.
        private bool wasMoving = false;
        private Vector2 cachedTouchPosition = new Vector2(0f, 0f);

        // The size the zoom snaps to on release.
        private bool animateRelease = false;
        private Vector2 targetPosition = new Vector2(0f, 0f);
        private Vector2 boundError = new Vector2(0f, 0f);
        
        // The curve that animates the release of the zoom.
        [SerializeField]
        private AnimationCurve moveReleaseCurve;
        
        // The duration since the pinch has been released.
        [SerializeField]
        private float releaseDuration;
        private float releaseTicks = 0f;


        // Runs once on instantiation.
        void Awake() {
            cam = Camera.main;
        }

        // Runs once every frame.
        void Update() {
            bool isMoving = IsMoving();
            if (isMoving) {
                if (!wasMoving) {
                    OnMove();
                }
                WhileMoving();
            }
            else {
                if (wasMoving) {
                    OnReleaseMove();
                }
                WhileMoveReleased(Time.deltaTime);
            }
            wasMoving = isMoving;

        }

        // What happens when the player starts zooming.
        private void OnMove() {
            // Not implemented.
        }

        // What happens while the player is zooming.
        private void WhileMoving() {
            Vector2 touchPosition = GetTouchPosition();

            Vector2 deltaTouchPosition = touchPosition - cachedTouchPosition;
            Vector2 deltaCamPosition = deltaTouchPosition * moveSpeed;

            cam.transform.localPosition += (Vector3)deltaCamPosition;
            
            Vector2 camPosition = cam.transform.localPosition;
            Mathf.Clamp(camPosition.x, moveBounds.min.x, moveBounds.max.x);
            Mathf.Clamp(camPosition.y, moveBounds.min.y, moveBounds.max.y);

            cam.transform.localPosition = camPosition;
            
            cachedTouchPosition = touchPosition;
        }

        // What happens when the player stops pinching.
        private void OnReleaseMove() {
            releaseTicks = 0f;
            animateRelease = false;
        }

        // What happens while the pinch is released.
        private void WhileMoveReleased(float deltaTime) {
            
        }

        // Check whether the player is zooming.
        private bool IsMoving() {
            return Input.touchCount == 1;
        }

        // Calculate the distance between two touches.
        private Vector2 GetTouchPosition() {
            return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        
    }

}