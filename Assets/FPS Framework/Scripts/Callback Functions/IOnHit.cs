using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public interface IOnHit
    {
        void OnHit(HitInfo info);
    }
}