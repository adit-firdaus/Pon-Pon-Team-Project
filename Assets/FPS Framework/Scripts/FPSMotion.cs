using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace FPSFramework
{
    [AddComponentMenu("Weapons/Animation/FPS Motion")]

    public class FPSMotion : MonoBehaviour
    {
        #region Variables
        //aim down sight
        [HideInInspector] public Transform aimTranform;
        [HideInInspector] public float aimSpeed = 5;
        [HideInInspector] public float aimFieldOfView = 50;
        [HideInInspector] public float aimWeaponFieldOfview = 40;
        [HideInInspector] public Vector3 aimOffcet = new Vector3(-0.0813f, 0.001f, 0.02f);
        [HideInInspector] public Vector3 aimRotation;
        [HideInInspector] public bool aimedReload = true;

        public bool isAiming { get; set; }


        //movement
        [HideInInspector] public float Movement_WalkSpeed = 5;
        [HideInInspector] public float Movement_RunSpeed = 10;
        [HideInInspector] public float Movement_TacticalSprintSpeed = 12f;
        [HideInInspector] public float Movement_AimWalkSpeed = 3;
        [HideInInspector] public float Movement_FireWalkSpeed = 4;
        [HideInInspector] public bool allowFullMovementSpeedWhileAiming;


        //bobbing
        [HideInInspector] public Transform bobbingTransform;
        [HideInInspector] public float bobbingAmount = 1f;
        [HideInInspector] public float bobbingRotationAmount = 2;
        [HideInInspector] public float aimBobbingMultipler = 0.25f;
        [HideInInspector] public float sprintingMultipler = 5;
        [HideInInspector] public float tacticalSprintingMultipler = 7;


        //Jumping & Landing
        [HideInInspector] public Transform JumpBobTranform;

        [Space(10f)]
        [HideInInspector] public Vector3 landingRotation = new Vector3(10f, 0f, 0f);
        [HideInInspector] public Vector3 landingPosition = new Vector3(0f, -0.1f, 0f);
        [HideInInspector] public Vector3 jumpRotation = new Vector3(3f, 0f, 0.5f);
        [HideInInspector] public Vector3 jumpPosition = new Vector3(0f, -0.06f, 0f);


        //sprinting
        [HideInInspector] public Transform sprintTransform;
        [HideInInspector] public Vector3 sprintPos = new Vector3(-0.07f, -0.03f, -0.04f);
        [HideInInspector] public Vector3 sprintRot = new Vector3(12.2f, -21.06f, 30.1f);

        [HideInInspector] public Vector3 tacticalSprintPos = new Vector3(0.04f, 0.11f, -0.18f);
        [HideInInspector] public Vector3 tacticalSprintRot = new Vector3(-44.92f, 7.34f - 4.94f);
        [HideInInspector] public float sprintSpeed = 4;


        //sway
        [HideInInspector] public Transform swayTransform;
        [HideInInspector] public float swayPositionAmount = -0.4f;
        [HideInInspector] public float swayPositonAimAmount = -0.26f;

        [HideInInspector] public float swayRotationAmount = -0.2f;
        [HideInInspector] public float swayRotationAimAmount = -0.1f;

        [HideInInspector] public float swayPositionSmoothness = 10;
        [HideInInspector] public float swayRotationSmoothness = 15;


        //holding
        [HideInInspector] public CameraShaker holder;
        [HideInInspector] public float idleMagnitude = 0.2f;
        [HideInInspector] public float movingMagnitude = 2;
        [HideInInspector] public float inAirMagnitude = 4;
        [HideInInspector] public float inAirAimMagnitude = 0.5f;
        [HideInInspector] public float aimingMagnitude = 0.2f;
        [HideInInspector] public float holdingRoughness = 0.3f;
        [HideInInspector] public float holdingFadeTime = 0.4f;

        //crouch
        [HideInInspector] public Transform crouchTransform;
        [HideInInspector] public Vector3 crouchPos = new Vector3(0.005f, 0, -0.03f);
        [HideInInspector] public Vector3 crouchRot = new Vector3(0, 0, 4);

        public bool isCrouching { get; set; }

        //Obstacle Avoidnace
        [HideInInspector] public LayerMask obstacleAvoidnaceLayermask;
        [HideInInspector] public Transform obstacleAvoidnaceTransform;
        [HideInInspector] public float obstacleAvoidnaceAmount;
        [HideInInspector] public Vector3 obstacleAvoidnacePosition = new Vector3(0, 0.1f, -0.3f);
        [HideInInspector] public Vector3 obstacleAvoidnaceRotation = new Vector3(-58.49f, 0, 5.7f);
        [HideInInspector] public float obstacleAvoidnaceRange = 1;
        [HideInInspector] public float obstacleAvoidnaceSmoothness = 8;


        //Leaning
        [HideInInspector] public Transform leaningTransform;
        [HideInInspector] public float leaningRotationAngle = 15;
        [HideInInspector] public Vector3 leaningOffsetRight = new Vector3(0.03f, -0.01f, 0);
        [HideInInspector] public Vector3 leaningOffsetLeft = new Vector3(-0.13f, -0.01f, 0);
        [HideInInspector] public Vector3 leanOffset;
        [HideInInspector] public float leaningSmoothness = 10;

        //Lowerd Pos
        [HideInInspector] public Transform lowerdPosTransform;
        [HideInInspector] public float lowerdSpeed = 1;
        [HideInInspector] public float lowerdDelay = 10;
        [HideInInspector] public Vector3 lowerdPos = new Vector3(0, -0.03f, 0);
        [HideInInspector] public Vector3 lowerdRot = new Vector3(0, 0, 7.01f);
        private RaycastHit obstacleAvoidnaceHit;
        #endregion

        #region Defaults
        private Vector3 defaultAimPosition;
        private Quaternion defaultAimRotation;
        private Vector3 defaultBobbingPosition;
        private Vector3 defaultSprintingPosition;
        private Quaternion defaultSprintingRotation;
        private Vector3 JumpBob_Current1;
        private Vector3 JumpBob_Current2;
        private Vector3 JumpBob_Current3;
        private Vector3 JumpBob_Current4;
        private Vector3 JumpBob_RotationOutput;
        private Vector3 defaultLeaningPosition;
        private Vector3 leaningFinalPosRight;
        private Vector3 leaningFinalPosLeft;
        private Vector3 defaultObstacleAvoidnacePosition;
        private Vector3 defaultSwayPosition;
        private Vector3 currentSwayPosition;
        private Vector3 defaultLowerdPos;
        private Quaternion defaultLowerdRot;
        private Vector3 defaultCrouchPos;
        private Quaternion defaultCrouchRot;
        #endregion

        public Inventory Inventory { get; set; }

        [HideInInspector] public bool Use_AimDownSight;
        [HideInInspector] public bool Use_Crouch;
        [HideInInspector] public bool Use_Movement;
        [HideInInspector] public bool Use_Bobbing;
        [HideInInspector] public bool Use_Jumping;
        [HideInInspector] public bool Use_Sprinting;
        [HideInInspector] public bool Use_TacticalSprint;
        [HideInInspector] public bool Use_Sway;
        [HideInInspector] public bool Use_Holding;
        [HideInInspector] public bool Use_ObstacleAvoidnace;
        [HideInInspector] public bool Use_Leaning;
        [HideInInspector] public bool Use_LowerdPos;

        public bool IsLowerd { get; set; }
        public bool IsFiring { get; set; }
        public bool IsReloading { get; set; }
        public bool IsRecharging { get; set; }


        public bool afterShotRelief { get; set; }
        private float sprintValue;
        private float bobbingMultipler;
        private float bobingTimer;
        private float currentHoldingMagnitude;
        public float lowerdTimer { get; set; }
        public float leanAngle { get; set; }
        public float ADSAmount { get; set; }

        public Firearm firearm { get; set; }

        /// <summary>
        /// Resets all values of the weapon
        /// </summary>
        public void ResetStates()
        {
            isAiming = false;
            IsLowerd = false;
            afterShotRelief = false;

            lowerdTimer = lowerdDelay;
            bobbingMultipler = 1;
        }

        /// <summary>
        /// Gets and sets all values for (sprint, ADS, recoil, etc..) to the default value
        /// </summary>
        public void GetDefaults()
        {
            defaultLeaningPosition = leaningTransform != null ? leaningTransform.localPosition : Vector3.zero;
            defaultSwayPosition = swayTransform != null ? swayTransform.localPosition : Vector3.zero;
            defaultLowerdPos = lowerdPosTransform != null ? lowerdPosTransform.localPosition : Vector3.zero;
            defaultLowerdRot = lowerdPosTransform != null ? lowerdPosTransform.localRotation : Quaternion.identity;
            defaultAimPosition = aimTranform != null ? aimTranform.localPosition : Vector3.zero;
            defaultAimRotation = aimTranform != null ? aimTranform.localRotation : Quaternion.identity;
            defaultBobbingPosition = bobbingTransform != null ? bobbingTransform.localPosition : Vector3.zero;
            defaultSprintingPosition = sprintTransform != null ? sprintTransform.localPosition : Vector3.zero;
            defaultSprintingRotation = sprintTransform != null ? sprintTransform.localRotation : Quaternion.identity;
            defaultObstacleAvoidnacePosition = obstacleAvoidnaceTransform != null ? obstacleAvoidnaceTransform.localPosition : Vector3.zero;
            defaultCrouchPos = crouchTransform != null ? crouchTransform.localPosition : Vector3.zero;
            defaultCrouchRot = crouchTransform != null ? crouchTransform.localRotation : Quaternion.identity;
        }

        private void Awake()
        {
            //find components
            Inventory = GetComponentInParent<Inventory>();

            //get default values
            GetDefaults();
        }

        private void Start()
        {
            //reset weapon 
            ResetStates();

            if (Inventory)
            {
                if (Inventory.Controller != null)
                {
                    //Reset movement speed
                    if (!Use_Movement) Inventory.Controller.ResetSpeed();
                }

                //Initialize actions
                Inventory.Controller.OnJump.AddListener(Jump);
                Inventory.Controller.OnLand.AddListener(Land);
            }
        }

        private void Update()
        {
            //get input
            UpdateInput();
            UpdateAim();
            UpdateBobbing();
            UpdateSprint();
            UpdateSway();
            UpdateMovement();
            UpdateObstacleAvoidnace();
            UpdateLeaning();
            UpdateLowerdPos();
            UpdateCrouch();

            if (Use_Jumping)
            {
                JumpBobTranform.localPosition = Vector3.Slerp(JumpBobTranform.localPosition, JumpBob_Current3, 6f * Time.deltaTime);
                JumpBob_RotationOutput = Vector3.Slerp(JumpBob_RotationOutput, JumpBob_Current1, 12f * Time.deltaTime);
                JumpBobTranform.localRotation = Quaternion.Euler(JumpBob_RotationOutput);
            }
        }

        private void FixedUpdate()
        {
            UpdateHolding();
            JumpBob_Current1 = Vector3.Lerp(JumpBob_Current1, Vector3.zero, 10f * Time.fixedDeltaTime);
            JumpBob_Current2 = Vector3.Lerp(JumpBob_Current2, JumpBob_Current1, 10f * Time.fixedDeltaTime);
            JumpBob_Current3 = Vector3.Lerp(JumpBob_Current3, Vector3.zero, 10f * Time.fixedDeltaTime);
            JumpBob_Current4 = Vector3.Lerp(JumpBob_Current4, JumpBob_Current3, 10f * Time.fixedDeltaTime);
        }


        /// <summary>
        /// handles all weapon input
        /// </summary>
        private void UpdateInput()
        {
            if (Use_AimDownSight && Input.GetKeyDown(Inventory.aimKey) && Inventory.toggleAim) isAiming = !isAiming;
            if (Use_AimDownSight && !Inventory.toggleAim) isAiming = Input.GetKey(Inventory.aimKey);


            if (Use_LowerdPos)
            {
                if (IsFiring || Inventory.Controller.IsSprinting() || IsReloading || isAiming)
                {
                    lowerdTimer = lowerdDelay;
                }
                else if (lowerdTimer > 0)
                {
                    lowerdTimer -= Time.deltaTime;
                }

                if (lowerdTimer <= 0)
                {
                    IsLowerd = true;
                }
                else
                {
                    IsLowerd = false;
                }
            }
        }

        /// <summary>
        /// rests time after shooting
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerator ResetSprint(float delay)
        {
            yield return new WaitForSeconds(delay);
            afterShotRelief = false;
        }

        /// <summary>
        /// updates Obstacle Avoidnace animation
        /// </summary>
        private void UpdateObstacleAvoidnace()
        {
            if (!Use_ObstacleAvoidnace || !obstacleAvoidnaceTransform) return;

            if (IsAvoidingObstacle())
            {
                float currentDistanceFromObstacle = obstacleAvoidnaceHit.distance;
                obstacleAvoidnaceAmount = (currentDistanceFromObstacle / obstacleAvoidnaceRange);
                obstacleAvoidnaceAmount = Mathf.InverseLerp(1, 0, obstacleAvoidnaceAmount);
            }
            else
            {
                float currentDistanceFromObstacle = obstacleAvoidnaceHit.distance;
                currentDistanceFromObstacle = Mathf.Lerp(currentDistanceFromObstacle, 0, Time.deltaTime * 10);
                obstacleAvoidnaceAmount = Mathf.Lerp(obstacleAvoidnaceAmount, 0, Time.deltaTime * 10);
            }

            obstacleAvoidnaceAmount = Mathf.Clamp(obstacleAvoidnaceAmount, 0, 0.7f);

            if (Use_ObstacleAvoidnace)
            {
                obstacleAvoidnaceTransform.localPosition = Vector3.Lerp(obstacleAvoidnaceTransform.localPosition, defaultObstacleAvoidnacePosition + obstacleAvoidnacePosition * obstacleAvoidnaceAmount, Time.deltaTime * obstacleAvoidnaceSmoothness);
                obstacleAvoidnaceTransform.localRotation = (Quaternion.Slerp(obstacleAvoidnaceTransform.localRotation, Quaternion.Euler(obstacleAvoidnaceRotation * obstacleAvoidnaceAmount), Time.deltaTime * obstacleAvoidnaceSmoothness));
            }
        }

        /// <summary>
        /// returns true if facing an Obstacle in the range
        /// </summary>
        /// <returns></returns>
        public bool IsAvoidingObstacle()
        {
            if (Use_ObstacleAvoidnace && Physics.Raycast(obstacleAvoidnaceTransform.position, Inventory.Controller.GetCameraManager().mainCamera.transform.forward, out obstacleAvoidnaceHit, obstacleAvoidnaceRange, obstacleAvoidnaceLayermask) && !obstacleAvoidnaceHit.transform.GetComponent<IgnoreObstacleAvoidnace>())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// rotates the weapon a bit to handle it
        /// </summary>
        private void UpdateLeaning()
        {
            if (!Use_Leaning || !leaningTransform || !Inventory.Controller.GetCameraManager().Use_Lean) return;

            leaningFinalPosRight = new Vector3(defaultLeaningPosition.x, defaultLeaningPosition.y, defaultLeaningPosition.z) + leaningOffsetRight;
            leaningFinalPosLeft = new Vector3(defaultLeaningPosition.x, defaultLeaningPosition.y, defaultLeaningPosition.z) + leaningOffsetLeft;

            if (Input.GetKey(KeyCode.E) && Use_Leaning)
            {
                if (!isAiming)
                    leaningTransform.localPosition = Vector3.Lerp(leaningTransform.localPosition, leaningFinalPosRight, Time.deltaTime * leaningSmoothness);
                else leaningTransform.localPosition = Vector3.Lerp(leaningTransform.localPosition, defaultLeaningPosition + leanOffset, Time.deltaTime * leaningSmoothness);
                leanAngle = Mathf.Lerp(leanAngle, -leaningRotationAngle, Time.deltaTime * leaningSmoothness);
            }

            if (Input.GetKey(KeyCode.Q) && Use_Leaning)
            {
                if (!isAiming)
                    leaningTransform.localPosition = Vector3.Lerp(leaningTransform.localPosition, leaningFinalPosLeft, Time.deltaTime * leaningSmoothness);
                else leaningTransform.localPosition = Vector3.Lerp(leaningTransform.localPosition, defaultLeaningPosition - leanOffset, Time.deltaTime * leaningSmoothness);
                leanAngle = Mathf.Lerp(leanAngle, leaningRotationAngle, Time.deltaTime * leaningSmoothness);
            }

            if (!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q))
            {
                leaningTransform.localPosition = Vector3.Lerp(leaningTransform.localPosition, defaultLeaningPosition, Time.deltaTime * leaningSmoothness);
                leanAngle = Mathf.Lerp(leanAngle, 0, Time.deltaTime * leaningSmoothness);
            }


            leaningTransform.localEulerAngles = new Vector3(0, 0, leanAngle);
        }

        private void UpdateLowerdPos()
        {
            if (!Use_LowerdPos) return;
            lowerdPosTransform.localPosition = IsLowerd && !Inventory.Controller.IsSprinting() && !isAiming ? Vector3.Lerp(lowerdPosTransform.localPosition, lowerdPos, Time.deltaTime * lowerdSpeed) : Vector3.Lerp(lowerdPosTransform.localPosition, defaultLowerdPos, Time.deltaTime * lowerdSpeed * 8);
            lowerdPosTransform.localRotation = IsLowerd && !Inventory.Controller.IsSprinting() && !isAiming ? Quaternion.Slerp(lowerdPosTransform.localRotation, Quaternion.Euler(lowerdRot), Time.deltaTime * lowerdSpeed) : Quaternion.Slerp(lowerdPosTransform.localRotation, defaultLowerdRot, Time.deltaTime * lowerdSpeed * 8);
        }

        private void UpdateCrouch()
        {
            if (!Use_Crouch) return;

            Vector3 currentPos;
            Quaternion currentRot;

            currentPos = !isAiming && isCrouching ? crouchPos : defaultCrouchPos;
            currentRot = !isAiming && isCrouching ? Quaternion.Euler(crouchRot) : defaultCrouchRot;

            crouchTransform.localPosition = Vector3.Lerp(crouchTransform.localPosition, currentPos, Time.deltaTime * 6);
            crouchTransform.localRotation = Quaternion.Lerp(crouchTransform.localRotation, currentRot, Time.deltaTime * 6);
        }

        /// <summary>
        /// updates movement speed in the controller
        /// </summary>
        private void UpdateMovement()
        {
            if (!Use_Movement) return;

            if (IsFiring || afterShotRelief)
            {
                Inventory.Controller.SetSpeed(Movement_FireWalkSpeed, Movement_FireWalkSpeed, Movement_FireWalkSpeed);
            }
            else
            {
                if (isAiming)
                {
                    Inventory.Controller.SetSpeed(Movement_AimWalkSpeed, Movement_AimWalkSpeed, Movement_AimWalkSpeed);
                }
                else
                {
                    Inventory.Controller.SetSpeed(Movement_WalkSpeed, Movement_RunSpeed, Movement_TacticalSprintSpeed);
                }
            }
        }

        /// <summary>
        /// shakes weapon if walking, animing, springing to give it more naturle look
        /// </summary>
        private void UpdateHolding()
        {
            if (Inventory.Controller.GetCharacterController().IsVelocityZero() && Inventory.Controller.GetCharacterController().isGrounded)
            {
                currentHoldingMagnitude = idleMagnitude;
            }

            if (!Inventory.Controller.GetCharacterController().IsVelocityZero() && Inventory.Controller.GetCharacterController().isGrounded)
            {
                currentHoldingMagnitude = movingMagnitude * (Inventory.Controller.GetCharacterController().velocity.magnitude / Inventory.Controller.walkSpeed);
            }

            if (!Inventory.Controller.GetCharacterController().IsVelocityZero() && !Inventory.Controller.GetCharacterController().isGrounded)
            {
                currentHoldingMagnitude = inAirMagnitude;
            }

            if (isAiming && Inventory.Controller.GetCharacterController().isGrounded)
            {
                currentHoldingMagnitude = aimingMagnitude;
            }

            if (isAiming && !Inventory.Controller.GetCharacterController().isGrounded)
            {
                currentHoldingMagnitude = inAirAimMagnitude;
            }

            if (Use_Holding)
                holder.Shake(currentHoldingMagnitude, holdingRoughness, holdingFadeTime, holdingFadeTime);
        }

        /// <summary>
        /// handles sway animation depending on camera speed
        /// </summary>
        private void UpdateSway()
        {
            if (!swayTransform || !Use_Sway) return;

            float positionAmount = !isAiming ? swayPositionAmount : swayPositonAimAmount;
            float rotationAmount = !isAiming ? swayRotationAmount : swayRotationAimAmount;

            bool IsPaused = new bool();

            if (PauseMenu.Instance) IsPaused = PauseMenu.Instance.paused;
            else IsPaused = false;

            float mouseXPos = Use_Sway && !IsPaused ? (Inventory.Controller.MouseX * positionAmount) / 100 : 0;
            float mouseYPos = Use_Sway && !IsPaused && !Inventory.Controller.MaxedCameraRotation() ? (Inventory.Controller.MouseY * positionAmount) / 100 : 0;
            float mouseXRot = Use_Sway && !IsPaused ? Inventory.Controller.MouseX * rotationAmount : 0;
            float mouseYRot = Use_Sway && !IsPaused && !Inventory.Controller.MaxedCameraRotation() ? (Inventory.Controller.MouseY * rotationAmount) / 2 : 0;

            currentSwayPosition.x = mouseXPos;
            currentSwayPosition.y = mouseYPos;

            Quaternion rotationX = Quaternion.AngleAxis(mouseYRot, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(-mouseXRot, Vector3.forward);
            Quaternion rotationZ = Quaternion.AngleAxis(mouseXRot, Vector3.up);
            Quaternion result = rotationX * rotationZ * rotationY;

            swayTransform.localPosition = Vector3.Lerp(swayTransform.localPosition, defaultSwayPosition + currentSwayPosition, Time.deltaTime * swayPositionSmoothness);
            swayTransform.localRotation = Quaternion.Slerp(swayTransform.localRotation, result, Time.deltaTime * swayRotationSmoothness);
        }


        /// <summary>
        /// handles aim down sight animation and zoom level
        /// </summary>
        private void UpdateAim()
        {
            bool allowed = aimedReload ? true : !IsReloading;
            float aimSpeedResult = firearm ? aimSpeed * firearm.attachmentsManager.aimSpeed : aimSpeed;
            if (isAiming && Use_AimDownSight && !IsAvoidingObstacle() && allowed)
            {
                ADSAmount = Mathf.Lerp(ADSAmount, 1, Time.deltaTime * aimSpeedResult * 4);
                aimTranform.localPosition = Vector3.Lerp(aimTranform.localPosition, aimOffcet, Time.deltaTime * aimSpeedResult * 2);
                aimTranform.localRotation = Quaternion.Slerp(aimTranform.localRotation, Quaternion.Euler(aimRotation), Time.deltaTime * aimSpeedResult * 2);
                Inventory.Controller.GetCameraManager().SetFieldOfView(aimFieldOfView, aimWeaponFieldOfview, Time.deltaTime * aimSpeedResult * 4);
            }
            else if (Use_AimDownSight)
            {
                ADSAmount = Mathf.Lerp(ADSAmount, 0, Time.deltaTime * aimSpeedResult * 2);
                aimTranform.localPosition = Vector3.Lerp(aimTranform.localPosition, defaultAimPosition, Time.deltaTime * aimSpeedResult * 1);
                aimTranform.localRotation = Quaternion.Slerp(aimTranform.localRotation, defaultAimRotation, Time.deltaTime * aimSpeedResult * 2);
                Inventory.Controller.GetCameraManager().ResetFieldOfView(Time.deltaTime * aimSpeedResult * 2);
            }
        }

        /// <summary>
        /// handles boobing animation
        /// </summary>
        private void UpdateBobbing()
        {
            if (Use_Bobbing)
            {
                bobingTimer += Time.deltaTime * Inventory.Controller.GetCharacterController().velocity.magnitude;
                float posX = 0f;
                float posY = 0f;
                float rotZ = 0;
                float multipler = new float();
                if (!isAiming && !Inventory.Controller.IsSprinting() || !isAiming && IsFiring || !isAiming && afterShotRelief) multipler = 1;
                if (isAiming && !afterShotRelief) multipler = aimBobbingMultipler;
                if (Inventory.Controller.IsSprinting() && !isAiming && !Inventory.Controller.IsTacticalSprinting() && !IsFiring && !afterShotRelief) multipler = sprintingMultipler;
                if (Inventory.Controller.IsTacticalSprinting() && !isAiming && !IsFiring && !afterShotRelief) multipler = tacticalSprintingMultipler;
                posX += ((bobbingAmount / 100) / 2f * Mathf.Sin(bobingTimer) * multipler);
                posY += ((bobbingAmount / 100) / 2f * Mathf.Sin(bobingTimer * 2f) * multipler);
                rotZ += ((bobbingRotationAmount / 100) / 2 * Mathf.Sin(bobingTimer) * multipler * bobbingMultipler);


                Vector3 posResult = new Vector3(posX, posY);
                Quaternion rotResult = new Quaternion(bobbingTransform.localRotation.x, bobbingTransform.localRotation.y, rotZ, bobbingTransform.localRotation.w);

                if (!Inventory.Controller.GetCharacterController().IsVelocityZero() && Inventory.Controller.GetCharacterController().isGrounded && Use_Bobbing)
                {
                    bobbingTransform.localPosition = Vector3.Lerp(bobbingTransform.localPosition, defaultBobbingPosition + posResult, Time.deltaTime * 5);
                    bobbingTransform.localRotation = Quaternion.Slerp(bobbingTransform.localRotation, rotResult, Time.deltaTime * 5);
                }
                else
                {
                    bobbingTransform.localPosition = Vector3.Lerp(bobbingTransform.localPosition, defaultBobbingPosition, Time.deltaTime * 5);
                    bobbingTransform.localRotation = Quaternion.Slerp(bobbingTransform.localRotation, Quaternion.identity, Time.deltaTime * 5);
                }
            }
        }

        /// <summary>
        /// handles sprinting animations
        /// </summary>
        private void UpdateSprint()
        {
            if (Use_Sprinting)
            {
                float multipler;

                if (Inventory.Controller.IsSprinting() && !IsRecharging && !isAiming && !IsReloading && !IsFiring && !Inventory.Controller.IsTacticalSprinting() && Inventory.Controller.canTacticalSprint && !afterShotRelief && !IsAvoidingObstacle())
                {
                    multipler = 1;
                    bobbingMultipler = Mathf.Lerp(bobbingMultipler, -1, Time.deltaTime * sprintSpeed * 3);
                    sprintValue = Mathf.Lerp(sprintValue, 0, Time.deltaTime * sprintSpeed * 5);
                    sprintTransform.localPosition = Vector3.Lerp(sprintTransform.localPosition, sprintPos, Time.deltaTime * sprintSpeed * multipler);
                    sprintTransform.localRotation = Quaternion.Slerp(sprintTransform.localRotation, Quaternion.Euler(sprintRot), Time.deltaTime * sprintSpeed * multipler);
                }
                else if (!Inventory.Controller.IsTacticalSprinting() || isAiming || IsFiring && Inventory.Controller.canTacticalSprint || afterShotRelief || IsReloading || IsRecharging)
                {
                    multipler = 3;
                    bobbingMultipler = Mathf.Lerp(bobbingMultipler, -1, Time.deltaTime * sprintSpeed);
                    sprintValue = Mathf.Lerp(sprintValue, 0, Time.deltaTime * sprintSpeed * 3);
                    sprintTransform.localPosition = Vector3.Lerp(sprintTransform.localPosition, defaultSprintingPosition, Time.deltaTime * sprintSpeed * multipler);
                    sprintTransform.localRotation = Quaternion.Slerp(sprintTransform.localRotation, defaultSprintingRotation, Time.deltaTime * sprintSpeed * multipler);
                }

                if (Inventory.Controller.IsSprinting() && !IsRecharging && !isAiming && !IsFiring && !IsReloading && !Inventory.Controller.IsTacticalSprinting() && !Inventory.Controller.canTacticalSprint && !afterShotRelief && !IsAvoidingObstacle())
                {
                    multipler = 3;
                    bobbingMultipler = Mathf.Lerp(bobbingMultipler, -1, Time.deltaTime * sprintSpeed * 5);
                    sprintValue = Mathf.Lerp(sprintValue, 0, Time.deltaTime * sprintSpeed);
                    sprintTransform.localPosition = Vector3.Lerp(sprintTransform.localPosition, sprintPos, Time.deltaTime * sprintSpeed * multipler);
                    sprintTransform.localRotation = Quaternion.Slerp(sprintTransform.localRotation, Quaternion.Euler(sprintRot), Time.deltaTime * sprintSpeed * multipler);
                }

                if (!isAiming && !IsRecharging && !IsFiring && Inventory.Controller.IsTacticalSprinting() && !IsReloading && !afterShotRelief && !IsAvoidingObstacle() && !IsReloading)
                {
                    multipler = 2;
                    bobbingMultipler = Mathf.Lerp(bobbingMultipler, 1, Time.deltaTime * sprintSpeed);
                    sprintValue = Mathf.Lerp(sprintValue, 10, Time.deltaTime * sprintSpeed);
                    sprintTransform.localPosition = Vector3.Lerp(sprintTransform.localPosition, tacticalSprintPos, Time.deltaTime * sprintSpeed * multipler);
                    sprintTransform.localRotation = Quaternion.Slerp(sprintTransform.localRotation, Quaternion.Euler(tacticalSprintRot), Time.deltaTime * sprintSpeed * multipler);
                }
            }
        }

        /// <summary>
        /// handles Jumping aimation
        /// </summary>
        private void Jump()
        {
            float multipler = !isAiming ? 1 : 0.5f;

            JumpBob_Current3 += new Vector3(jumpPosition.x, jumpPosition.y, jumpPosition.z) * multipler;
            JumpBob_Current1 += new Vector3(jumpRotation.x, jumpRotation.y, jumpRotation.z) * multipler;
        }



        /// <summary>
        /// handles landing animations
        /// </summary>
        private void Land()
        {
            float multipler = !isAiming ? 1 : 0.5f;

            JumpBob_Current3 += new Vector3(landingPosition.x, landingPosition.y, landingPosition.z) * multipler;
            JumpBob_Current1 += new Vector3(landingRotation.x, landingRotation.y, landingRotation.z) * multipler;
        }

        private void OnDestroy()
        {
            Inventory.Controller.ResetSpeed();
        }

        private void OnDisable()
        {
            if (Inventory.Controller != null)
            {
                Inventory.Controller.ResetSpeed();
            }
        }

        private void OnEnable()
        {
            ResetStates();
            ResetMotion();
        }

        public void ResetMotion()
        {
            aimTranform.localPosition = defaultAimPosition;
            aimTranform.localRotation = defaultAimRotation;
            bobbingTransform.localPosition = defaultBobbingPosition;
            sprintTransform.localPosition = defaultSprintingPosition;
            sprintTransform.localRotation = defaultSprintingRotation;
            swayTransform.localPosition = defaultSwayPosition;
            obstacleAvoidnaceTransform.localPosition = defaultObstacleAvoidnacePosition;
            leaningTransform.localPosition = defaultLeaningPosition;
            lowerdPosTransform.localPosition = defaultLowerdPos;
            lowerdPosTransform.localRotation = defaultLowerdRot;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FPSMotion))]
    public class WeaponMotionEditor : Editor
    {
        public static bool Foldout_AimDownSight;
        public static bool Foldout_Crouch;
        public static bool Foldout_Movement;
        public static bool Foldout_Bobbing;
        public static bool Foldout_JumpingAndLanding;
        public static bool Foldout_Sprinting;
        public static bool Foldout_Sway;
        public static bool Foldout_Holding;
        public static bool Foldout_ObstacleAvoidnace;
        public static bool Foldout_Leaning;
        public static bool Foldout_LowerdPos;


        public override void OnInspectorGUI()
        {
            FPSMotion motion = (FPSMotion)target;
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            UpdateAimDownSight(motion);
            UpdateMovement(motion);
            UpdateBobing(motion);
            UpdateJumpingAndLanding(motion);
            UpdateSprinting(motion);
            UpdateSway(motion);
            UpdateCrouch(motion);
            UpdateHolding(motion);
            UpdateObstacleAvoidnace(motion);
            UpdateLeaning(motion);
            UpdateLowerdPos(motion);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(motion);
                Undo.RecordObject(motion, "weapon motion modified");
            }
        }

        /// <summary>
        /// draws message if object is null
        /// </summary>
        /// <param name="type">message type</param>
        /// <param name="target">target object</param>
        /// <param name="message">message content</param>
        private void DrawMessage(MessageType type, object target, string message)
        {
            if (target != null) return;
            EditorGUILayout.HelpBox(message, MessageType.Error);
            EditorGUILayout.Space();
        }

        private void UpdateAimDownSight(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_AimDownSight = EditorGUILayout.Toggle(motion.Use_AimDownSight, GUILayout.MaxWidth(28));
            Foldout_AimDownSight = EditorGUILayout.Foldout(Foldout_AimDownSight, "Aim Down Sight", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_AimDownSight) return;

            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_AimDownSight);

            motion.aimTranform = EditorGUILayout.ObjectField("  Transform", motion.aimTranform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.aimTranform, "Tranform must be assgined in order to use this section.");

            motion.aimSpeed = EditorGUILayout.FloatField("  Speed", motion.aimSpeed);
            motion.aimFieldOfView = EditorGUILayout.FloatField("  Field Of View", motion.aimFieldOfView);
            motion.aimWeaponFieldOfview = EditorGUILayout.FloatField("   Weapon Field Of View", motion.aimWeaponFieldOfview);
            motion.aimOffcet = EditorGUILayout.Vector3Field("  Aim Down Sight Offcet", motion.aimOffcet);
            motion.aimRotation = EditorGUILayout.Vector3Field("  Aim Down Sight Rotation", motion.aimRotation);
            motion.aimedReload = EditorGUILayout.ToggleLeft(new GUIContent(" Aimed Reload", "if true the player will stop aiming when reloading"), motion.aimedReload);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        private void UpdateMovement(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Movement = EditorGUILayout.Toggle(motion.Use_Movement, GUILayout.MaxWidth(28));
            Foldout_Movement = EditorGUILayout.Foldout(Foldout_Movement, "Movement", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Movement) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Movement);

            motion.Movement_WalkSpeed = EditorGUILayout.FloatField("  Walk", motion.Movement_WalkSpeed);
            motion.Movement_RunSpeed = EditorGUILayout.FloatField("  Run", motion.Movement_RunSpeed);
            motion.Movement_TacticalSprintSpeed = EditorGUILayout.FloatField("  Tactical Sprint", motion.Movement_TacticalSprintSpeed);
            motion.Movement_AimWalkSpeed = EditorGUILayout.FloatField("  Aim Walk", motion.Movement_AimWalkSpeed);
            motion.Movement_FireWalkSpeed = EditorGUILayout.FloatField("  Fire Walk", motion.Movement_FireWalkSpeed);

            motion.allowFullMovementSpeedWhileAiming = EditorGUILayout.Toggle("  Allow Full Aim Speed", motion.allowFullMovementSpeedWhileAiming);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateBobing(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Bobbing = EditorGUILayout.Toggle(motion.Use_Bobbing, GUILayout.MaxWidth(28));
            Foldout_Bobbing = EditorGUILayout.Foldout(Foldout_Bobbing, "Bobbing", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Bobbing) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Bobbing);

            motion.bobbingTransform = EditorGUILayout.ObjectField("  Transform", motion.bobbingTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.bobbingTransform, "Tranform must be assgined in order to use this section.");


            motion.bobbingAmount = EditorGUILayout.FloatField("  Amount", motion.bobbingAmount);
            motion.bobbingRotationAmount = EditorGUILayout.FloatField("  Rotation Amount", motion.bobbingRotationAmount);
            motion.aimBobbingMultipler = EditorGUILayout.FloatField("  Aim Down Sight Multipler", motion.aimBobbingMultipler);
            motion.sprintingMultipler = EditorGUILayout.FloatField("  Sprinting Multipler", motion.sprintingMultipler);
            motion.tacticalSprintingMultipler = EditorGUILayout.FloatField("  Tactical Sprinting Multipler", motion.tacticalSprintingMultipler);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateJumpingAndLanding(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Jumping = EditorGUILayout.Toggle(motion.Use_Jumping, GUILayout.MaxWidth(28));
            Foldout_JumpingAndLanding = EditorGUILayout.Foldout(Foldout_JumpingAndLanding, "Jumping & Landing", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_JumpingAndLanding) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Jumping);

            motion.JumpBobTranform = EditorGUILayout.ObjectField("  Jump Bob Tranform", motion.JumpBobTranform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.JumpBobTranform, "Tranform must be assgined in order to use this section.");



            motion.landingPosition = EditorGUILayout.Vector3Field("  Landing Position", motion.landingPosition);
            motion.landingRotation = EditorGUILayout.Vector3Field("  Landing Rotation", motion.landingRotation);
            motion.jumpPosition = EditorGUILayout.Vector3Field("  Jump Position", motion.jumpPosition);
            motion.jumpRotation = EditorGUILayout.Vector3Field("  Jump Rotation", motion.jumpRotation);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateSprinting(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Sprinting = EditorGUILayout.Toggle(motion.Use_Sprinting, GUILayout.MaxWidth(28));
            Foldout_Sprinting = EditorGUILayout.Foldout(Foldout_Sprinting, "Sprinting", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Sprinting) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Sprinting);

            motion.sprintTransform = EditorGUILayout.ObjectField(" Transform", motion.sprintTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.sprintTransform, "Tranform must be assgined in order to use this section.");


            motion.sprintSpeed = EditorGUILayout.FloatField(" Roughness", motion.sprintSpeed);


            motion.sprintPos = EditorGUILayout.Vector3Field(" Position", motion.sprintPos);

            motion.sprintRot = EditorGUILayout.Vector3Field(" Rotation", motion.sprintRot);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(string.Empty, GUILayout.MaxWidth(1));

            motion.Use_TacticalSprint = EditorGUILayout.ToggleLeft("Use Tactical", motion.Use_TacticalSprint);

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!motion.Use_TacticalSprint);
            motion.tacticalSprintPos = EditorGUILayout.Vector3Field(" Tactical Position", motion.tacticalSprintPos);
            motion.tacticalSprintRot = EditorGUILayout.Vector3Field(" Tactical Rotation", motion.tacticalSprintRot);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateSway(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Sway = EditorGUILayout.Toggle(motion.Use_Sway, GUILayout.MaxWidth(28));
            Foldout_Sway = EditorGUILayout.Foldout(Foldout_Sway, "Sway", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Sway) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Sway);

            motion.swayTransform = EditorGUILayout.ObjectField(" Transform", motion.swayTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.swayTransform, "Tranform must be assgined in order to use this section.");


            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" Position", EditorStyles.boldLabel);
            motion.swayPositionAmount = EditorGUILayout.FloatField(" Amount", motion.swayPositionAmount);
            motion.swayPositonAimAmount = EditorGUILayout.FloatField(" Aim Amount", motion.swayPositonAimAmount);
            motion.swayPositionSmoothness = EditorGUILayout.FloatField(" Smoothness", motion.swayPositionSmoothness);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" Rotation", EditorStyles.boldLabel);
            motion.swayRotationAmount = EditorGUILayout.FloatField(" Amount", motion.swayRotationAmount);
            motion.swayRotationAimAmount = EditorGUILayout.FloatField(" Aim Amount", motion.swayRotationAimAmount);
            motion.swayRotationSmoothness = EditorGUILayout.FloatField(" Smoothness", motion.swayRotationSmoothness);


            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateHolding(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Holding = EditorGUILayout.Toggle(motion.Use_Holding, GUILayout.MaxWidth(28));
            Foldout_Holding = EditorGUILayout.Foldout(Foldout_Holding, "Holding", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Holding) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Holding);

            motion.holder = EditorGUILayout.ObjectField(" Shaker", motion.holder, typeof(CameraShaker), true) as CameraShaker;
            DrawMessage(MessageType.Error, motion.holder, "Tranform must be assgined in order to use this section.");


            motion.idleMagnitude = EditorGUILayout.FloatField(" Idle", motion.idleMagnitude);
            motion.movingMagnitude = EditorGUILayout.FloatField(" Moving", motion.movingMagnitude);
            motion.inAirMagnitude = EditorGUILayout.FloatField(" In Air", motion.inAirMagnitude);
            motion.inAirAimMagnitude = EditorGUILayout.FloatField(" In Air Aim Down Sight", motion.inAirAimMagnitude);
            motion.aimingMagnitude = EditorGUILayout.FloatField(" Aiming Down Sight", motion.aimingMagnitude);
            motion.holdingRoughness = EditorGUILayout.FloatField(" Roughness", motion.holdingRoughness);
            motion.holdingFadeTime = EditorGUILayout.FloatField(" Fade Time", motion.holdingFadeTime);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateObstacleAvoidnace(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_ObstacleAvoidnace = EditorGUILayout.Toggle(motion.Use_ObstacleAvoidnace, GUILayout.MaxWidth(28));
            Foldout_ObstacleAvoidnace = EditorGUILayout.Foldout(Foldout_ObstacleAvoidnace, "Obstacle Avoidnace", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_ObstacleAvoidnace) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_ObstacleAvoidnace);

            EditorGUI.BeginChangeCheck();

            var layermask = EditorGUILayout.MaskField(" Layer Mask", LayerMaskToField(motion.obstacleAvoidnaceLayermask), InternalEditorUtility.layers);

            if (EditorGUI.EndChangeCheck())
            {
                motion.obstacleAvoidnaceLayermask = FieldToLayerMask(layermask);
            }

            motion.obstacleAvoidnaceTransform = EditorGUILayout.ObjectField(" Transform", motion.obstacleAvoidnaceTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.obstacleAvoidnaceTransform, "Tranform must be assgined in order to use this section.");


            motion.obstacleAvoidnacePosition = EditorGUILayout.Vector3Field(" Position", motion.obstacleAvoidnacePosition);
            motion.obstacleAvoidnaceRotation = EditorGUILayout.Vector3Field(" Rotation", motion.obstacleAvoidnaceRotation);
            motion.obstacleAvoidnaceRange = EditorGUILayout.FloatField(" Range", motion.obstacleAvoidnaceRange);
            motion.obstacleAvoidnaceSmoothness = EditorGUILayout.FloatField(" Smoothness", motion.obstacleAvoidnaceSmoothness);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// converts the field value to layermask
        /// </summary>
        /// <param name="field">value to convert</param>
        /// <returns></returns>
        private LayerMask FieldToLayerMask(int field)
        {
            LayerMask mask = 0;
            var layers = InternalEditorUtility.layers;

            for (int i = 0; i < layers.Length; i++)
            {
                if ((field & (1 << i)) != 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[i]);
                }
            }

            return mask;
        }

        /// <summary>
        /// converts a layer mask to a field value
        /// </summary>
        /// <param name="mask">layer mask to convert</param>
        /// <returns></returns>
        private int LayerMaskToField(LayerMask mask)
        {
            int field = 0;
            var layers = InternalEditorUtility.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                if ((mask & (1 << LayerMask.NameToLayer(layers[i]))) != 0)
                {
                    field |= 1 << i;
                }
            }

            return field;
        }

        private void UpdateLeaning(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Leaning = EditorGUILayout.Toggle(motion.Use_Leaning, GUILayout.MaxWidth(28));
            Foldout_Leaning = EditorGUILayout.Foldout(Foldout_Leaning, "Leaning", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Leaning) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Leaning);

            motion.leaningTransform = EditorGUILayout.ObjectField(" Transform", motion.leaningTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.leaningTransform, "Tranform must be assgined in order to use this section.");


            motion.leaningOffsetRight = EditorGUILayout.Vector3Field(" Position Right", motion.leaningOffsetRight);
            motion.leaningOffsetLeft = EditorGUILayout.Vector3Field(" Rotation Left", motion.leaningOffsetLeft);
            motion.leaningRotationAngle = EditorGUILayout.FloatField(" Rotation Angle", motion.leaningRotationAngle);
            motion.leaningSmoothness = EditorGUILayout.FloatField(" Smoothness", motion.leaningSmoothness);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateLowerdPos(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_LowerdPos = EditorGUILayout.Toggle(motion.Use_LowerdPos, GUILayout.MaxWidth(28));
            Foldout_LowerdPos = EditorGUILayout.Foldout(Foldout_LowerdPos, "Lowerd Pos", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_LowerdPos) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_LowerdPos);

            motion.lowerdPosTransform = EditorGUILayout.ObjectField(" Transform", motion.lowerdPosTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.lowerdPosTransform, "Tranform must be assgined in order to use this section.");


            motion.lowerdPos = EditorGUILayout.Vector3Field(" Position", motion.lowerdPos);
            motion.lowerdRot = EditorGUILayout.Vector3Field(" Rotation", motion.lowerdRot);
            motion.lowerdSpeed = EditorGUILayout.FloatField(" Smoothness", motion.lowerdSpeed);
            motion.lowerdDelay = EditorGUILayout.FloatField(" Delay", motion.lowerdDelay);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void UpdateCrouch(FPSMotion motion)
        {
            EditorGUILayout.BeginHorizontal("box");
            motion.Use_Crouch = EditorGUILayout.Toggle(motion.Use_Crouch, GUILayout.MaxWidth(28));
            Foldout_Crouch = EditorGUILayout.Foldout(Foldout_Crouch, "Crouch", true);
            EditorGUILayout.EndHorizontal();

            if (!Foldout_Crouch) return;
            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginDisabledGroup(!motion.Use_Crouch);

            motion.crouchTransform = EditorGUILayout.ObjectField(" Transform", motion.crouchTransform, typeof(Transform), true) as Transform;
            DrawMessage(MessageType.Error, motion.crouchTransform, "Tranform must be assgined in order to use this section.");


            motion.crouchPos = EditorGUILayout.Vector3Field(" Position", motion.crouchPos);
            motion.crouchRot = EditorGUILayout.Vector3Field(" Rotation", motion.crouchRot);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
#endif
}