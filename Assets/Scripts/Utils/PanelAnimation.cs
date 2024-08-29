// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class Panel : MonoBehaviour {

        [System.Serializable]
        public class PanelDetails {
            public string name;
            public Sprite icon;
            public string[] tabs;
        }

        public PanelDetails details;

        // The ui components.
        public TextMeshProUGUI nameText;
        public Image iconImage; 
        public RectTransform navRect; 

        // Animating the opening and closing of the panel.
        public Animator animator;
        private bool closing;

        public AnimationCurve openPosCurve;
        public float posYFactor;
        public AnimationCurve openScaleCurve;
        public float scaleYFactor;
        Vector3 localScale;
        Vector3 origin;

        float ticks = 0f;
        public float duration = 0.5f;

        void Awake() {
            if (animator == null) {
                foreach (Transform child in transform) {
                    animator = child.GetComponent<Animator>();
                    if (animator != null) {
                        break;
                    }
                }
            }

            origin = animator.transform.localPosition;
            localScale = animator.transform.localScale;
        }

        void Update() {
            if (!Application.isPlaying) {
                nameText.text = details.name;
                iconImage.sprite = details.icon;

                // if (details.tabs.Length > 0) {
                    
                //     Debug.Log(navRect.rect.width);
                //     GridLayoutGroup navGroup = navRect.GetComponent<GridLayoutGroup>();
                //     navGroup.cellSize = new Vector2(navRect.rect.width / details.tabs.Length, navRect.rect.height);

                // }
                
            }

            float factor = closing ? -1f : 1f;
            
            ticks += factor * Time.deltaTime;
            if (ticks >= duration || ticks <= 0f) {
                ticks = Mathf.Clamp(ticks, 0f, duration);
                return;
            }

            animator.transform.localPosition = origin + posYFactor * Vector3.up * openPosCurve.Evaluate(ticks/duration);
            transform.localScale = localScale + scaleYFactor * Vector3.up * openScaleCurve.Evaluate(ticks/duration);

        }

        public void Open(bool open) {
            closing = !open;

            if (animator == null) {
                foreach (Transform child in transform) {
                    animator = child.GetComponent<Animator>();
                    if (animator != null) {
                        break;
                    }
                }
            }
            if (open) { 
                if (!gameObject.activeSelf) {
                    gameObject.SetActive(true);
                    
                }
                animator.gameObject.SetActive(true);
                // animator.Play("OpenWindow"); 
                closing = false;
            }
            else {
                if (gameObject.activeSelf && !closing) {
                    // animator.Play("CloseWindow");
                    closing = true;
                }
            }

        }


    }

}
