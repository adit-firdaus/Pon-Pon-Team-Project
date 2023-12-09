using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.Examples
{
    public class FirearmHitCallbacksExample : MonoBehaviour, IOnHit, IOnHitInChildren, IOnHitInParent
    {
        public void OnHit(HitInfo info)
        {
            print("hit");
        }

        public void IOnHitInChildren(HitInfo hitInfo)
        {
            print("hitInChildren");
        }

        public void OnHitInParent(HitInfo hitInfo)
        {
            print("hitInParent");
        }
    }
}