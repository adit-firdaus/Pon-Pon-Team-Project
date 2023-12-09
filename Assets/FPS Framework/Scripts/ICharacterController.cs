using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace FPSFramework
{
    public interface ICharacterController
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public CharacterController GetCharacterController();
        public CameraManager GetCameraManager();
        public void SetSpeed(float walkSpeed, float runSpeed, float tacticalSprintSpeed);
        public void ResetSpeed();
        public Actor Actor { get; set; }
        public GameObject GetAudioHolder();
        public bool IsSprinting();
        public Transform GetOrientation();
        public void LookAt(float vertical, float horizontal);
        public bool IsCrouching();
        public bool IsTacticalSprinting();
        public float sprintSpeed { get; }
        public float walkSpeed { get; }
        public float tacticalSprintSpeed { get; }
        public float tacticalSprintAmount { get; }
        public bool canTacticalSprint { get; }
        public bool MaxedCameraRotation();
        public float MouseX { get; set; }
        public float MouseY { get; set;}

        public UnityEvent OnJump { get; }
        public UnityEvent OnLand { get;  }

        public void EnableInput();
        public void DisableInput();

        public void EnableKeyboardInput();
        public void DisableKeyboardInput();

        public void EnableMouseInput();
        public void DisableMouseInput();

        public bool keyboardInputEnabled { get; set; }

        public bool mouseInputEnabled { get; set; }

        public bool inputEnabled { get; set; }
    }
}