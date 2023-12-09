using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    /// <summary>
    /// base class for collision in explosions
    /// </summary>
    public class ExplosionHitInfo : MonoBehaviour
    {
        /// <summary>
        /// The explosive which did hit the collider
        /// </summary>
        public Explosive explosive { get; set; }

        /// <summary>
        /// The raycast hit of the Projectile or Hitscan(Soon)
        /// </summary>
        public RaycastHit raycastHit { get; set; }

        /// <summary>
        /// The amount of damage damagable has received
        /// </summary>
        public float damageReceived { get; set; }

        public ExplosionHitInfo()
        {

        }

        public ExplosionHitInfo(Explosive explosive)
        {
            this.explosive = explosive;
        }
    }
}