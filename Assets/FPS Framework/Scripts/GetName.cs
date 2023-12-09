using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace FPSFramework
{
    [ExecuteAlways, RequireComponent(typeof(TextMeshProUGUI)), AddComponentMenu("UI/Get Name")]
    public class GetName : MonoBehaviour
    {
        public Transform target;
        private TextMeshProUGUI text;

        private void Start()
        {
            if (!target) target = transform.parent;
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            text.text = target.name;
        }
    }
}