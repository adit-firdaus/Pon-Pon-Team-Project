using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FPSFramework
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] UIManager HUD;
        [SerializeField] DeathCamera deathCamera;


        public UIManager UIManager { get; set; }
        public DeathCamera DeathCamera { get; set; }

        private void Awake()
        {
            UIManager = Instantiate(HUD);
            DeathCamera = Instantiate(deathCamera);
        }
    }
}