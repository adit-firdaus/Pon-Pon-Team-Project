using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public interface IOnExplosionHitInParent
    {
        void OnExplosionHitInParent(ExplosionHitInfo hitInfo);
    }
}