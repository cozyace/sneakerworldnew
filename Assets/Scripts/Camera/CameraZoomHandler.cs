// // Unity.
// using UnityEngine;

// namespace SneakerWorld.Graphics {

//     using Input = UnityEngine.Input;

//     /// <summary>
//     /// A simple script to control the zoom of the camera.
//     /// </summary>
//     public class CameraZoomHandler : MonoBehaviour {

//         // Cache a reference to the camera.
//         private Camera cam = null;

//         // The ratio between pinching to zooming.
//         [SerializeField]
//         private float zoomSpeed = 0.5f;  

//         // The orthographic bounds of the zooming.
//         [SerializeField]
//         public Vector2 zoomBounds = new Vector2(3f, 20f);
//         intern const float zoomLeeway = 0.5f; 
        
//         // Caches the state of the zooming between frames.
//         private bool wasPinching = false;
//         private float cachedPinchDistance = 0f;

//         // The size the zoom snaps to on release.
//         private bool animateRelease = false;
//         private float targetBound = 0f;
//         private float deltaBound = 0f;
        
//         // The curve that animates the release of the zoom.
//         [SerializeField]
//         private AnimationCurve zoomReleaseCurve;
        
//         // The duration since the pinch has been released.
//         [SerializeField]
//         private float releaseDuration;
//         private float releaseTicks = 0f;


//         // Runs once on instantiation.
//         void Awake() {
//             cam = Camera.main;
//         }

//         // Runs once every frame.
//         void Update() {
//             bool isPinching = IsPinching();
//             if (isPinching) {
//                 if (!wasPinching) {
//                     OnPinch();
//                 }
//                 WhilePinching();
//             }
//             else {
//                 if (wasPinching) {
//                     OnReleasePinch();
//                 }
//                 WhilePinchReleased(Time.deltaTime);
//             }
//             wasPinching = isPinching;

//         }

//         // What happens when the player starts zooming.
//         private void OnPinch() {
//             // Not implemented.
//         }

//         // What happens while the player is zooming.
//         private void WhilePinching() {
//             float pinchDistance = GetPinchDistance();

//             float deltaPinch = pinchDistance - cachedPinchDistance;
//             float deltaOrth = deltaPinch * zoomSpeed;

//             cam.orthographicSize += deltaOrth;
//             Mathf.Clamp(cam.orthographicSize, zoomBounds.x - zoomLeeway, zoomBounds.y + zoomLeeway);
            
//             cachedPinchDistance = pinchDistance;
//         }

//         // What happens when the player stops pinching.
//         private void OnReleasePinch() {
//             releaseTicks = 0f;
//             animateRelease = false;

//             if (cam.orthographicSize > zoomBounds.y) {
//                 targetBound = zoomBounds.y;
//                 deltaBound = cam.orthographicSize - zoomBounds.y;
//                 animateRelease = true;
//             }
//             else if (cam.orthographicSize < zoomBounds.x) {
//                 targetBound = zoomBounds.x;
//                 deltaBound = -(zoomBounds.x - cam.orthographicSize);
//                 animateRelease = true;
//             }
//         }

//         // What happens while the pinch is released.
//         private void WhilePinchReleased(float deltaTime) {
//             if (!animateRelease) { return; }

//             releaseTicks += deltaTime;
//             if (releaseTicks > releaseDuration) {
//                 Mathf.Clamp(cam.orthographicSize, zoomBounds.x, zoomBounds.y);
//                 animateRelease = false;
//             }

//             float t = animationTicks / animationDuration;
//             cam.orthographicSize = targetBound + deltaBound * zoomReleaseCurve.Evaluate(t);

//         }

//         // Check whether the player is zooming.
//         private bool IsPinching() {
//             return Input.touchCount == 2;
//         }

//         // Calculate the distance between two touches.
//         private float GetPinchDistance() {
//             return Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
//         }
        
//     }

// }