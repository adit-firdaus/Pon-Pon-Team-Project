using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    /// <summary>
    /// base class for collision in (Projectile or Hitscan)
    /// </summary>
    public class HitInfo
    {
        /// <summary>
        /// The projectile which did hit the collider
        /// </summary>
        public Projectile projectile { get; }

        /// <summary>
        /// The raycast hit of the Projectile or Hitscan(Soon)
        /// </summary>
        public RaycastHit raycastHit { get; }

        public float damageReceived { get; set; }

        public HitInfo()
        {

        }

        public HitInfo(Projectile projectile, RaycastHit hit)
        {
            this.projectile = projectile;
            this.raycastHit = hit;
        }
    }
}