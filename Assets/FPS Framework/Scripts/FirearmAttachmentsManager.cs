using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class FirearmAttachmentsManager : MonoBehaviour
    {

        [Header("Attachments")]
        public string sight;
        public string magazine;
        public string stock;
        public string muzzle;
        public string laser;

        public Firearm targetFirearm { get; protected set; }

        public int GetAttachmentsCount()
        {
            int count = 0;

            List<string> names = new List<string>();

            names.Add(sight);
            names.Add(muzzle);
            names.Add(magazine);
            names.Add(laser);

            foreach (string s in names)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    count++;
                }
            }

            return count;
        }

        public float damage
        {
            get
            {
                float result = 1;

                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].Damage();
                }

                return result;
            }
        }

        public float spread
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].Spread();
                }

                return result;
            }
        }

        public float fireRate
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].FireRate();
                }

                return result;
            }
        }

        public float range
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].Range();
                }

                return result;
            }
        }

        public float muzzleVelocity
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].MuzzleVelocity();
                }

                return result;
            }
        }

        public float recoil
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].Recoil();
                }

                return result;
            }
        }

        public float visualRecoil
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].VisualRecoil();
                }

                return result;
            }
        }

        public float aimSpeed
        {
            get
            {
                float result = 1;
                if (behaviors == null || behaviors.Length <= 0) return result;

                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i].IsUsed())
                        result *= behaviors[i].AimSpeed();
                }

                return result;
            }
        }

        private AttachementBehavior[] behaviors;

        private void Start()
        {
            targetFirearm = GetComponent<Firearm>();
            behaviors = GetComponentsInChildren<AttachementBehavior>();
        }

        private void OnEnable()
        {
            AttachmentSwitching.OnSightSwitch += SwitchSight;
            AttachmentSwitching.OnMuzzleSwitch += SwitchMuzzle;
            AttachmentSwitching.OnMagazineSwitch += SwitchMagazine;
            AttachmentSwitching.OnStockSwitch += SwitchStock;
            AttachmentSwitching.OnLaserSwitch += SwitchLaser;
        }

        private void OnDisable()
        {
            AttachmentSwitching.OnSightSwitch -= SwitchSight;
            AttachmentSwitching.OnMuzzleSwitch -= SwitchMuzzle;
            AttachmentSwitching.OnMagazineSwitch -= SwitchMagazine;
            AttachmentSwitching.OnStockSwitch -= SwitchStock;
            AttachmentSwitching.OnLaserSwitch -= SwitchLaser;
        }

        public void SaveAttachments()
        {
            if (!targetFirearm || !targetFirearm.preset) return;
            PlayerPrefs.SetString($"{targetFirearm.preset.Name}.sight", sight);
            PlayerPrefs.SetString($"{targetFirearm.preset.Name}.muzzle", muzzle);
            PlayerPrefs.SetString($"{targetFirearm.preset.Name}.magazine", magazine);
            PlayerPrefs.SetString($"{targetFirearm.preset.Name}.laser", laser);
        }

        public void LoadAttachments()
        {
            if (!targetFirearm || !targetFirearm.preset) return;
            sight = PlayerPrefs.GetString($"{targetFirearm.preset.Name}.sight");
            muzzle = PlayerPrefs.GetString($"{targetFirearm.preset.Name}.muzzle");
            magazine = PlayerPrefs.GetString($"{targetFirearm.preset.Name}.magazine");
            laser = PlayerPrefs.GetString($"{targetFirearm.preset.Name}.laser");
        }

        public void DeleteSavedAttachments()
        {
            if (!targetFirearm || !targetFirearm.preset) return;
            PlayerPrefs.DeleteKey($"{targetFirearm.preset.Name}.sight");
            PlayerPrefs.DeleteKey($"{targetFirearm.preset.Name}.muzzle");
            PlayerPrefs.DeleteKey($"{targetFirearm.preset.Name}.magazine");
            PlayerPrefs.DeleteKey($"{targetFirearm.preset.Name}.laser");
        }

        [ContextMenu("Reset Attachments")]
        public void ResetAttachments()
        {
            sight = null;
            muzzle = null;
            laser = null;
            magazine = null;
            DeleteSavedAttachments();
        }

        private void SwitchSight() => sight = AttachmentSwitching.Instance.sight;
        private void SwitchMuzzle() => muzzle = AttachmentSwitching.Instance.muzzle;
        private void SwitchMagazine() => magazine = AttachmentSwitching.Instance.magazine;
        private void SwitchStock() => stock = AttachmentSwitching.Instance.stock;
        private void SwitchLaser() => laser = AttachmentSwitching.Instance.laser;
    }
}