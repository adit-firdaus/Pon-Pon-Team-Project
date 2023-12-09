using UnityEngine;
using TMPro;

namespace FPSFramework
{
    public class PickupManager : MonoBehaviour
    {
        public LayerMask layerMask;
        public float range = 3;
        public float fadeSpeed = 30;
        public CanvasGroup pickupUI;
        public TextMeshProUGUI label;

        private Pickupable item;
        public Inventory Inventory { get; set; }


        private void Start()
        {
            Inventory = GetComponent<Inventory>();

            if (pickupUI) pickupUI.alpha = 0;
        }

        private void Update()
        {
            if (item)
            {
                if (pickupUI && pickupUI.alpha != 1) pickupUI.alpha = Mathf.Lerp(pickupUI.alpha, 1, Time.deltaTime * fadeSpeed);

                string ammoAmount = item.type == PickableType.Ammo ? $"{item.ammoAmount}X" : "";
                if (label) label.text = $"{item.type} - {item.item.Name}" + ammoAmount;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (item.type == PickableType.Item)
                    {
                        if (!Inventory.IsFullSlots())
                        {
                            item.Take(Inventory);

                            int weaponIndex = Inventory.items.ToArray().Length;

                            Inventory.Switch(weaponIndex);
                        }
                        else
                        {
                            item.Take(Inventory);
                            Inventory.FindItem(Inventory.items[Inventory.currentItemIndex].Name).Drop(Vector3.down * Inventory.dropForce, Vector3.up * Inventory.dropForce * 3);
                        }
                    }


                    if (item.type == PickableType.Ammo)
                    {
                        item.Take(Inventory);
                    }
                }
            }
            else
            {
                if (pickupUI && pickupUI.alpha != 0) pickupUI.alpha = Mathf.Lerp(pickupUI.alpha, 0, Time.deltaTime * fadeSpeed);
            }

            RaycastHit nearby;
            item = Physics.Raycast(transform.position, transform.forward, out nearby, range, layerMask) ?
            nearby.transform.GetComponent<Pickupable>() : null;
        }
    }
}