// Unity.
using UnityEngine;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    [ExecuteInEditMode]
    public class NameObjectFromText : MonoBehaviour {

        public TextMeshProUGUI nameText;
        public string tag;
        
        void Update() {
            if (!Application.isPlaying) {
                gameObject.name = $"{nameText.text} {tag}";
            }
        }

    }

}
