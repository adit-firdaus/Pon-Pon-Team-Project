using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public interface IOnExplosionHitInChildren
    {
        void OnExplosionHitInChildren(ExplosionHitInfo hitInfo);
    }
}