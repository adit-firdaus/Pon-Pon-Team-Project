using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace FPSFramework
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {
            if (!LoadingScreen.Instance)
                SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        }

        public void LoadGame(string name)
        {
            StartCoroutine(LoadingScreen.Instance.LoadSceneAsync(name));
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}