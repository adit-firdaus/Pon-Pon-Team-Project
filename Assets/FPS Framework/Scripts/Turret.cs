using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class Turret : MonoBehaviour, IOnHit
    {
        public GameObject target;
        public Transform muzzle;
        public Firearm firearm;
        public float lookSpeed = 50;

        Vector3 enemyPos;

        bool greenLight;

        private void Start()
        {
            Invoke(nameof(GiveGreenLight), Random.Range(1, 10));
        }

        private void GiveGreenLight()
        {
            greenLight = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!greenLight) return;
            if (target == null && FindObjectOfType<CharacterController>()) target = FindObjectOfType<CharacterController>().gameObject;

            if (target != null)
            {
                enemyPos = Vector3.Lerp(enemyPos, target.transform.position, Time.deltaTime * lookSpeed);
                if (muzzle) muzzle.LookAt(enemyPos);
                if (firearm) firearm.Fire();
            }
        }

        public void OnHit(HitInfo info)
        {
            GiveGreenLight();
        }
    }
}