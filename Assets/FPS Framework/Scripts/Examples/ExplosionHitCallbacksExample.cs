using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class ExplosionHitCallbacksExample : MonoBehaviour, IOnExplosionHit, IOnExplosionHitInChildren, IOnExplosionHitInParent
    {
        public void OnExplosionHit(ExplosionHitInfo info)
        {
            print("hit");
        }

        public void OnExplosionHitInChildren(ExplosionHitInfo hitInfo)
        {
            print("hitInChildren");
        }

        public void OnExplosionHitInParent(ExplosionHitInfo hitInfo)
        {
            print("hitInParent");
        }
    }
}