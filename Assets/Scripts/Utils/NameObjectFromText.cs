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
                string titleText = ToTitleCase(nameText.text);
                gameObject.name = $"{titleText} {tag}";
            }
        }

        private string ToTitleCase(string stringToConvert) {
            string firstChar= stringToConvert[0].ToString();
            return (stringToConvert.Length>0 ? firstChar.ToUpper()+stringToConvert.Substring(1) : stringToConvert);
        }

    }

}
