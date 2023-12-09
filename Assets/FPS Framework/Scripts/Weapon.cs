using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    /// <summary>
    /// base class for all weapons
    /// </summary>
    public class Weapon : Item
    {
        /// <summary>
        /// The script which is responsable for weapon pro
        /// </summary>
        public FPSMotion Motion { get; set; }

        /// <summary>
        /// If weapon is not firearm this will be null
        /// </summary>
        public Firearm Firearm { get; set; }

        /// <summary>
        /// If weapon is not throwable this will be null
        /// </summary>
        public Throwable Throwable { get; set; }

        /// <summary>
        /// All active projectile from this weapon
        /// </summary>
        public List<Projectile> Projectiles { get; set; } = new List<Projectile>(1);
  
        public void Setup(Throwable throwable)
        {
            if (!throwable)
            {
                Debug.LogError("Throwable is not setup due to null reference exception");
                return;
            }

            Throwable = throwable;
            Name = "Gernade";
            Replacement = throwable?.replacement;

            Setup();
            
            GetMainComponents();
        }

        protected override void GetMainComponents()
        {
            base.GetMainComponents();
            if (!Motion) Motion = GetComponent<FPSMotion>();
        }
        
        /// <summary>
        /// state of weapon firing mode
        /// </summary>
        public enum FireMode
        {
            Auto = 0,
            SemiAuto = 1,
            Selective = 2
        }

        /// <summary>
        /// unit for fire rate
        /// </summary>
        public enum FireRateUnit
        {
            RoundPerMinute = 60,
            RoundPerSecond = 1
        }

        /// <summary>
        /// weapon bolt type
        /// </summary>
        public enum RechargingType
        {
            GasPowerd = 0,
            Manual = 1
        }

        /// <summary>
        /// Type of reload manual needs animation events in order to reload
        /// </summary>
        public enum ReloadType
        {
            Automatic = 0,
            Manual = 1
        }
    }
}