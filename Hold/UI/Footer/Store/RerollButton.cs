using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    public class RerollButton : MonoBehaviour {

        public LerpInt lerpCost;
        public TextMeshProUGUI costText;
        public Button button;

        public SizeAnimation sizeAnimation;
        public RotationAnimation rotAnimation;

        public enum ItemType {
            Sneaker,
            Crates,
        }

        public ItemType itemType;

        void Start() {
            // button = GetComponent<Button>();

            Player.instance.store.roller.onRerollInit.AddListener(SetCost);
            if (itemType == ItemType.Sneaker) {
                button.onClick.AddListener(Player.instance.store.roller.RerollSneakers);
                Player.instance.store.roller.onRerollSneakersEvent.AddListener(DrawSneakerRerollCost);
            }
            if (itemType == ItemType.Crate) {
                button.onClick.AddListener(Player.instance.store.roller.RerollCrates);
                Player.instance.store.roller.onRerollCratesEvent.AddListener(DrawCratesRerollCost);
            }
        }

        void FixedUpdate() {
            costText.text = lerpCost.currValue.ToString();
        }

        public void SetCost(int sneakerRerollCost, int cratesRerollCost) {
            if (itemType == ItemType.Sneaker) {
                SetSneakerRerollCost(sneakerRerollCost);
            }
            if (itemType == ItemType.Crate) {
                SetCratesRerollCost(cratesRerollCost);
            }
        }

        void DrawSneakerRerollCost(int newValue) {
            lerpCost.targetValue = newValue;
        }

        void SetSneakerRerollCost(int newValue) {
            lerpCost.currValue = newValue;
            sizeAnimation.Play();
            rotAnimation.Play();
        }

        void DrawCratesRerollCost(int newValue) {
            lerpCost.targetValue = newValue;
            sizeAnimation.Play();
            rotAnimation.Play();
        }

        void SetCratesRerollCost(int newValue) {
            lerpCost.currValue = newValue;
        }

    }

}
