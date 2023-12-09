using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FPSFramework
{
    [AddComponentMenu("Weapons/Throwable")]
    public class Throwable : Weapon
    {
        public Pickupable replacement;
        public Explosive grenade;
        public Transform throwPoint;
        public AmmoProfileData ammoProfile;
        public float throwDelay = 0.5f;
        public float heatSpeed = 1;
        public float throwForceMin = 2;
        public float throwForceMax = 10;

        private float throwHeat = 0;

        public event Action OnTrigger;
        public event Action OnItemThrow;

        public WeaponHUD HUD { get; set; }
        public AmmoProfile AmmoProfile { get; set; }

        private void Awake()
        {
            Setup(this);
        }

        private void Start()
        {
            OnTrigger += Trigger;
            AmmoProfile = Inventory.FindAmmunition(ammoProfile);

            if (AmmoProfile == null)
            {
                AmmoProfile = new AmmoProfile();
                AmmoProfile.data = ScriptableObject.CreateInstance<AmmoProfileData>();

                AmmoProfile.data.Name = "No Ammo Data";
                AmmoProfile.amount = 5;

                Debug.LogWarning($"Can't find ammo profile in the inventory. Throwable will use clone profile version.");
            }
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && replacement) Drop(Vector3.down * Inventory.dropForce, Vector3.up * Inventory.dropForce * 3);

            if (AmmoProfile.amount <= 0) return;

            if (Input.GetKeyDown(KeyCode.Mouse0) && !IsTriggering())
            {
                OnTrigger?.Invoke();
                throwHeat = 0;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                throwHeat += Time.deltaTime * heatSpeed;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0)) StartCoroutine(StartThrow());
        }

        private void Trigger()
        {
            Animator.CrossFade("Trigger", 0.15f, 0, 0);
        }

        public bool IsTriggering()
        {
            return Animator ? Animator.GetCurrentAnimatorStateInfo(0).IsName("Trigger") : false;
        }

        public IEnumerator StartThrow()
        {
            yield return new WaitForSeconds(throwDelay);

            Animator.SetTrigger("Throw");
        }

        public void PerformThrow()
        {
            AmmoProfile.amount--;
            float throwForce = Mathf.Lerp(throwForceMin, throwForceMax, throwHeat);
            Explosive newExplosive = Instantiate(grenade, throwPoint.position, throwPoint.rotation);
           
            newExplosive.GetComponent<Rigidbody>().AddForce((transform.forward * throwForce) + (transform.up * throwForce / 10), ForceMode.VelocityChange);
            newExplosive.GetComponent<Rigidbody>().AddTorque((transform.forward * throwForce) + (transform.up * throwForce / 10) * 10 * UnityEngine.Random.Range(0.5f, 1), ForceMode.VelocityChange);
            newExplosive.source = Inventory?.Controller?.Actor;

            OnItemThrow?.Invoke();
        }

        private void OnDestroy()
        {
            OnTrigger -= Trigger;
        }
    }
}