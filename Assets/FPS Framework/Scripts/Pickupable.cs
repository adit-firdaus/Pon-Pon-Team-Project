using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPSFramework
{
    public class Pickupable : MonoBehaviour, IInteractable
    {
        [HideInInspector] public KeyCode pickupKey = KeyCode.F;
        [HideInInspector] public string interactionName = "Take";
        [HideInInspector] public string displayName = "Item";
        [HideInInspector] public PickableType type;
        [HideInInspector] public AmmoProfileData ammoProfile;
        [HideInInspector] public int ammoAmount = 30;
        [HideInInspector] public Item item;

        private FirearmAttachmentsManager firearmAttachmentsManager;
        private int m_ammoReserve;
        private bool isDroped;

        private void Start()
        {
            firearmAttachmentsManager = transform.SearchFor<FirearmAttachmentsManager>();
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb) rb.centerOfMass = Vector3.zero;
        }

        public void Take(Inventory inventory)
        {
            StartCoroutine(ConfirmTake(inventory));

            if (inventory.items.ToArray().Length != 0 && inventory.items[inventory.currentItemIndex].Animator)
            {
                inventory.items[inventory.currentItemIndex].Animator.CrossFade("Pickup", 0.1f, 0, 0);
            }
        }

        public void UpdateFirearmValues(Firearm firearm)
        {
            firearmAttachmentsManager = transform.SearchFor<FirearmAttachmentsManager>();

            firearmAttachmentsManager.sight = firearm.attachmentsManager.sight;
            firearmAttachmentsManager.muzzle = firearm.attachmentsManager.muzzle;
            firearmAttachmentsManager.magazine = firearm.attachmentsManager.magazine;
            firearmAttachmentsManager.stock = firearm.attachmentsManager.stock;
            firearmAttachmentsManager.laser = firearm.attachmentsManager.laser;
            m_ammoReserve = firearm.reserve;
            isDroped = true;
        }

        private IEnumerator ConfirmTake(Inventory inventory)
        {
            switch (type)
            {
                case PickableType.Item:

                    Item newItem = null;
                    if (inventory.items.Count < inventory.maxSlots)
                    {
                        newItem = inventory.CreateAndAddItem(item);
                    }
                    else if(inventory.items.Count >= inventory.maxSlots)
                    {
                        Item oldItem = inventory.FindItem(inventory.currentItemIndex);
                        newItem = inventory.ReplaceItem(oldItem, item.GetComponent<Weapon>());
                    }

                    yield return new WaitForEndOfFrame();
                    if(!isDroped) m_ammoReserve = ammoAmount;

                    if (newItem.TryGetComponent<Firearm>(out Firearm firearm))
                    {
                        firearm.reserve = m_ammoReserve;
                        firearm.attachmentsManager.sight = firearmAttachmentsManager.sight;
                        firearm.attachmentsManager.muzzle = firearmAttachmentsManager.muzzle;
                        firearm.attachmentsManager.magazine = firearmAttachmentsManager.magazine;
                        firearm.attachmentsManager.laser = firearmAttachmentsManager.laser;
                        firearm.attachmentsManager.stock = firearmAttachmentsManager.stock;
                    }

                    break;

                case PickableType.Ammo:
                    inventory.AddAmmunition(ammoProfile, ammoAmount);
                    break;
            }

            Destroy(gameObject);
        }

        public void Interact(InteractablesManager source)
        {
            Take(source.Inventory);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetInteractionName()
        {
            string info = "";
            switch(type)
            {
                case PickableType.Item:
                    info = $"{displayName} - {type}";
                    break;

                case PickableType.Ammo:
                    info = $"{displayName} {ammoAmount}X - {type}";
                    break;
            }

            return $"{interactionName} {info}";
        }

        public KeyCode GetInteractionKey()
        {
            return pickupKey;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Pickupable))]
    public class PickupableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Pickupable pickupable = (Pickupable)target;
            EditorGUI.BeginChangeCheck();

            pickupable.pickupKey = (KeyCode)EditorGUILayout.EnumPopup("Pickup Key", pickupable.pickupKey);
            pickupable.type = (PickableType)EditorGUILayout.EnumPopup(new GUIContent("Type", "The type of the item."), pickupable.type);
            pickupable.interactionName = EditorGUILayout.TextField("Interaction Name", pickupable.interactionName);
            pickupable.displayName = EditorGUILayout.TextField("Display Name", pickupable.displayName);

            if (pickupable.type == PickableType.Item)
            {
                pickupable.item = EditorGUILayout.ObjectField(new GUIContent("Item", "The item going to be equiped"), pickupable.item, typeof(Item), true) as Item;
            }
            if(pickupable.type == PickableType.Ammo)
            {
                pickupable.ammoProfile = EditorGUILayout.ObjectField("Ammo Profile", pickupable.ammoProfile, typeof(AmmoProfileData), true) as AmmoProfileData;
            }

            pickupable.ammoAmount = EditorGUILayout.IntField(new GUIContent("Ammo Amount", "The amount of ammo in weapon or the ammo item"), pickupable.ammoAmount);

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
        }
    }

#endif

    public enum PickableType
    {
        Item = 0,
        Ammo = 1,
    }
}