// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    [ExecuteInEditMode]
    public class NameObjectFromParent : MonoBehaviour {

        public int keepWords = -1;
        public string tag;
        
        void Update() {
            if (!Application.isPlaying) {
                string titleText = ToTitleCase(transform.parent.name);
                if (keepWords < 0) {
                    gameObject.name = $"{titleText} {tag}";
                }
                else {
                    string[] newName = titleText.Split(' ');
                    gameObject.name = "";
                    int min = Mathf.Min(keepWords, newName.Length);
                    for (int i = 0; i < min; i++) {
                        gameObject.name += newName[i];
                        gameObject.name += " ";
                    }
                    gameObject.name += tag;
                }
            }
        }

        private string ToTitleCase(string stringToConvert) {
            string firstChar= stringToConvert[0].ToString();
            return (stringToConvert.Length>0 ? firstChar.ToUpper()+stringToConvert.Substring(1) : stringToConvert);
        }

    }

}
