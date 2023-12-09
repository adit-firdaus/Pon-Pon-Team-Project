using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace FPSFramework
{
    [AddComponentMenu("UI/Selectable Behavior"), RequireComponent(typeof(Selectable))]
    public class SelectableBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public TextMeshProUGUI text;
        public RectTransform image;
        public CanvasGroup canvasGroup;
        public float imageSize = 1000;
        public Color normal = Color.white;
        public Color highlight = Color.gray;
        public float movementSpeed = 20;
        public float imageFadeSpeed = 1;
        public float fadeSpeed = 10;
        public AudioClip pointerEnterSFX;
        public AudioClip buttonDownSFX;

        bool isOn;
        AudioSource audioSource;
        Selectable selectable;

        private void Start()
        {
            if (text)
            {
                text.color = normal;
            }

            selectable = GetComponent<Selectable>();
        }

        private void OnEnable()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            isOn = false;

            if (canvasGroup)
            {
                canvasGroup.alpha = 0;
            }
            if (image)
            {
                image.sizeDelta = Vector2.one * imageSize;
            }
        }

        private void Update()
        {
            if (!selectable.interactable) return;

            if (isOn) A(); else B();

            if (image)
            {
                image.sizeDelta = Vector2.Lerp(image.sizeDelta, Vector2.one * imageSize, Time.unscaledDeltaTime * movementSpeed);
            }
            if (canvasGroup)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.unscaledDeltaTime * imageFadeSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!selectable.interactable) return;

            isOn = true;

            if (pointerEnterSFX)
                audioSource.PlayOneShot(pointerEnterSFX);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!selectable.interactable) return;

            isOn = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!selectable.interactable) return;

            if (image)
            {
                image.position = Input.mousePosition;
                image.sizeDelta = Vector2.zero;
            }
            if (canvasGroup)
            {
                canvasGroup.alpha = 1;
            }

            if (buttonDownSFX)
                audioSource.PlayOneShot(buttonDownSFX);
        }

        public void A()
        {
            if (text)
                text.color = Color.Lerp(text.color, highlight, Time.unscaledDeltaTime * fadeSpeed);
        }
        public void B()
        {
            if (text)
                text.color = Color.Lerp(text.color, normal, Time.unscaledDeltaTime * fadeSpeed);
        }
    }
}