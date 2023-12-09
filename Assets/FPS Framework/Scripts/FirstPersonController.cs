using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace FPSFramework
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour, ICharacterController
    {
        [Header("Movement")]
        [Tooltip("The amount of time needed to walk or sprint in full speed.")]
        public float acceleration = 0.1f;
        [Tooltip("The amount of meters to move per second while walking.")]
        public float walkSpeed = 5;
        [Tooltip("The amount of meters to move per second while sprinting.")]
        public float sprintSpeed = 10;
        [Tooltip("The amount of meters to move per second while tactical walking.")]
        public float tacticalSprintSpeed = 11;
        [Tooltip("The amount of force applied when jumping.")]
        public float jumpHeight = 6;
        [Tooltip("Player height while crouching.")]
        public float crouchHeight = 1.5f;
        [Tooltip("The amount of update calles in order to perform one step.")]
        public float stepInterval = 7;

        [Header("Slopes")]
        public bool slideDownSlopes = true;
        public float slopeSlideSpeed = 1;

        [Space]
        [Tooltip("Force multiplier from Physics/Gravity.")]
        public float gravity = 1;
        [Tooltip("Max speed the player can reach while falling")]
        public float maxFallSpeed = 350;
        [Tooltip("Force multiplier from Physics/Gravity when grounded")]
        public float stickToGroundForce = 0.5f;

        [Header("Camera")]
        [Tooltip("Camera or camera holder which will rotate when rotating view.")]
        public Transform _Camera;
        [Tooltip("Sensitivity of camera movement.")]
        public float sensitivity = 200;
        [Tooltip("Max angle of view rotation.")]
        public float maximumX = 90f;
        [Tooltip("Min angle of view rotation.")]
        public float minimumX = -90f;
        [Tooltip("Camera offset from the player.")]
        public Vector3 offset = new Vector3(0, -0.2f, 0);
        [Tooltip("Changes camera sens dynamicly change with camera field of view.")]
        public bool dynamicSensitivity = true;
        [Tooltip("Locks and reset cursor on start")]
        public bool lockCursor = true;
        public bool globalOrientation = false;

        [Header("Audio")]
        [Tooltip("(optional) Footsteps list to play a random sound clip from while walking.")]
        public AudioProfile[] footstepsSFX;
        [Tooltip("(optional) Sound of jumping.")]
        public AudioProfile jumpSFX;
        [Tooltip("(optional) Sound of landing.")]
        public AudioProfile landSFX;

        [Space]

        public UnityAction action;
        [Tooltip("What to do on jump.")]
        public UnityEvent OnJump = new UnityEvent();
        [Tooltip("What to do on land.")]
        public UnityEvent OnLand = new UnityEvent();

        public CollisionFlags CollisionFlags { get; set; }
        public CharacterController controller { get; set; }
        public CameraManager cameraManager { get; set; }
        public Actor Actor { get; set; }


        //input velocity
        private Vector3 desiredVelocityRef;
        private Vector3 desiredVelocity;
        private Vector3 slideVelocity;

        //out put velocity
        private Vector3 velocity;

        public Transform Orientation { get; set; }
        public GameObject AudioHolder { get; set; }

        public float horizontal { get; set; }
        public float vertical { get; set; }
        public float MouseX { get; set; }
        public float MouseY { get; set; }
        public float tacticalSprintAmount { get; set; }
        private bool IsTacticalSprinting { get; set; }
        public bool canTacticalSprint { get; set; }
        float ICharacterController.sprintSpeed { get => sprintSpeed; }
        float ICharacterController.walkSpeed { get => walkSpeed; }
        float ICharacterController.tacticalSprintSpeed { get => tacticalSprintSpeed; }
        UnityEvent ICharacterController.OnJump { get => OnJump; }
        UnityEvent ICharacterController.OnLand { get => OnLand; }
        public bool keyboardInputEnabled { get; set; }
        public bool mouseInputEnabled { get; set; }
        public bool inputEnabled { get; set; }

        private Vector3 slopeDirection;

        private float lookAtVertical;
        private float lookAtHorizontal;

        private float yRotation;
        private float xRotation;

        private float speed;
        private float outputWalkSpeed;
        private float outputSprintSpeed;
        private float outputTacticalSprintSpeed;
        private float defaultHeight;
        private float defaultstepOffset;

        private bool previouslyGrounded;
        private float stepCycle;
        private float nextStep;

        private float lastTacticalSprintTime;
        private float currentTacticalSprintTime;

        public virtual void Awake()
        {
            EnableInput();
            EnableKeyboardInput();
            EnableMouseInput();

            if (GetComponentInChildren<Inventory>()) GetComponentInChildren<Inventory>().Controller = this;

            if (transform.Find("Orientation") != null)
            {
                Orientation = transform.Find("Orientation");
            }
            else
            {
                Orientation = new GameObject("Orientation").transform;
                Orientation.parent = transform;
                Orientation.localPosition = Vector3.zero;
                Orientation.localRotation = Quaternion.identity;
            }

            if (transform.Find("Audio Holder"))
            {
                AudioHolder = transform.Find("Audio Holder").gameObject;
            }
            else
            {
                AudioHolder = new GameObject("Audio Holder");
                AudioHolder.transform.parent = Orientation;
                AudioHolder.transform.localPosition = Vector3.zero;
                AudioHolder.transform.localRotation = Quaternion.identity;
            }
        }

        public virtual void Start()
        {
            if (!_Camera) _Camera = GetComponentInChildren<Camera>().transform;

            //setup nesscary values
            controller = GetComponent<CharacterController>();
            previouslyGrounded = controller.isGrounded;

            ResetSpeed();

            //get defaults
            defaultHeight = controller.height;
            defaultstepOffset = controller.stepOffset;
            controller.skinWidth = controller.radius / 10;

            //hide and lock cursor if there is no pause menu in the scene
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Actor = GetComponent<Actor>();
            cameraManager = GetComponentInChildren<CameraManager>();
            if (cameraManager)
            {
                cameraManager.controller = this;
            }

            //setup audio
            if (footstepsSFX != null)
            {
                foreach (AudioProfile profile in footstepsSFX)
                {
                    AudioManager.EquipAudio(AudioHolder, profile);
                }
            }

            if (jumpSFX)
                AudioManager.EquipAudio(AudioHolder, jumpSFX);
            if (landSFX)
                AudioManager.EquipAudio(AudioHolder, landSFX);

            currentTacticalSprintTime = 10;

            OnLand.AddListener(PlayLandSFX);
        }


        public virtual void Update()
        {
            if (!previouslyGrounded && controller.isGrounded) OnLand?.Invoke();
            if (previouslyGrounded && !controller.isGrounded) OnJump?.Invoke();

            //copy is grounded to previouslyGrounded
            previouslyGrounded = controller.isGrounded;

            //update inputs
            GetInput();

            //slide down slope if on maxed angle slope
            if (slideDownSlopes && OnMaxedAngleSlope()) 
                slideVelocity += new Vector3(slopeDirection.x, -slopeDirection.y, slopeDirection.z) * slopeSlideSpeed * Time.deltaTime;
            else
                //reset velocity if not on slope
                slideVelocity = Vector3.zero;

            //update desiredVelocity in order to normlize it and smooth the movement
            desiredVelocity = slideVelocity + Vector3.SmoothDamp(desiredVelocity,
                (SlopeDirection() * vertical + Orientation.right * horizontal).normalized * speed, ref desiredVelocityRef, acceleration);

            //set controller height according to if player is crouching
            controller.height = IsCrouching() ?
            Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * 15) :
            Mathf.Lerp(controller.height, defaultHeight, Time.deltaTime * 15);

            if (!controller.isGrounded || OnSlope())
            {
                controller.stepOffset = 0;
            }
            else
            {
                controller.stepOffset = defaultstepOffset;
            }

            //copy desiredVelocity x, z with normlized values
            velocity.x = (desiredVelocity.x);
            velocity.z = (desiredVelocity.z);

            //update speed according to if player is holding sprint
            if (IsSprinting() && !IsTacticalSprinting) speed = outputSprintSpeed;
            else if (!IsTacticalSprinting) speed = outputWalkSpeed;

            if (IsTacticalSprinting) speed = outputTacticalSprintSpeed;

            //update gravity and jumping
            if (controller.isGrounded)
            {
                //set small force when grounded in order to staplize the controller
                velocity.y = Physics.gravity.y * stickToGroundForce;

                //check jumping input
                if (Input.GetKey(KeyCode.Space))
                {
                    //update velocity in order to jump
                    velocity += jumpHeight * Vector3.up + (-Physics.gravity * gravity * stickToGroundForce);

                    //play jump sound
                    if (jumpSFX)
                        AudioManager.PlayOneShot(jumpSFX);
                }
            }
            else if (velocity.magnitude * 3.5f < maxFallSpeed)
            {
                //add gravity
                velocity += Physics.gravity * gravity * Time.deltaTime;
            }

            //move and update CollisionFlags in order to check if collition is coming from above ot center or bottom
            CollisionFlags = controller.Move(velocity * Time.deltaTime);

            //move camera according to controller height
            _Camera.position = transform.position + ((transform.up * controller.height / 2) + offset);

            //rotate camera
            UpdateCameraRotation();
        }

        public void PlayLandSFX()
        {
            if (landSFX)
                AudioManager.PlayOneShot(landSFX);
        }

        public virtual void FixedUpdate()
        {
            //update step sounds
            ProgressStepCycle(speed);
        }

        public virtual void GetInput()
        {
            GetkeyboardInput();

            if (IsTacticalSprinting) currentTacticalSprintTime -= Time.deltaTime;
            else if (currentTacticalSprintTime < 10) currentTacticalSprintTime += Time.deltaTime;

            if (currentTacticalSprintTime <= 0) canTacticalSprint = false;
            if (currentTacticalSprintTime > 0 && currentTacticalSprintTime <= 10) canTacticalSprint = true;

            currentTacticalSprintTime = Mathf.Clamp(currentTacticalSprintTime, 0, 10);
            tacticalSprintAmount = IsTacticalSprinting ? 1 : 0;

            GetMouseInput();
        }

        public virtual void GetMouseInput()
        {
            if (!inputEnabled) return;
            if (!mouseInputEnabled) return;

            if (cameraManager)
            {
                if (PauseMenu.Instance)
                {
                    float dynamicSensitivity = !PauseMenu.Instance.paused ? Time.fixedDeltaTime * (sensitivity * (cameraManager.mainCamera.fieldOfView / 179)) : 0;
                    MouseX = lookAtVertical + Input.GetAxisRaw("Mouse X") * dynamicSensitivity;
                    MouseY = lookAtHorizontal + Input.GetAxisRaw("Mouse Y") * dynamicSensitivity;
                }
                else
                {
                    float dynamicSensitivity = Time.fixedDeltaTime * (sensitivity * (cameraManager.mainCamera.fieldOfView / 179));
                    MouseX = lookAtVertical + Input.GetAxisRaw("Mouse X") * dynamicSensitivity;
                    MouseY = lookAtHorizontal + Input.GetAxisRaw("Mouse Y") * dynamicSensitivity;
                }
            }
            else
            {
                MouseX = lookAtVertical + Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensitivity;
                MouseY = lookAtHorizontal + Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensitivity;
            }
        }

        public virtual void GetkeyboardInput()
        {
            if (!inputEnabled) return;
            if (!keyboardInputEnabled) return;

            //update horiz and vert inputs
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            //update tac sprint input
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                float tacSprintClickTime = Time.time - lastTacticalSprintTime;

                if (tacSprintClickTime <= 0.3f && canTacticalSprint)
                {
                    IsTacticalSprinting = true;
                }


                lastTacticalSprintTime = Time.time;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || !canTacticalSprint)
            {
                IsTacticalSprinting = false;
            }
        }

        private void ProgressStepCycle(float speed)
        {
            //stop if not grounded
            if (!controller.isGrounded || footstepsSFX.Length <= 0) return;

            //check if taking input and input
            if (controller.velocity.sqrMagnitude > 0 && (horizontal != 0 || vertical != 0))
            {
                //update step cycle
                stepCycle += (controller.velocity.magnitude + (speed * (!GetCharacterController().IsVelocityZero() ? 1f : 1))) * Time.fixedDeltaTime;
            }

            //check step cycle not equal to next step in order to update right
            if (!(stepCycle > nextStep))
            {
                return;
            }

            //update
            nextStep = stepCycle + stepInterval;

            if (footstepsSFX != null)
                AudioManager.PlayOneShot(footstepsSFX[Random.Range(0, footstepsSFX.Length)]);
        }

        private void UpdateCameraRotation()
        {
            if (prevCamRotation != _Camera.rotation) OnCameraRotationUpdated();

            lookAtHorizontal = 0;
            lookAtVertical = 0;

            yRotation += MouseX;
            xRotation -= MouseY;

            xRotation = Mathf.Clamp(xRotation, minimumX, maximumX);
            Quaternion cameraRotation = Quaternion.Euler(xRotation, yRotation, 0);
            Quaternion playerRotation = Quaternion.Euler(0, yRotation, 0);

            Orientation.SetRotation(playerRotation, !globalOrientation);
            _Camera.SetRotation(cameraRotation, !globalOrientation);

            prevCamRotation = _Camera.rotation;
        }

        private Quaternion prevCamRotation;

        public virtual void OnCameraRotationUpdated() { }

        public void LookAt(float vertical, float horizontal)
        {
            lookAtHorizontal += vertical;
            lookAtHorizontal += horizontal;
        }
        public bool IsCrouching()
        {
            //set IsCrouching to true if holding crouch key else set it to false
            return Input.GetKey(KeyCode.C) && !IsTacticalSprinting && controller.isGrounded;
        }

        public bool IsSprinting()
        {
            //set IsSprinting to true if holding sprint key and moving forward and not sprinting else set it to false
            return Input.GetKey(KeyCode.LeftShift) && vertical > 0 && !IsCrouching();
        }

        public bool OnSlope()
        {
            //check if slope angle is more than 0
            if (SlopeAngle() > 0)
            {
                return true;
            }

            return false;
        }

        public bool OnMaxedAngleSlope()
        {
            if (controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height))
            {
                slopeDirection = hit.normal;
                return Vector3.Angle(slopeDirection, Vector3.up) > controller.slopeLimit;
            }

            return false;
        }

        public Vector3 SlopeDirection()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;
            if (Physics.Raycast(Orientation.position, Vector3.down, out slopeHit, (controller.height / 2) + 0.1f))
            {
                //get the direction result according to slope normal
                return Vector3.ProjectOnPlane(Orientation.forward, slopeHit.normal);
            }

            //if not on slope then slope is forward ;)
            return Orientation.forward;
        }

        public float SlopeAngle()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
            {
                //get the direction result according to slope normal
                return (Vector3.Angle(Vector3.down, slopeHit.normal) - 180) * -1;
            }

            //if not on slope then slope is forward ;)
            return 0;
        }

        public void SetSpeed(float walk, float sprint, float tacSprint)
        {
            outputWalkSpeed = walk;
            outputSprintSpeed = sprint;
            outputTacticalSprintSpeed = tacSprint;
        }

        public void ResetSpeed()
        {
            outputWalkSpeed = walkSpeed;
            outputSprintSpeed = sprintSpeed;
            outputTacticalSprintSpeed = tacticalSprintSpeed;
        }

        public bool MaxedCameraRotation()
        {
            return xRotation < -90 + 1 || xRotation > 90 - 1;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //if hit something while jumping from the above then go down again
            if (CollisionFlags == CollisionFlags.Above)
            {
                velocity.y = 0;
            }
        }

        private void OnDestroy()
        {
            OnJump.RemoveAllListeners();
            OnLand.RemoveAllListeners();
        }

        public CharacterController GetCharacterController()
        {
            return controller;
        }

        public CameraManager GetCameraManager()
        {
            return cameraManager;
        }

        public GameObject GetAudioHolder()
        {
            return AudioHolder;
        }

        public Transform GetOrientation()
        {
            return Orientation;
        }

        bool ICharacterController.IsTacticalSprinting()
        {
            return IsTacticalSprinting;
        }

        public void EnableInput()
        {
            inputEnabled = true;
        }

        public void DisableInput()
        {
            inputEnabled = false;
        }

        public void EnableKeyboardInput()
        {
            keyboardInputEnabled = true;
        }

        public void DisableKeyboardInput()
        {
            keyboardInputEnabled = false;
        }

        public void EnableMouseInput()
        {
            mouseInputEnabled = true;
        }

        public void DisableMouseInput()
        {
            mouseInputEnabled = false;
        }
    }
}