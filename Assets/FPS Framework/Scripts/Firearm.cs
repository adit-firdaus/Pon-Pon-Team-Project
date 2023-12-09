using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FPSFramework
{
    [AddComponentMenu("Weapons/Firearm")]
    [RequireComponent(typeof(FirearmAttachmentsManager))]
    public class Firearm : Weapon
    {
        [Header("Base")]
        public FirearmData preset;
        public Transform _muzzle;
        public Transform casingEjectionPort;
        public Transform RecoilTranform;
        public ParticleSystem rechargingVFX;
        [SerializeField, Tooltip("If true firearm will show it's own HUD")] bool m_HUD = true;

        [Space]
        public FirearmEvents events;

        #region Defaults
        private Vector3 defaultRecoilPosition;
        private Vector3 CurrentRecoilRotation;
        private Vector3 CurrentRecoil2;
        private Vector3 CurrentRecoilPosition;
        private Vector3 CurrentRecoil4;
        private Vector3 RecoilRotationOutput;
        #endregion

        /// <summary>
        /// used ammo type
        /// </summary>
        public AmmoProfile AmmoProfile { get; set; }
        public WeaponHUD HUD { get; set; }
        public FirearmAttachmentsManager attachmentsManager { get; protected set; }

        /// <summary>
        /// all weapon effects
        /// </summary>
        public ParticleSystem[] VFX { get; set; }
        /// <summary>
        /// current fire modes if using selective
        /// </summary>
        public FireMode currentSelectiveFireMode { get; set; }

        private bool IsFiring;
        private bool IsReloading;
        private float fireTimer;


        private int shots;
        [HideInInspector] public int reserve;
        [HideInInspector] public int magazineCapacity;

        public AudioProfile currentFireSFX { get; set; }
        private float currentSpray;
        private float TacticalSprintAmount;
        private Vector3 leanOffset;

        private SprayPattern sprayPattern;
        private SprayPattern aimSprayPattern;

        private Transform farFireLocation;

        private GameObject projectilesParent;
        private GameObject casingsParent;
        private FirearmInputManager inputManager;

        public float GetReservePercentage()
        {
            return (float)reserve / magazineCapacity;
        }

        public Projectile CreateProjectile(Projectile projectile, Firearm source, Transform muzzle, Vector3 direction, float speed, float range, Transform parent = null)
        {
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation, parent);
            newProjectile.direction = direction;

            if (Inventory?.Controller?.GetCharacterController())
                newProjectile.shooterVelocity = Inventory.Controller.GetCharacterController().velocity;

            float muzzleModifier = source?.attachmentsManager != null ? source.attachmentsManager.muzzleVelocity : 1;

            newProjectile.speed = speed * muzzleModifier;
            newProjectile.source = source;
            newProjectile.range = range * source.attachmentsManager.range;

            Projectiles?.Add(newProjectile);

            return newProjectile;
        }

        protected override void Setup()
        {
            attachmentsManager = GetComponent<FirearmAttachmentsManager>();
            inputManager = GetComponent<FirearmInputManager>();

            projectilesParent = GameObject.Find("Projectiles");
            casingsParent = GameObject.Find("Casings");

            if (!projectilesParent) projectilesParent = new GameObject("Projectiles");
            if (!casingsParent) casingsParent = new GameObject("Casings");

            if (!preset)
            {
                Debug.LogError("Firearm is not setup due to null reference exception");
                return;
            }

            //Initialize
            Firearm = this;
            Name = preset?.Name;
            Replacement = preset?.replacement;

            if (_muzzle)
            {
                farFireLocation = _muzzle.CreateChild("Far Fire Location");
                farFireLocation.position = _muzzle.forward * 1500;
            }

            base.Setup();

            //find components
            GetMainComponents();

            VFX = GetComponentsInChildren<ParticleSystem>();

            if (Motion)
                Motion.firearm = this;

            //Initialize HUD
            if (m_HUD && preset.canves)
            {
                HUD = Instantiate(preset.canves, transform);
                HUD.target = this;
                HUD._ammoCount = true;
                HUD._gernadesCount = true;
                HUD._ammoName = true;
                HUD.defaultCrosshair = preset.crosshair;
            }
        }

        /// <summary>
        /// Resets all values of the weapon
        /// </summary>
        public void ResetStates()
        {
            if (Motion)
            {
                Motion.isAiming = false;
            }

            IsReloading = false;
        }

        private void Awake()
        {
            Setup();
        }

        private void Start()
        {
            if (RecoilTranform) defaultRecoilPosition = RecoilTranform.localPosition;

            if (!preset) return;

            sprayPattern = preset.sprayPattern ? preset.sprayPattern : ScriptableObject.CreateInstance<SprayPattern>();
            aimSprayPattern = preset.aimSprayPattern ? preset.aimSprayPattern : ScriptableObject.CreateInstance<SprayPattern>();

            if (!preset.sprayPattern)
                Debug.LogWarning($"{preset.Name} doesn't have spray pattern firearm will use an instance but it's recommended to use custom pattern");

            if (!preset.aimSprayPattern)
                Debug.LogWarning($"{preset.Name} doesn't have aim spray pattern firearm will use an instance but it's recommended to use custom pattern");

            reserve = preset.reserve;
            magazineCapacity = preset.magazineCapacity;
            if (preset.fireMode == FireMode.Selective) autoFireMode = true;

            if (!_muzzle) _muzzle = transform;
            if (!casingEjectionPort) casingEjectionPort = transform;

            //reset weapon 
            ResetStates();

            if (Inventory)
            {
                //Initialize Ammunition
                AmmoProfile = Inventory.FindAmmunition(preset.ammoType);

                if (Inventory.Controller != null)
                {
                    //Reset movement speed
                    Inventory.Controller.ResetSpeed();
                    AudioHolder.transform.parent = Inventory.Controller.GetOrientation().transform;
                }
            }
            else
            {
                AudioHolder.transform.parent = transform;
            }

            AudioHolder.transform.localPosition = Vector3.zero;
            AudioHolder.transform.localRotation = Quaternion.identity;

            //use this if you want to use the built-in save and load system for the attachemnts
            //LoadAttachments();

            currentFireSFX = preset.fire;

            if (AmmoProfile == null)
            {
                AmmoProfile = new AmmoProfile();
                AmmoProfile.data = ScriptableObject.CreateInstance<AmmoProfileData>();

                AmmoProfile.data.Name = "No Ammo Data";
                AmmoProfile.amount = 100;

                string firearmName = preset ? preset.Name : gameObject.name;
                Debug.LogWarning($"Can't find {firearmName}'s ammo profile in the inventory. Firearm will use clone profile version.");
            }
        }

        public void Update()
        {
            if (!preset)
            {
                Debug.LogError($"Firearm on {gameObject.name} has no preset. Firearm wouldn't function please stop play mode and assign preset in order to function correctly.");
                return;
            }

            //get input
            UpdateInput();

            if (AmmoProfile.amount <= 0) ResetReload();

            //update spray value
            if (Motion)
            {
                currentSpray = Mathf.Lerp(currentSpray, Mathf.Lerp(sprayPattern.GetAmount(this) / 2, aimSprayPattern.GetAmount(this), Motion.ADSAmount) * attachmentsManager.spread, Time.deltaTime * 10);
            }
            else
            {
                currentSpray = sprayPattern.GetAmount(this);
            }

            if (IsFiring) currentSpray = sprayPattern.GetAmount(this);


            reserve = Mathf.Clamp(reserve, 0, magazineCapacity);

            if (shots >= preset.shotCount) shots = 0;

            if (IsFiring && reserve > 0)
            {
                if (PauseMenu.Instance && !PauseMenu.Instance.paused)
                    events.OnFire?.Invoke();

                if (!PauseMenu.Instance) events.OnFire?.Invoke();
            }
            //update animations
            if (Animator)
            {
                Animator.SetBool("Is Reloading", IsReloading);
                Animator.SetInteger("Ammo", reserve);
                Animator.SetFloat("ADS Amount", Motion.ADSAmount);

                if (Inventory.Controller.tacticalSprintAmount > 0 && !Motion.isAiming && !IsFiring) TacticalSprintAmount = Mathf.Lerp(TacticalSprintAmount, Inventory.Controller.tacticalSprintAmount, Time.deltaTime * Motion.sprintSpeed / 1.3f);
                if (Inventory.Controller.tacticalSprintAmount <= 0 && !Motion.isAiming && !IsFiring) TacticalSprintAmount = Mathf.Lerp(TacticalSprintAmount, Inventory.Controller.tacticalSprintAmount, Time.deltaTime * Motion.sprintSpeed * 5);
                if (Motion.isAiming || IsFiring) TacticalSprintAmount = Mathf.Lerp(TacticalSprintAmount, 0, Time.deltaTime * Motion.sprintSpeed * 5);

                Animator.SetFloat("Sprint Amount", TacticalSprintAmount);
            }

            if (HUD)
            {
                HUD.UpdateUI();

                if (HUD.Crosshair)
                {
                    HUD.Crosshair.UpdateSize(currentSpray);
                    if (HUD.Crosshair.floatingRect && _muzzle)
                    {
                        if (Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit))
                            HUD.Crosshair.floatingRect.position = hit.point;
                        else
                            HUD.Crosshair.floatingRect.position = farFireLocation.position;
                    }
                }

                if (Inventory && Motion)
                {
                    if (Inventory.Controller.IsSprinting() && !Motion.isAiming) HUD.Crosshair.HideLines();
                    if (Motion && !Motion.isAiming && !Inventory?.Controller?.IsSprinting() != null) HUD?.Crosshair?.Show();
                }

                if (Motion && Motion.isAiming) HUD.Crosshair.HideAll();
            }
        }

        private void FixedUpdate()
        {
            if (!preset) return;

            CurrentRecoilRotation = Vector3.Lerp(CurrentRecoilRotation, Vector3.zero, 35 * Time.deltaTime);
            CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoilRotation, 50 * Time.deltaTime);
            CurrentRecoilPosition = Vector3.Lerp(CurrentRecoilPosition, Vector3.zero, 35 * Time.deltaTime);
            CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoilPosition, 50 * Time.deltaTime);

            if (preset.Use_Recoil && RecoilTranform)
            {
                RecoilTranform.localPosition = Vector3.Slerp(RecoilTranform.localPosition, defaultRecoilPosition + CurrentRecoilPosition, preset.recoilPositionRoughness * Time.fixedDeltaTime);
                RecoilRotationOutput = Vector3.Slerp(RecoilRotationOutput, CurrentRecoilRotation, preset.recoilRotationRoughness * Time.fixedDeltaTime);
                RecoilTranform.localRotation = Quaternion.Euler(RecoilRotationOutput);
            }

            if (preset.fireTail && !preset.fireTail.AudioSource)
                AudioManager.EquipAudio(AudioHolder, preset.fireTail);

            if (preset.reloadAudio && !preset.reloadAudio.AudioSource)
                AudioManager.EquipAudio(AudioHolder, preset.reloadAudio);

            if (preset.reloadEmptyAudio && !preset.reloadEmptyAudio.AudioSource)
                AudioManager.EquipAudio(AudioHolder, preset.reloadEmptyAudio);

            if (preset.fireTail) AudioManager.UpdatePitch(preset.fireTail);
            if (preset.reloadAudio) AudioManager.UpdatePitch(preset.reloadAudio);
            if (preset.reloadEmptyAudio) AudioManager.UpdatePitch(preset.reloadEmptyAudio);
        }

        private bool autoFireMode;

        /// <summary>
        /// handles all weapon input
        /// </summary>
        private void UpdateInput()
        {
            if (!inputManager) return;

            if (Motion)
            {
                Motion.isCrouching = Inventory.Controller.IsCrouching();
                Motion.IsReloading = IsReloading;
                Motion.IsFiring = IsFiring;
                Motion.IsRecharging = IsRecharging();
            }

            if (Input.GetKeyDown(KeyCode.Tab) && preset.replacement) Drop(Vector3.down * Inventory.dropForce, Vector3.up * Inventory.dropForce * 3, this);

            if (AmmoProfile != null)
            {
                if (Input.GetKeyDown(KeyCode.R) && !IsReloading && reserve < magazineCapacity && AmmoProfile.amount > 0) Reload();
                if (Input.GetKeyDown(KeyCode.Mouse0) && reserve <= 0 && AmmoProfile.amount > 0 && !IsReloading) Reload();
                if (preset.automaticReload && reserve <= 0 && AmmoProfile.amount > 0 && !IsReloading) Reload();
            }

            //fire mode input
            if (preset.Use_Fire)
            {
                if (preset.fireMode == FireMode.Auto)
                {
                    IsFiring = reserve > 0 ? Input.GetKey(KeyCode.Mouse0) : false;
                }

                if (preset.fireMode == FireMode.SemiAuto)
                {
                    IsFiring = reserve > 0 ? Input.GetKeyDown(KeyCode.Mouse0) : false;
                }

                if (preset.fireMode == FireMode.Selective)
                {
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        autoFireMode = !autoFireMode;
                        if (autoFireMode) currentSelectiveFireMode = FireMode.Auto;
                        if (!autoFireMode) currentSelectiveFireMode = FireMode.SemiAuto;

                        events.OnFireModeChange?.Invoke();
                    }

                    if (currentSelectiveFireMode == FireMode.Auto)
                    {
                        IsFiring = reserve > 0 ? Input.GetKey(KeyCode.Mouse0) : false;
                    }

                    if (currentSelectiveFireMode == FireMode.SemiAuto)
                    {
                        IsFiring = reserve > 0 ? Input.GetKeyDown(KeyCode.Mouse0) : false;
                    }
                }
            }

            if (Motion && Motion.Use_LowerdPos)
            {
                if (IsFiring || Inventory.Controller.IsSprinting() || IsReloading)
                {
                    Motion.lowerdTimer = preset.lowerdDelay;
                }
                else if (Motion.lowerdTimer > 0)
                {
                    Motion.lowerdTimer -= Time.deltaTime;
                }

                if (Motion.lowerdTimer <= 0)
                {
                    Motion.IsLowerd = true;
                }
                else
                {
                    Motion.IsLowerd = false;
                }
            }
        }

        public void Fire()
        {
            if (_muzzle)
                Fire(_muzzle.forward);
        }

        /// <summary>
        ///  handles weapon shooting
        /// </summary>
        /// <param name="direction">what direction the projectile should be shot</param>
        public void Fire(Vector3 direction)
        {
            int unit = (int)preset.fireRateUnit;

            if (Time.time > fireTimer)
            {
                if (PauseMenu.Instance && PauseMenu.Instance.paused) return;

                fireTimer = Time.time + 1 / preset.fireRate * unit;
                if (preset.allowFireWhileRecharging || !preset.allowFireWhileRecharging && !IsRecharging())
                {
                    shots = 0;
                    FireDone(direction);

                    if (!preset.alwaysApplyFire)
                        ApplyFireOnce();
                }
            }
        }

        /// <summary>
        /// shoots a projectile without fire rate
        /// </summary>
        private void FireDone(Vector3 direction)
        {
            CancelInvoke(nameof(FireDone));

            foreach (ParticleSystem particle in VFX)
            {
                if (particle != rechargingVFX) particle.Play();
            }

            Projectile newProjectile = CreateProjectile(preset.projectile, this, _muzzle, FireDirection(direction), preset.muzzleVelocity, preset.range, projectilesParent.transform);
            newProjectile.useAutoScaling = preset.tracerRounds;
            newProjectile.scaleMultipler = preset.projectileSize;

            shots++;

            if (shots < preset.shotCount && reserve > 0)
            {
                if (preset.shotDelay <= 0)
                {
                    FireDone(direction);
                }
                else if (shots >= 1)
                    Invoke(nameof(FireDone), preset.shotDelay);
            }

            if (preset.alwaysApplyFire)
            {
                ApplyFireOnce();
            }
        }


        /// <summary>
        /// handles fire animation, audio, crosshair, etc..
        /// </summary>
        private void ApplyFireOnce()
        {
            if (preset.Use_Recoil) ApplyRecoil();
            reserve--;

            if (Animator) Animator.CrossFade("Fire", preset.fireTransition, 0, 0);

            CancelReload();

            Inventory?.Controller?.GetCameraManager()?.ShakeCameras(preset.cameraShake, preset.cameraShakeFadeOutTime);


            if (preset.Use_Audio && currentFireSFX)
            {
                if (currentFireSFX)
                    AudioManager.PlayOneShot(currentFireSFX);

                if (Motion && Motion.Use_Sprinting)
                {
                    Motion.afterShotRelief = true;

                    if (gameObject.activeSelf)
                        StartCoroutine(Motion.ResetSprint(preset.afterShotReliefTime));
                }

                AudioManager.Stop(preset.fireTail);
                AudioManager.PlayOneShot(preset.fireTail);
            }


            if (preset.rechargingType == RechargingType.GasPowerd)
                ThrowCasing();
        }

        /// <summary>
        /// Spwans a casing
        /// </summary>
        public void ThrowCasing()
        {
            if (!preset.casing && !casingEjectionPort) return;

            Rigidbody newCasing = Instantiate(preset.casing, casingEjectionPort.position, casingEjectionPort.rotation).GetComponent<Rigidbody>();

            Vector3 shooterVelocity = Vector3.zero;
            Vector3 casingVelocity = Vector3.zero;

            if (Inventory && Inventory.Controller != null && Inventory.Controller.GetCharacterController()) shooterVelocity = Inventory.Controller.GetCharacterController().velocity;
            casingVelocity = transform.GetDirection(preset.casingDirection) * preset.ejectionVelocity * Random.Range(0.6f, 1);
            casingVelocity += shooterVelocity;

            newCasing.AddForce(casingVelocity, ForceMode.VelocityChange);
            newCasing.transform.Rotate(Random.Range(-preset.casingRotationFactor, preset.casingRotationFactor), Random.Range(-preset.casingRotationFactor, preset.casingRotationFactor), Random.Range(-preset.casingRotationFactor, preset.casingRotationFactor));
            Destroy(newCasing.gameObject, 5);

            if (rechargingVFX) rechargingVFX.Play();
        }

        /// <summary>
        /// Applies force to the weapon & camera
        /// </summary>
        private void ApplyRecoil()
        {

            if (Motion)
            {

                if (Motion.isAiming)
                {
                    CurrentRecoilRotation += attachmentsManager.visualRecoil * preset.staticRecoilRotation + new Vector3(preset.RecoilRotation_Aim.x, Random.Range(-preset.RecoilRotation_Aim.y, preset.RecoilRotation_Aim.y));
                    CurrentRecoilPosition += attachmentsManager.visualRecoil * new Vector3(Random.Range(-preset.RecoilKickBack_Aim.x, preset.RecoilKickBack_Aim.x), Random.Range(-preset.RecoilKickBack_Aim.y, preset.RecoilKickBack_Aim.y), preset.RecoilKickBack_Aim.z);
                }
                if (!Motion.isAiming)
                {
                    CurrentRecoilRotation += attachmentsManager.visualRecoil * preset.staticRecoilRotation + new Vector3(preset.RecoilRotation.x, Random.Range(-preset.RecoilRotation.y, preset.RecoilRotation.y), Random.Range(-preset.RecoilRotation.z, preset.RecoilRotation.z));
                    CurrentRecoilPosition += attachmentsManager.visualRecoil * new Vector3(Random.Range(-preset.RecoilKickBack.x, preset.RecoilKickBack.x), Random.Range(-preset.RecoilKickBack.y, preset.RecoilKickBack.y), preset.RecoilKickBack.z);

                }

                Inventory.Controller.GetCameraManager().ApplyRecoil(preset.verticalRecoil * attachmentsManager.recoil, preset.horizontalRecoil * attachmentsManager.recoil, preset.cameraRecoil * attachmentsManager.recoil, Motion.isAiming);
            }
            else
            {
                Inventory?.Controller?.GetCameraManager()?.ApplyRecoil(preset.verticalRecoil * attachmentsManager.recoil, preset.horizontalRecoil * attachmentsManager.recoil, preset.cameraRecoil * attachmentsManager.recoil);
            }
        }

        /// <summary>
        /// Stops reload process
        /// </summary>
        public void CancelReload()
        {
            if (IsReloading)
            {
                CancelInvoke(nameof(ApplyReload));
                IsReloading = false;

                if (preset.reloadAudio) AudioManager.Stop(preset.reloadAudio);
                if (preset.reloadEmptyAudio) AudioManager.Stop(preset.reloadEmptyAudio);
            }
        }

        /// <summary>
        /// Handles delayed reloads
        /// </summary>
        public void Reload()
        {
            if (AmmoProfile.amount > 0)
            {
                if (GetComponentInChildren<WeaponEvents>())
                {
                    GetComponentInChildren<WeaponEvents>().MagThrown = false;
                }

                IsReloading = true;
                events.OnReload?.Invoke();

                if (preset && preset.reloadType == ReloadType.Automatic)
                {
                    if (preset.reloadAudio) AudioManager.Stop(preset.reloadAudio);
                    if (preset.reloadEmptyAudio) AudioManager.Stop(preset.reloadEmptyAudio);

                    if (preset.reloadAudio && reserve <= 0) AudioManager.Play(preset.reloadEmptyAudio);
                    else if (preset.reloadEmptyAudio) AudioManager.Play(preset.reloadAudio);
                }
            }
            else
            {
                ResetReload();
            }
        }

        private void StartReload()
        {

            if (preset.flexibleReloadTime)
            {
                float totalReloadTime = preset.reloadTime * magazineCapacity;
                float reservePercentage = reserve <= 0 ? 1 : Mathf.InverseLerp(1, 0, (float)reserve / magazineCapacity);

                Invoke(nameof(ApplyReload), (totalReloadTime * reservePercentage));
            }
            else
            {
                Invoke(nameof(ApplyReload), reserve <= 0 ? preset.emptyReloadTime : preset.reloadTime);
            }


        }

        /// <summary>
        /// handles ammo reserve and inventory ammo
        /// </summary>
        public void ApplyReload()
        {
            if (AmmoProfile.amount <= 0)
            {
                return;
            }

            int current;
            int result;

            if (preset.reloadType == ReloadType.Automatic)
            {
                current = magazineCapacity - reserve;
                result = AmmoProfile.amount >= current ? current : AmmoProfile.amount;

                if (AmmoProfile.data.Name != "No Ammo Data") AmmoProfile.amount -= result;
                reserve += result;
            }

            ResetReload();
        }

        /// <summary>
        /// adds (amount) to reserve and removes (amount) from inventory
        /// </summary>
        /// <param name="amount">amount of ammo to add</param>
        public void ApplyReloadOnce(int amount = 1)
        {
            if (AmmoProfile.amount <= 0)
            {
                ResetReload();
                return;
            }

            AmmoProfile.amount -= amount;
            reserve += amount;

            AmmoProfile.amount = Mathf.Clamp(AmmoProfile.amount, 0, int.MaxValue);

            AudioManager.Stop(preset.reloadAudio);
            AudioManager.Stop(preset.reloadEmptyAudio);

            if (!preset.reloadAudio.AudioSource) AudioManager.EquipAudio(Inventory.Controller.GetAudioHolder(), preset.reloadAudio);
            AudioManager.PlayOneShot(preset.reloadAudio);
        }

        /// <summary>
        /// resets is reloading to false
        /// </summary>
        public void ResetReload()
        {
            IsReloading = false;
        }

        /// <summary>
        /// returns random direction from muzzle to muzzle forward
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public Vector3 FireDirection(Vector3 direction)
        {
            Vector3 targetSpread = Vector3.zero;

            if (Motion)
                targetSpread = !Motion.isAiming ? sprayPattern.GetPattern(this, direction) : aimSprayPattern.GetPattern(this, direction);
            else
                targetSpread = sprayPattern.GetPattern(this, direction);

            return targetSpread;
        }

        private void OnDestroy()
        {
            if (Inventory && Inventory.Controller != null)
            {
                Inventory.Controller.ResetSpeed();
            }

            Destroy(AudioHolder);
        }

        private void OnEnable()
        {
            events.OnFire.AddListener(Fire);
            events.OnReload.AddListener(StartReload);

            ResetStates();
            CancelReload();

            if (preset)
            {
                if (preset.fire) AudioManager.EquipAudio(AudioHolder, preset.fire);
                if (preset.fireTail) AudioManager.EquipAudio(AudioHolder, preset.fireTail);
                if (preset.reloadAudio) AudioManager.EquipAudio(AudioHolder, preset.reloadAudio);
                if (preset.reloadEmptyAudio) AudioManager.EquipAudio(AudioHolder, preset.reloadEmptyAudio);
            }
        }

        private void OnDisable()
        {
            //use this if you want to use the built-in save and load system for the attachemnts
            //SaveAttachments();

            CancelReload();


            if (Inventory && Inventory.Controller != null)
            {
                Inventory.Controller.ResetSpeed();
            }

            AudioSource[] audioSources = GetComponents<AudioSource>();
            foreach (AudioSource source in audioSources)
            {
                Destroy(source);
            }
        }

        private void OnApplicationQuit()
        {
            //use this if you want to use the built-in save and load system for the attachemnts
            //SaveAttachments();
        }



        /// <summary>
        /// returns true if recharging animation is playing
        /// </summary>
        /// <returns></returns>
        public bool IsRecharging()
        {
            return Animator ? Animator.GetCurrentAnimatorStateInfo(0).IsName(preset.rechargingStateName) : false;
        }

        public float FiringAmount
        {
            get
            {
                return IsFiring ? 1 : Mathf.Lerp(1, 0, Time.deltaTime * 10);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Setup/Animator parameter")]
        public void AddMissingAnimatorParameters()
        {
            //get the target animator
            Animator animator = GetComponentInChildren<Animator>();

            if (!animator)
            {
                Debug.LogError($"Can't find animator in {gameObject.name}'s children make sure to add animator and assgin animator controller.");
                return;
            }

            AddParameter("Is Reloading", AnimatorControllerParameterType.Bool, false);
            AddParameter("Sprint Amount", AnimatorControllerParameterType.Float, false);
            AddParameter("ADS Amount", AnimatorControllerParameterType.Float, false);
            AddParameter("Ammo", AnimatorControllerParameterType.Int, false);
        }

        protected void AddParameter(string name, AnimatorControllerParameterType type, bool overwrite)
        {
            //get the target animator
            Animator animator = GetComponentInChildren<Animator>();

            //check if animator controller has state with the name given
            if (!overwrite && HasParameter(name, animator))
            {
                //if overwrite is false and animator controller has paremator don't continue and print a message to the consle
                Debug.Log($"Animator on {animator.gameObject.name} already has Parameter with the name ({name}).");
                return;
            }

            //get animator controller
            UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;

            // upate name and type
            AnimatorControllerParameter parameter = new AnimatorControllerParameter();
            parameter.type = type;
            parameter.name = name;

            //add the parameter
            animatorController.AddParameter(parameter);
        }

        public static bool HasParameter(string paramName, Animator animator)
        {
            //loop through every parameter and if animator has parameter return true if not return false
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                //check if parameter name matchs given name
                if (param.name == paramName)
                    return true;
            }
            return false;
        }
#endif
    }
}