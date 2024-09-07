// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public class CustomContentFitter : MonoBehaviour {

        public Transform fitTo;

        public float baseHeight = 1000;
        public float heightPer = 1000;
        public int unitCount = 2;

        
        void Update() {
            float y = baseHeight + fitTo.childCount * heightPer / (float)unitCount; 
            RectTransform thisRt = GetComponent<RectTransform>();
            thisRt.sizeDelta = new Vector2(thisRt.sizeDelta.x, y);

            // if (fitTo.childCount <= 0) {
            //     return;
            // }

            // Vector3[] v = new Vector3[4];
            // Vector3 min;
            // Vector3 max;
            // min = new Vector3(Mathf.Infinity, Mathf.Infinity, 0f);
            // max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, 0f);

            // foreach (Transform child in fitTo) {
            //     RectTransform rt = child.GetComponent<RectTransform>();
            //     rt.GetLocalCorners(v);

            //     if (v[0].x < min.x) {
            //         min.x = v[0].x;
            //     }
            //     if (v[0].y < min.y) {
            //         min.y = v[0].y;
            //     }

            //     if (v[2].x < max.x) {
            //         max.x = v[2].x;
            //     }
            //     if (v[2].y < max.y) {
            //         max.y = v[2].y;
            //     }

            // }

            // RectTransform thisRt = GetComponent<RectTransform>();

            // Vector3 size = max - min;
            // Vector3 center = (max + min) / 2f;

            // thisRt.sizeDelta = size;
            // thisRt.anchoredPosition = center;
            // // thisRt.rect = new Rect(center.x, center.y, size.x, size.y);

        }

    }

}
