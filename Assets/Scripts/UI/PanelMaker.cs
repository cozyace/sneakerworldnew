// // System.
// using System;
// using System.Collections;
// // Unity.
// using UnityEngine;
// using UnityEngine.UI;

// namespace SneakerWorld.UI {

//     public class Panel {

//         public VerticalSlideAnimation animation;
//         public TextMeshProUGUI[] nameTexts;

//         public void SetName(string name) {
//             foreach (TextMeshProUGUI nameText in nameTexts) {
//                 nameText.text = name;
//             }
//         }

//         public void Open(bool open) {
//             animation.Open(open);
//         }

//     }

//     using Main;
//     using Utils;

//     /// <summary>
//     /// Listen to the wallet and draws it.
//     /// </summary>
//     public class PanelMaker : MonoBehaviour {

//         public string[] tabs = new string[];

//         public Panel panelTemplate;
//         public PanelNavigation panelNavTemplate;

//         void Update() {
//             if (!Application.isPlaying) {

//                 foreach (string tab in tabs) {
//                     Panel panel = CreatePanel(tab);
//                     CreateToggle(tab, panel);
//                 }

//             }
//         }

//         void Create(string name) {
//             Panel panel = Instantiate(panelTemplate).GetComponent<Panel>();
//             panel.SetName(name);

//             PanelNavigation newNav = Instantiate(panelNavTemplate).GetComponent<PanelNavigation>();
//             Toggle toggle = newNav.GetComponent<Toggle>();
//             toggle.onValueChange.AddListener(panel.Open);
//         }

//     }

// }
