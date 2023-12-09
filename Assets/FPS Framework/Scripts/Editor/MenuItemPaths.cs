using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class MenuItemPaths
    {
        private const string Main = "GameObject/";
        private const string Create = "FPS Engine/";
        private const string FPSEngine = "FPS Engine/";

        public const string Help = FPSEngine + "Help";

        public const string CreateFPSController = Main + Create + "FPS Controller";
        public const string CreateWeaponMotion = Main + Create + "FPS Motion";

        public const string CreateFirearm = Main + Create + "Firearm/Firearm";

        public const string CreateAttachment = Main + Create + "Firearm/Attachment";
        public const string CreateAttachmentSight = Main + Create + "Firearm/Attachment/Sight";
        public const string CreateAttachmentMuzzle = Main + Create + "Firearm/Attachment/Muzzle";
    }
}