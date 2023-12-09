using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;
using UnityEngine;

namespace FPSFramework
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenu : MonoBehaviour
    {
        public bool freezOnPause = true;
        public GameObject UI;

        public UnityEvent OnPause;
        public UnityEvent OnResume;

        public static PauseMenu Instance;
        private CanvasGroup canvasGroup;
        private Volume postProcessingVolume;
        private DepthOfField depthOfField;

        public bool paused { get; set; }

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);

            if (!LoadingScreen.Instance)
                SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        }

        private void Start()
        {
            if(postProcessingVolume)
            postProcessingVolume = GameObject.Find("Global Volume").GetComponent<Volume>();
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = !paused;

                if (paused) OnPause?.Invoke();
                if (!paused) OnResume?.Invoke();
            }


            UI.SetActive(paused);

            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            if(canvasGroup)
            canvasGroup.alpha = paused ? Mathf.Lerp(canvasGroup.alpha, 1, Time.unscaledDeltaTime * 10) : 0;

            if (freezOnPause)
            {
                Time.timeScale = paused ? 0 : 1;
            }

            if (postProcessingVolume && postProcessingVolume.profile.TryGet(out depthOfField))
            {
                depthOfField.active = paused;
            }
        }

        public void Resume()
        {
            paused = false;
            OnResume?.Invoke();
            if (postProcessingVolume && postProcessingVolume.profile.TryGet(out depthOfField))
            {
                depthOfField.active = !paused;
            }
        }


        public void Pause()
        {
            paused = true;
            OnPause?.Invoke();
            if (postProcessingVolume && postProcessingVolume.profile.TryGet(out depthOfField))
            {
                depthOfField.active = !paused;
            }
        }
        public void Quit() => Application.Quit();
    }
}