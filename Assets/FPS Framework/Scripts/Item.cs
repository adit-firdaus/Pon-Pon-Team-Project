using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FPSFramework
{
    public class Item : MonoBehaviour
    {
        /// <summary>
        /// name of this item
        /// </summary>
        public string Name { get; protected set; }

        public Inventory Inventory { get; protected set; }

        public Animator Animator { get; protected set; }

        public Pickupable Replacement { get; protected set; }

        public Actor Actor { get => actor; set => actor = value; }

        private Actor actor;

        public ICharacterController Controller { get => Inventory?.Controller; }

        /// <summary>
        /// holder for all sounds
        /// </summary>
        public GameObject AudioHolder { get; set; }

        public Action onDroped { get; set; }

        protected virtual void Setup()
        {
            AudioHolder = new GameObject($"Audio Holder ({Name})");
        }

        protected virtual void GetMainComponents()
        {
            Inventory = GetComponentInParent<Inventory>();
            Animator = GetComponentInChildren<Animator>();

            if(Inventory)
            actor = Inventory.Actor;
            else
            {
                actor = transform.SearchFor<Actor>();
            }
        }

        /// <summary>
        /// Drops the item on ground
        /// </summary>
        public void Drop(Vector3 force, Vector3 torque, Firearm firearm = null)
        {
            Inventory.Controller.GetCameraManager().ResetFieldOfView(10);
            Inventory.RemoveItem(this);
            Pickupable newPickupable = Instantiate(Replacement, Inventory.dropLocation.position, Inventory.transform.rotation);
            newPickupable.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
            newPickupable.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.VelocityChange);

            if (Inventory.items.Count > 0)
            {
                Inventory.Switch(Inventory.items.Count - 1);
            }

            if (firearm) newPickupable.UpdateFirearmValues(firearm);

            OnDroped();
            onDroped?.Invoke();

            Destroy(gameObject);
        }

        protected virtual void OnDroped()
        {

        }
    }
}