using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Authentication
{
    public class AuthUIManager : MonoBehaviour
    {
        public static AuthUIManager instance;

        [Header("References")]
        [SerializeField] private GameObject loginUI;
        [SerializeField] private GameObject registerUI;
        [SerializeField] private GameObject loadingUI;

        private void Start()
        {
            // FirebaseManager.instance.AssignObjects();
        }

        private void ClearUI()
        {
            loginUI.SetActive(false);
            registerUI.SetActive(false);
            // loadingUI.SetActive(false);
        }

        public void LoginScreen()
        {
            ClearUI();
            loginUI.SetActive(true);
        }

        public void RegisterScreen()
        {
            ClearUI();
            registerUI.SetActive(true);
        }
        
        public void LoadingScreen()
        {
            ClearUI();
            // loadingUI.SetActive(true);
        }
        
        public void ChangeScene(int sceneIndex)
        {
            SceneManager.LoadSceneAsync(sceneIndex);
        }
    }
}