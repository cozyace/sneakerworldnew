// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Tests {

    using Main;
    using UI;

    [ExecuteInEditMode]
    public class TestItemSlotUI : MonoBehaviour {

        public CrateData crateData;

        public ItemSlotUI itemSlotUI;

        void Update() {
            if (!Application.isPlaying) {
                if (itemSlotUI == null) { return; }
                itemSlotUI.Draw(crateData);
            }
        }

    }

}

