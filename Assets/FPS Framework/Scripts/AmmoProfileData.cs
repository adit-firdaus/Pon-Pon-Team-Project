using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    [CreateAssetMenu(fileName = "New Ammo Data", menuName = "Weapons/Ammo")]
    public class AmmoProfileData : ScriptableObject
    {
        public string Name;
    }
}