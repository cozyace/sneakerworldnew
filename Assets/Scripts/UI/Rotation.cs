// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class Rotation : MonoBehaviour {

        public float rotationSpeed;

        public void FixedUpdate() {
            float dt = Time.fixedDeltaTime;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationSpeed * dt) * transform.rotation;
        }

    }

}
