using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class Pinger : MonoBehaviour
    {
        public LayerMask pingableLayers = -1;
        public KeyCode pingKey = KeyCode.F1;
        public Ping ping;
        public Canvas canvas;
        public float range = 100;

        private void Update()
        {
            if (Input.GetKeyDown(pingKey))
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, pingableLayers))
                {
                    Ping newPing = Instantiate(ping, canvas.transform);
                    OnPinged(newPing);
                    newPing.GetComponent<FloatingRect>().position = hit.point;

                    if(ping.soundEffect)
                    PlayPingSoundEffect(newPing);
                }
            }
        }

        public virtual void OnPinged(Ping ping)
        {

        }

        public virtual void PlayPingSoundEffect(Ping ping)
        {
            AudioManager.PlayOneShot(ping.soundEffect);
        }
    }
}
