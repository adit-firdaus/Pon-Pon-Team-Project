using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPSFramework
{
    public class CameraManager : MonoBehaviour
    {
        [Header("FOV Kick")]
        public float FOVKick = 5;
        public float overlayFOVKick = 5;
        public float FOVKickSmoothness = 10;
        public Camera mainCamera;
        public Camera overlayCamera;

        [Header("Lean")]
        public float rotationAngle = 4;
        public float offset = 0.35f;
        public float smoothness = 10;

        [Header("Camera Shake")]
        public CameraShaker mainCameraShaker;
        public float mainCameraShakeMagnitude = 1.6f;
        public float cameraShakeRoughness = 7;
        public float cameraShakeFadeInTime = 0.2f;
        public float cameraShakeFadeOutTime = 2;

        [Header("Camera Recoil")]
        public float RecoilDampTime = 10f;
        public Vector3 RecoilAmount = new Vector3(-3f, 4f, 4f);

        [Header("Head Bob")]
        public float headbobAmount = 20;
        public float headbobRotationAmount = 30;

        private float headbobTimer;

        [HideInInspector] public ICharacterController controller;
        [HideInInspector] public AudioFiltersManager audioFiltersManager;
        private float movemnetPercentage;
        [HideInInspector] public float fieldOfView;
        [HideInInspector] public float overlayFieldOfView;

        #region a
        private float defaultFieldOfView;
        private float defaultOverlayFieldOfView;
        private float currentLeanAngle;
        private Vector3 leanRightPosition;
        private Vector3 leanLeftPosition;
        private Vector3 CurrentRecoil;


        public bool Use_FOVKick = true;
        public bool Use_Lean = true;
        public bool Use_CameraShake = true;
        public bool Use_CameraRecoil = true;
        public bool Use_Headbob = true;

        public bool Foldout_FOVKick;
        public bool Foldout_Lean;
        public bool Foldout_CameraShake;
        public bool Foldout_CameraRecoil;
        public bool Foldout_Headbob;
        #endregion

        public Vector3 ResultPosition
        {
            get
            {
                Vector3 result = Vector3.zero;
                return result += ResultLeanPosition + HeadbobPosition;
            }
        }

        public Vector3 ResultRotation
        {
            get
            {
                Vector3 result = Vector3.zero;
                return result += ResultLeanRotation + ResultRecoilRotation + HeadbobRotation;
            }
        }

        public Vector3 ResultLeanPosition { get; set; }
        public Vector3 ResultLeanRotation { get; set; }
        public Vector3 ResultRecoilRotation { get; set; }
        public Vector3 HeadbobPosition { get; set; }
        public Vector3 HeadbobRotation { get; set; }


        #region Logic
        private void Start()
        {
            controller = GetComponentInParent<ICharacterController>();
            audioFiltersManager = mainCamera.transform.GetComponent<AudioFiltersManager>();

            if (mainCamera)
            {
                fieldOfView = mainCamera.fieldOfView;
                defaultFieldOfView = mainCamera.fieldOfView;
            }

            if (overlayCamera)
            {
                overlayFieldOfView = overlayCamera.fieldOfView;
                defaultOverlayFieldOfView = overlayCamera.fieldOfView;
            }

            if (Use_Lean)
            {
                leanRightPosition = new Vector3(offset, 0, 0);
                leanLeftPosition = new Vector3(-offset, 0, 0);
            }
        }

        private void FixedUpdate()
        {
            if (!Use_CameraRecoil) return;
            CurrentRecoil = Vector3.Lerp(CurrentRecoil, Vector3.zero, 35 * Time.deltaTime);

            ResultRecoilRotation = Vector3.Slerp(ResultRecoilRotation, CurrentRecoil, RecoilDampTime * Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (controller != null)
            {
                movemnetPercentage = controller.GetCharacterController().velocity.magnitude / controller.sprintSpeed;
                movemnetPercentage = Mathf.Clamp(movemnetPercentage, 0, 1.3f);
            }

            if (mainCamera)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fieldOfView + FOVKickResult(), Time.deltaTime * FOVKickSmoothness);

            if (overlayCamera)
                overlayCamera.fieldOfView = Mathf.Lerp(overlayCamera.fieldOfView, overlayFieldOfView + OverlayFOVKickResult(), Time.deltaTime * FOVKickSmoothness);

            if (Input.GetKey(KeyCode.E) && Use_Lean)
            {
                ResultLeanPosition = Vector3.Lerp(ResultLeanPosition, leanRightPosition, Time.deltaTime * smoothness);
                currentLeanAngle = Mathf.Lerp(currentLeanAngle, -rotationAngle, Time.deltaTime * smoothness);
            }

            if (Input.GetKey(KeyCode.Q) && Use_Lean)
            {
                ResultLeanPosition = Vector3.Lerp(ResultLeanPosition, leanLeftPosition, Time.deltaTime * smoothness);
                currentLeanAngle = Mathf.Lerp(currentLeanAngle, rotationAngle, Time.deltaTime * smoothness);
            }

            if (!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q) && Use_Lean)
            {
                ResultLeanPosition = Vector3.Lerp(ResultLeanPosition, Vector3.zero, Time.deltaTime * smoothness);
                currentLeanAngle = Mathf.Lerp(currentLeanAngle, 0, Time.deltaTime * smoothness);
            }

            if (Use_Lean)
                ResultLeanRotation = new Vector3(0, 0, currentLeanAngle);

            if (Use_Headbob) UpdateHeadbob();

            transform.localPosition = ResultPosition;
            transform.localRotation = Quaternion.Euler(ResultRotation);
        }

        public void ApplyRecoil(float vertical, float horizontal, float shakeMultipler, bool isAiming = false)
        {
            if (!Use_CameraRecoil) return;

            float multipler = isAiming ? 1 : 0.7f;
            controller.LookAt(vertical * multipler, horizontal * multipler);
            CurrentRecoil += new Vector3(RecoilAmount.x, Random.Range(-RecoilAmount.y, RecoilAmount.y), Random.Range(-RecoilAmount.z, RecoilAmount.z)) * multipler * shakeMultipler;
        }

        public void ShakeCameras(float multipler)
        {
            if (!Use_CameraShake) return;
            if (mainCameraShaker)

                mainCameraShaker.Shake(mainCameraShakeMagnitude * multipler, cameraShakeRoughness, cameraShakeFadeInTime, cameraShakeFadeOutTime);
        }

        public void ShakeCameras(float multipler, float fadeOutTime)
        {
            if (!Use_CameraShake) return;

            if(mainCameraShaker)
            mainCameraShaker.Shake(mainCameraShakeMagnitude * multipler, cameraShakeRoughness, cameraShakeFadeInTime, fadeOutTime);
        }

        public void ShakeCameras(float multipler, float roughness, float fadeOutTime)
        {
            if (!Use_CameraShake) return;
            if (mainCameraShaker)

                mainCameraShaker.Shake(mainCameraShakeMagnitude * multipler, roughness, cameraShakeFadeInTime, fadeOutTime);
        }

        public void SetFieldOfView(float main, float overlay)
        {
            fieldOfView = main;
            overlayFieldOfView = overlay;
        }

        public void SetFieldOfView(float main, float overlay, float time)
        {
            fieldOfView = Mathf.Lerp(fieldOfView, main, time);
            overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, overlay, time);
        }

        public void SetFieldOfView(float main, float overlay, float timeMain, float timeOverlay)
        {
            fieldOfView = Mathf.Lerp(fieldOfView, main, timeMain);
            overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, overlay, timeOverlay);
        }

        public void ResetFieldOfView(float time)
        {
            fieldOfView = Mathf.Lerp(fieldOfView, defaultFieldOfView, time);
            overlayFieldOfView = Mathf.Lerp(overlayFieldOfView, defaultOverlayFieldOfView, time);
        }

        private float FOVKickResult()
        {
            if (!Use_FOVKick) return 0;

            float value;

            if (movemnetPercentage > 0.8f)
            {
                value = FOVKick * movemnetPercentage * movemnetPercentage;
            }
            else
            {
                value = 0;
            }

            return value;
        }

        private float OverlayFOVKickResult()
        {
            if (!Use_FOVKick) return 0;

            float value;

            if (movemnetPercentage > 0.8f)
            {
                value = overlayFOVKick * movemnetPercentage;
            }
            else
            {
                value = 0;
            }

            return value;
        }


        private void UpdateHeadbob()
        {
            headbobTimer += Time.deltaTime * controller.GetCharacterController().velocity.magnitude;
            float posX = 0f;
            float posY = 0f;
            float rotZ = 0;
            float multipler = controller.GetCharacterController().velocity.magnitude / controller.tacticalSprintSpeed;
            posX += ((headbobAmount / 100) / 2f * Mathf.Sin(headbobTimer) * multipler);
            posY += ((headbobAmount / 100) / 2f * Mathf.Sin(headbobTimer * 2f) * multipler);
            rotZ += ((headbobRotationAmount / 100) / 2 * Mathf.Sin(headbobTimer) * multipler);

            Vector3 posResult = new Vector3(posX, posY);
            Vector3 rotResult = new Vector3(0, 0, rotZ);

            if (!controller.GetCharacterController().IsVelocityZero() && controller.GetCharacterController().isGrounded)
            {
                HeadbobPosition = Vector3.Lerp(HeadbobPosition, posResult, Time.deltaTime * 5);
                HeadbobRotation = Vector3.Lerp(HeadbobRotation, rotResult, Time.deltaTime * 20);
            }
            else
            {
                HeadbobPosition = Vector3.Lerp(HeadbobPosition, Vector3.zero, Time.deltaTime * 5);
                HeadbobRotation = Vector3.Lerp(HeadbobRotation, Vector3.zero, Time.deltaTime * 5);
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraManager))]
    public class CameraManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUI.BeginChangeCheck();


            manager.mainCamera = EditorGUILayout.ObjectField(new GUIContent(" Main Camera", "the cam which is responsable for rendering the world."), manager.mainCamera, typeof(Camera), true) as Camera;

            if (manager.mainCamera)
            {
                manager.overlayCamera = EditorGUILayout.ObjectField(new GUIContent(" Overlay Camera", "the cam which is responsable for rendering anything in player is hand."), manager.overlayCamera, typeof(Camera), true) as Camera;
            }



            UpdateCameraRecoil();
            UpdateCameraShake();
            UpdateFOVKick();
            UpdateLean();
            UpdateHeadbob();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(manager);
                Undo.RecordObject(manager, "manager modified");
            }
        }

        private void UpdateFOVKick()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUILayout.BeginHorizontal("box");
            manager.Use_FOVKick = EditorGUILayout.Toggle(manager.Use_FOVKick, GUILayout.MaxWidth(28));
            manager.Foldout_FOVKick = EditorGUILayout.Foldout(manager.Foldout_FOVKick, "FOV Kick", true);
            EditorGUILayout.EndHorizontal();

            if (manager.Foldout_FOVKick)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginDisabledGroup(!manager.Use_FOVKick);

                if (manager.mainCamera)
                {
                    manager.FOVKick = EditorGUILayout.FloatField(new GUIContent(" Amount", "the amount of fov added to main camera while in high speed"), manager.FOVKick);

                    if (manager.overlayCamera)
                        manager.overlayFOVKick = EditorGUILayout.FloatField(new GUIContent(" Overlay Amount", "the amount of fov added to overlay while in high speed"), manager.overlayFOVKick);
                    else
                        EditorGUILayout.HelpBox(" Overlay Camera is null to use overlay camera FOV Kick please assign overlay camera.", MessageType.Info);

                    manager.FOVKickSmoothness = EditorGUILayout.FloatField(new GUIContent(" Roughness", "How rough fov kick is"), manager.FOVKickSmoothness);
                }
                else if (manager.Use_FOVKick)
                {
                    EditorGUILayout.HelpBox("Main Camera can't be null.", MessageType.Warning);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
        }

        private void UpdateLean()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUILayout.BeginHorizontal("box");
            manager.Use_Lean = EditorGUILayout.Toggle(manager.Use_Lean, GUILayout.MaxWidth(28));
            manager.Foldout_Lean = EditorGUILayout.Foldout(manager.Foldout_Lean, "Lean", true);
            EditorGUILayout.EndHorizontal();

            if (manager.Foldout_Lean)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginDisabledGroup(!manager.Use_Lean);


                manager.rotationAngle = EditorGUILayout.FloatField(new GUIContent(" Angle", "angle of leaning"), manager.rotationAngle);
                manager.offset = EditorGUILayout.FloatField(new GUIContent(" Offset", "offset of leaning on the right and left"), manager.offset);
                manager.smoothness = EditorGUILayout.FloatField(new GUIContent(" Smoothness", "the speed of leaning"), manager.smoothness);

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
        }

        private void UpdateHeadbob()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUILayout.BeginHorizontal("box");
            manager.Use_Headbob = EditorGUILayout.Toggle(manager.Use_Headbob, GUILayout.MaxWidth(28));
            manager.Foldout_Headbob = EditorGUILayout.Foldout(manager.Foldout_Headbob, "Head Bob", true);
            EditorGUILayout.EndHorizontal();

            if (manager.Foldout_Headbob)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginDisabledGroup(!manager.Use_Headbob);

                manager.headbobAmount = EditorGUILayout.FloatField(new GUIContent(" Amount", "amount of movement while moving"), manager.headbobAmount);
                manager.headbobRotationAmount = EditorGUILayout.FloatField(new GUIContent(" Rotation Amount", "amount of rotation movement while moving"), manager.headbobRotationAmount);


                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
        }

        private void UpdateCameraShake()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUILayout.BeginHorizontal("box");
            manager.Use_CameraShake = EditorGUILayout.Toggle(manager.Use_CameraShake, GUILayout.MaxWidth(28));
            manager.Foldout_CameraShake = EditorGUILayout.Foldout(manager.Foldout_CameraShake, "Camera Shake", true);
            EditorGUILayout.EndHorizontal();

            if (manager.Foldout_CameraShake)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginDisabledGroup(!manager.Use_CameraShake);

                manager.mainCameraShaker = EditorGUILayout.ObjectField(new GUIContent(" Shaker", "camera shaker which is responsable for shaking the camera."), manager.mainCameraShaker, typeof(CameraShaker), true) as CameraShaker;

                if (manager.mainCameraShaker)
                {
                    manager.mainCameraShakeMagnitude = EditorGUILayout.FloatField(new GUIContent(" Magnitude", "the amount of shake."), manager.mainCameraShakeMagnitude);
                    manager.cameraShakeRoughness = EditorGUILayout.FloatField(new GUIContent(" Roughness", "how hard the lerp is while shaking."), manager.cameraShakeRoughness);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(" Fade", EditorStyles.boldLabel);
                    manager.cameraShakeFadeInTime = EditorGUILayout.FloatField(new GUIContent(" In", "the speed of leaning"), manager.cameraShakeFadeInTime);
                    manager.cameraShakeFadeOutTime = EditorGUILayout.FloatField(new GUIContent(" Out", "the speed of leaning"), manager.cameraShakeFadeOutTime);

                }
                else if (manager.Use_CameraShake)
                {
                    EditorGUILayout.HelpBox("Shaker can't be null.", MessageType.Warning);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
        }

        private void UpdateCameraRecoil()
        {
            CameraManager manager = (CameraManager)target;

            EditorGUILayout.BeginHorizontal("box");
            manager.Use_CameraRecoil = EditorGUILayout.Toggle(manager.Use_CameraRecoil, GUILayout.MaxWidth(28));
            manager.Foldout_CameraRecoil = EditorGUILayout.Foldout(manager.Foldout_CameraRecoil, "Camera Recoil", true);
            EditorGUILayout.EndHorizontal();

            if (manager.Foldout_CameraRecoil)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginDisabledGroup(!manager.Use_CameraRecoil);

                manager.RecoilDampTime = EditorGUILayout.FloatField(new GUIContent(" Roughness", "how hard the lerp is while shaking."), manager.RecoilDampTime);
                manager.RecoilAmount = EditorGUILayout.Vector3Field(new GUIContent(" Amount", "the amount of rotation applied when recoil applied."), manager.RecoilAmount);


                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();
            }
        }

    }
#endif
}