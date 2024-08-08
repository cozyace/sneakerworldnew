// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace GobbleFish.UI.Text {

    [ExecuteInEditMode, RequireComponent(typeof(ToggleGroup))]
    public class AutoToggleGroup : MonoBehaviour {

        public ToggleGroup toggleGroup;

        void Update() {
            if (!Application.isPlaying) {

                toggleGroup = GetComponent<ToggleGroup>();

                foreach (Transform child in transform) {
                    Toggle toggle = child.GetComponent<Toggle>();
                    if (toggle != null) {
                        toggle.group =  toggleGroup;
                    }

                }


            }
        }

    }

}