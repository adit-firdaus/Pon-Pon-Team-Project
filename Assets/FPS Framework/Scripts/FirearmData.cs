using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPSFramework
{
    [CreateAssetMenu(fileName = "New Firearm", menuName = "Weapons/Firearm")]
    public class FirearmData : ScriptableObject
    {
        #region Variables

        //base
        public string Name;
        public Pickupable replacement;
        public WeaponHUD canves;
        public Crosshair crosshair;

        //fire
        public Weapon.RechargingType rechargingType;
        public Weapon.FireMode fireMode = Weapon.FireMode.Auto;
        public Weapon.FireRateUnit fireRateUnit = Weapon.FireRateUnit.RoundPerMinute;
        public Vector3Direction casingDirection = Vector3Direction.right;
        public Projectile projectile;
        public GameObject casing;
        public float fireRate = 833;
        public float muzzleVelocity = 250;
        public float ejectionVelocity = 10;
        public float impactForce = 10;
        public float damage = 20;
        public float range = 300;


        public SprayPattern sprayPattern;
        public SprayPattern aimSprayPattern;

        public float fireTransition = 0.1f;
        public string rechargingStateName = "Recharging";
        public bool allowFireWhileRecharging;
        public float casingRotationFactor = 25;
        public float lowerdDelay = 10;

        public bool tracerRounds = true;
        public float projectileSize = 40;

        public float decalSize = 1;

        public float afterShotReliefTime = 0.2f;
        public int shotCount = 1;
        public float shotDelay = 0;
        public bool alwaysApplyFire = false;

        //ammo
        public AmmoProfileData ammoType;
        public int magazineCapacity = 30;
        public int reserve = 0;
        public bool automaticReload = true;
        public bool reloadOnAwake = false;

        public Weapon.ReloadType reloadType;
        public bool flexibleReloadTime;
        public float reloadTime = 1.6f;
        public float emptyReloadTime = 2.13f;

        //recoil
        public float recoilPositionRoughness = 10f;
        public float recoilRotationRoughness = 10f;
        public float horizontalRecoil = 0.7f;
        public float verticalRecoil = 0.1f;
        public float cameraRecoil = 1;
        public Vector3 RecoilRotation = new Vector3(-15f, 25, 15);
        public Vector3 RecoilKickBack = new Vector3(0.01f, 0.01f, -0.3f);

        public Vector3 RecoilRotation_Aim = new Vector3(-1f, 3f, 3f);
        public Vector3 RecoilKickBack_Aim = new Vector3(-0.01f, 0.01f, 0.2f);

        //aim down sight
        public float aimSpeed = 10;
        public float aimFieldOfView = 50;
        public float aimWeaponFieldOfview = 40;


        //movement
        public float Movement_WalkSpeed = 5;
        public float Movement_RunSpeed = 10;
        public float Movement_TacticalSprintSpeed = 13f;
        public float Movement_AimWalkSpeed = 3;
        public float Movement_FireWalkSpeed = 4;
        public bool allowFullMovementSpeedWhileAiming;


        //Audio
        public AudioProfile fire;
        public AudioProfile fireLoop;
        public AudioProfile fireTail;
        #endregion

        public Vector3 staticRecoilRotation = new Vector3(-1, 2, -10);
        public float cameraShake = 0.1f;
        public float cameraShakeFadeOutTime = 0.1f;
        public AudioProfile reloadAudio;
        public AudioProfile reloadEmptyAudio;

        public bool Use_Fire;
        public bool Use_Recoil;
        public bool Use_Audio;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FirearmData))]
    public class FirearmDataEditor : Editor
    {
        private string fireRateUnitLabel;
        private static bool Foldout_Fire;
        private static bool Foldout_Recoil;
        private static bool Foldout_Audio;

        public override void OnInspectorGUI()
        {
            FirearmData weapon = (FirearmData)target;


            UpdateBase(weapon);
            EditorGUILayout.Space();

            UpdateFire(weapon);
            UpdateRecoil(weapon);
            UpdateAudio(weapon);
        }

        private void UpdateBase(FirearmData weapon)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Base", EditorStyles.boldLabel);

            weapon.Name = EditorGUILayout.TextField(new GUIContent("Name", "The name of the weapone."), weapon.Name);
            weapon.replacement = EditorGUILayout.ObjectField(new GUIContent("Replacement", "The pickuable item which is going to be thrown when replacing weapon with another or throwing it."), weapon.replacement, typeof(Pickupable), true) as Pickupable;
            weapon.canves = EditorGUILayout.ObjectField(new GUIContent("Canves", "The object which contains all the UI elments of the weapon this sould be a prefap."), weapon.canves, typeof(WeaponHUD), true) as WeaponHUD;
            weapon.crosshair = EditorGUILayout.ObjectField(new GUIContent("Crosshair", "The used crosshair for this weapon."), weapon.crosshair, typeof(Crosshair), true) as Crosshair;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(weapon, "weapon data modified");
                EditorUtility.SetDirty(weapon);
            }
        }

        private void UpdateFire(FirearmData weapon)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal("box");
            weapon.Use_Fire = EditorGUILayout.Toggle(weapon.Use_Fire, GUILayout.MaxWidth(28));
            Foldout_Fire = EditorGUILayout.Foldout(Foldout_Fire, "Fire", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Fire) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!weapon.Use_Fire);

            weapon.fireMode = (Weapon.FireMode)EditorGUILayout.EnumPopup(new GUIContent(" Mode", "What the type of fire you want Auto or Semi-Auto"), weapon.fireMode);
            weapon.rechargingType = (Weapon.RechargingType)EditorGUILayout.EnumPopup(new GUIContent(" Bolt Type", "If set to Manual shells will be thrown via animation event and weapon won't shoot if playing the rechaing animation this is going to be used in bolt action weapons."), weapon.rechargingType);
            weapon.fireRateUnit = (Weapon.FireRateUnit)EditorGUILayout.EnumPopup(new GUIContent(" Fire Rate Unit", "How fire rate will be counted as Round per minite or Round per second."), weapon.fireRateUnit);
            weapon.casingDirection = (Vector3Direction)EditorGUILayout.EnumPopup(new GUIContent(" Casing Direction", "Which direction should the casing shell go."), weapon.casingDirection);
            weapon.projectile = EditorGUILayout.ObjectField(new GUIContent(" Projectile", "The projectile which going to be fired."), weapon.projectile, typeof(Projectile), false) as Projectile;

            if (!weapon.projectile)
            {
                EditorGUILayout.HelpBox("Projectile must be assgined in order to use this section.", MessageType.Error);
                EditorGUILayout.Space();
            }

            if (weapon.projectile)
            {
                weapon.casing = EditorGUILayout.ObjectField(new GUIContent(" Casing", "Most weapons ejects a casing after each shot however this is optinal"), weapon.casing, typeof(GameObject), false) as GameObject;

                EditorGUILayout.BeginHorizontal();
                weapon.fireRate = EditorGUILayout.FloatField(new GUIContent(" Fire Rate", "how many round will be fired depening on the unit"), weapon.fireRate);

                if (weapon.fireRateUnit == Weapon.FireRateUnit.RoundPerMinute)
                    fireRateUnitLabel = "RPM";
                if (weapon.fireRateUnit == Weapon.FireRateUnit.RoundPerSecond)
                    fireRateUnitLabel = "RPS";

                EditorGUILayout.LabelField(fireRateUnitLabel, GUILayout.MaxWidth(33));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                weapon.muzzleVelocity = EditorGUILayout.FloatField(new GUIContent(" Muzzle Velocity", "the speed of the projectile."), weapon.muzzleVelocity);
                EditorGUILayout.LabelField("M/S", GUILayout.MaxWidth(33));
                EditorGUILayout.EndHorizontal();

                if (weapon.casing != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    weapon.ejectionVelocity = EditorGUILayout.FloatField(new GUIContent(" Casing Ejection Velocity", "the speed of the shell when fired."), weapon.ejectionVelocity);
                    EditorGUILayout.LabelField("M/S", GUILayout.MaxWidth(33));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                if (weapon.casing != null)
                {
                    weapon.casingRotationFactor = EditorGUILayout.FloatField(new GUIContent(" Casing Rotation", "The amount of random rotaion for the shells."), weapon.casingRotationFactor);
                }

                weapon.impactForce = EditorGUILayout.FloatField(new GUIContent(" Impact Force", "The force applied on impact with any rigidbody"), weapon.impactForce);

                weapon.damage = EditorGUILayout.FloatField(new GUIContent(" Damage", "Amount of damage this weapon can do to any object with Health System script."), weapon.damage);
                weapon.range = EditorGUILayout.FloatField(new GUIContent(" Range", "Range of the firearm which effects damage."), weapon.range);


                EditorGUILayout.Space();
                weapon.sprayPattern = EditorGUILayout.ObjectField(" Spray Pattern", weapon.sprayPattern, typeof(SprayPattern), true) as SprayPattern;
                weapon.aimSprayPattern = EditorGUILayout.ObjectField(" Aim Spray Pattern", weapon.aimSprayPattern, typeof(SprayPattern), true) as SprayPattern;
                EditorGUI.EndDisabledGroup();


                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Other Settings", EditorStyles.boldLabel);
                weapon.fireTransition = EditorGUILayout.FloatField(new GUIContent(" Fire Transition", "The time needed to tranition bettwen current animation to fire animation."), weapon.fireTransition);

                if (weapon.rechargingType == Weapon.RechargingType.Manual)
                {
                    weapon.rechargingStateName = EditorGUILayout.TextField(new GUIContent(" Recharging State Name", "the bold action animation state name used in bolt action weapons."), weapon.rechargingStateName);
                    weapon.allowFireWhileRecharging = EditorGUILayout.Toggle(new GUIContent(" Allow Fire While Recharging", "Is it okay to fire when charing the bolt."), weapon.allowFireWhileRecharging);
                }

                weapon.tracerRounds = EditorGUILayout.Toggle(new GUIContent(" Tracer Rounds", "Does the projectile scale depening on the distance from player this is used to make MW like projectiles."), weapon.tracerRounds);
                weapon.projectileSize = EditorGUILayout.FloatField(new GUIContent(" Projectile Size", "The scale of the projecile."), weapon.projectileSize);
                weapon.decalSize = EditorGUILayout.FloatField(new GUIContent(" Decal Size", "the scale of the decals on impact."), weapon.decalSize);
                weapon.afterShotReliefTime = EditorGUILayout.FloatField(new GUIContent(" After Shot Relief Time", "The time that you can't sprint after you fire."), weapon.afterShotReliefTime);
                weapon.shotCount = EditorGUILayout.IntField(new GUIContent(" Shot Count", "How many projectiles will be fired at once."), weapon.shotCount);

                if (weapon.shotCount > 1)
                {

                    EditorGUILayout.BeginHorizontal();
                    weapon.shotDelay = EditorGUILayout.FloatField(new GUIContent(" Shots Delay", "the delay bettwen every shot ignoring of the first one."), weapon.shotDelay);

                    if (GUILayout.Button(new GUIContent(" Calculate", "Calculates shot delay from fire rate")))
                    {
                        if (weapon.fireRateUnit == Weapon.FireRateUnit.RoundPerMinute)
                        {
                            weapon.shotDelay = 1 / (weapon.fireRate / 60);
                        }

                        if (weapon.fireRateUnit == Weapon.FireRateUnit.RoundPerSecond)
                        {
                            weapon.shotDelay = 1 / weapon.fireRate;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    weapon.alwaysApplyFire = EditorGUILayout.Toggle(new GUIContent(" Always Apply Fire", "Always play fire sound and throw a shell."), weapon.alwaysApplyFire);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Ammunition", EditorStyles.boldLabel);

                weapon.ammoType = EditorGUILayout.ObjectField("Ammo Profile", weapon.ammoType, typeof(AmmoProfileData), true) as AmmoProfileData;

                weapon.magazineCapacity = EditorGUILayout.IntField(new GUIContent(" Magazine Capacity", "How many projectiles can the mag have at once."), weapon.magazineCapacity);
                weapon.reserve = EditorGUILayout.IntField(new GUIContent(" Reserve", "The amount of ammo in the mag at the moment."), weapon.reserve);

                weapon.automaticReload = EditorGUILayout.Toggle(" Always Reload", weapon.automaticReload);
                weapon.reloadOnAwake = EditorGUILayout.Toggle(" Reload On Awake", weapon.reloadOnAwake);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(" Reload", EditorStyles.boldLabel);

                weapon.reloadType = (Weapon.ReloadType)EditorGUILayout.EnumPopup(new GUIContent(" Type", "Type of reload, handling manaul needs animation events."), weapon.reloadType);
                weapon.reloadTime = EditorGUILayout.FloatField(new GUIContent(" Reload Time", "Time needed to reload."), weapon.reloadTime);
                if (weapon.reloadType == Weapon.ReloadType.Manual)
                {
                    weapon.flexibleReloadTime = EditorGUILayout.ToggleLeft(new GUIContent(" Flexible Reload Time", "is the reload time will depend on ammo reserve"), weapon.flexibleReloadTime);
                }

                if (weapon.reloadType == Weapon.ReloadType.Automatic)
                {

                    weapon.emptyReloadTime = EditorGUILayout.FloatField(new GUIContent(" Empty Reload Time", "Time needed to reload empty mag."), weapon.emptyReloadTime);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(weapon, "weapon data modified");
                EditorUtility.SetDirty(weapon);
            }
        }

        private void UpdateRecoil(FirearmData weapon)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal("box");
            weapon.Use_Recoil = EditorGUILayout.Toggle(weapon.Use_Recoil, GUILayout.MaxWidth(28));
            Foldout_Recoil = EditorGUILayout.Foldout(Foldout_Recoil, "Recoil", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Recoil) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!weapon.Use_Recoil);

            EditorGUILayout.LabelField(" Camera", EditorStyles.boldLabel);
            weapon.cameraRecoil = EditorGUILayout.FloatField(" Recoil", weapon.cameraRecoil);
            weapon.cameraShake = EditorGUILayout.FloatField(" Shake", weapon.cameraShake);
            weapon.cameraShakeFadeOutTime = EditorGUILayout.FloatField(" Fadeout Time", weapon.cameraShakeFadeOutTime);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" Camera Recoil", EditorStyles.boldLabel);
            weapon.horizontalRecoil = EditorGUILayout.FloatField(" Horizontal Recoil", weapon.horizontalRecoil);
            weapon.verticalRecoil = EditorGUILayout.FloatField(" Vertical Recoil", weapon.verticalRecoil);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" Weapon Recoil", EditorStyles.boldLabel);
            weapon.recoilPositionRoughness = EditorGUILayout.FloatField(" Position Damp Time", weapon.recoilPositionRoughness);
            weapon.recoilRotationRoughness = EditorGUILayout.FloatField(" Rotation Damp Time", weapon.recoilRotationRoughness);

            weapon.staticRecoilRotation = EditorGUILayout.Vector3Field(" Static Recoil Rotation", weapon.staticRecoilRotation);
            weapon.RecoilKickBack = EditorGUILayout.Vector3Field(" Kick Back", weapon.RecoilKickBack);
            weapon.RecoilRotation = EditorGUILayout.Vector3Field(" Rotation", weapon.RecoilRotation);
            weapon.RecoilKickBack_Aim = EditorGUILayout.Vector3Field(" KickBack Aim", weapon.RecoilKickBack_Aim);
            weapon.RecoilRotation_Aim = EditorGUILayout.Vector3Field(" Rotation Aim", weapon.RecoilRotation_Aim);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(weapon, "weapon data modified");
                EditorUtility.SetDirty(weapon);
            }
        }

        private void UpdateAudio(FirearmData weapon)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal("box");
            weapon.Use_Audio = EditorGUILayout.Toggle(weapon.Use_Audio, GUILayout.MaxWidth(28));
            Foldout_Audio = EditorGUILayout.Foldout(Foldout_Audio, "Audio", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Audio) return;
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(!weapon.Use_Audio);

            weapon.fire = EditorGUILayout.ObjectField(new GUIContent(" Fire", "The sound which going to play when firing."), weapon.fire, typeof(AudioProfile), true) as AudioProfile;

            if (!weapon.fire)
            {
                EditorGUILayout.HelpBox("Fire can't be null.", MessageType.Error);
                EditorGUILayout.Space();
            }

            weapon.fireTail = EditorGUILayout.ObjectField(new GUIContent(" Fire Tail", "The sound which going to play when firing."), weapon.fireTail, typeof(AudioProfile), true) as AudioProfile;

            if (!weapon.fireTail)
            {
                EditorGUILayout.HelpBox("Fire trail can't be null.", MessageType.Error);
                EditorGUILayout.Space();
            }

            weapon.reloadAudio = EditorGUILayout.ObjectField(new GUIContent(" Reload", "The sound which going to play when reloading with ammo in the mag."), weapon.reloadAudio, typeof(AudioProfile), true) as AudioProfile;

            if (!weapon.reloadAudio)
            {
                EditorGUILayout.HelpBox("Reload can't be null.", MessageType.Error);
                EditorGUILayout.Space();
            }

            if (weapon.reloadType == Weapon.ReloadType.Automatic)
            {
                weapon.reloadEmptyAudio = EditorGUILayout.ObjectField(new GUIContent(" Reload Empty", "The sound which going to play when reloading empty mag."), weapon.reloadEmptyAudio, typeof(AudioProfile), true) as AudioProfile;

                if (!weapon.reloadEmptyAudio)
                {
                    EditorGUILayout.HelpBox("Reload Empty can't be null.", MessageType.Error);
                    EditorGUILayout.Space();
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(weapon, "weapon data modified");
                EditorUtility.SetDirty(weapon);
            }
        }
    }
#endif
}