using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPSFramework
{
    [AddComponentMenu("UI/Floating Rect")]
    public class FloatingRect : MonoBehaviour
    {
        public TargetingType targetingType;
        public Camera main;
        public Transform target;
        public Vector3 position;

        private float dotProduct;
        private RectTransform rectTransform;

        private void Start()
        {
            main = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            var pos = targetingType == TargetingType.Transform ? target.position : position;

            Vector2 targetPos = main.WorldToScreenPoint(pos);

            dotProduct = Vector3.Dot((pos - main.transform.position), main.transform.forward);

            if(dotProduct < 0)
            {
                if(targetPos.x < Screen.width / 2)
                {
                    targetPos.x = Screen.width;
                }
                else
                {
                    targetPos.x = 0;
                }
            }

            targetPos.x = Mathf.Clamp(targetPos.x, 0, Screen.width);
            targetPos.y = Mathf.Clamp(targetPos.y, 0, Screen.height);

            rectTransform.position = targetPos;
        }

        public enum TargetingType
        {
            Transform,
            Position
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FloatingRect))]
    public class FloatingRectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FloatingRect rect = (FloatingRect)target;

            EditorGUI.BeginChangeCheck();

            rect.targetingType = (FloatingRect.TargetingType)EditorGUILayout.EnumPopup(new GUIContent("Targeting Type", "Does the rect follow a transform or position"), rect.targetingType);

            if (rect.targetingType == FloatingRect.TargetingType.Transform)
            {
                rect.target = EditorGUILayout.ObjectField(new GUIContent("Target", "Target transform."), rect.target, typeof(Transform), true) as Transform;
            }

            if (rect.targetingType == FloatingRect.TargetingType.Position)
            {
                rect.position = EditorGUILayout.Vector3Field(new GUIContent("Target Position"), rect.position);
            }

            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(rect, "floating rect modified");
                EditorUtility.SetDirty(rect);
            }
        }
    }
#endif
}