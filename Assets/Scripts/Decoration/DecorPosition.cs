// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Decoration {

    [ExecuteInEditMode]
    public class DecorPosition : MonoBehaviour {

        private SpriteRenderer spriteRenderer;

        public Grid grid;
        public Vector3Int gridPosition;

        public bool selected = false;
        public Vector3Int selectedGridPosition;
        public float selectTime = 0.5f;
        public float ticks = 0f;

        public TouchPhase lastTouchPhase;

        void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update() {
            float dt = Time.deltaTime;

            if (grid != null) {
                transform.position = grid.GetCellCenterWorld(gridPosition);
            }

            if (Application.isPlaying) {
                if (selected) {
                    spriteRenderer.color = Color.red;
                }
                else {
                    spriteRenderer.color = Color.white;
                }

                if (Input.touchCount == 1) {

                    Touch touch = Input.GetTouch(0);
                    if (selected && touch.phase == TouchPhase.Moved) {
                        SetDecorPosition(touch);
                        lastTouchPhase = TouchPhase.Moved;
                    }
                    else if (!selected) {

                        if (touch.phase == TouchPhase.Began) {

                            Vector2 touchPosition = Input.GetTouch(0).position;
                            Vector3 worldPos = Camera.main.ScreenToWorldPoint((Vector3)touchPosition);
                            selectedGridPosition = grid.WorldToCell(worldPos);
                            lastTouchPhase = TouchPhase.Began;
                            
                        }
                        else if (touch.phase == TouchPhase.Stationary && selectedGridPosition.x == gridPosition.x && selectedGridPosition.y == gridPosition.y) {
                            ticks += dt;
                            if (ticks > selectTime) {
                                selected = true;
                            }
                            lastTouchPhase = TouchPhase.Stationary;

                        }

                    }
                    
                    if (selected && touch.phase == TouchPhase.Ended) {
                        Drop();
                    }

                }
                else {
                    ticks = 0f;
                    selected = false;
                }


            }
        }

        public void SetDecorPosition(Touch touch) {
            Vector2 touchPosition = Input.GetTouch(0).position;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint((Vector3)touchPosition);
            Debug.Log(worldPos);
            
            gridPosition = grid.WorldToCell(worldPos);
        }

        public void Drop() {
            ticks = 0f;
            selected = false;
        }

    }

}
