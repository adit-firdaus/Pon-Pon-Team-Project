using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class LaserSight : MonoBehaviour
    {
        public LayerMask hitableLayers;
        public Transform source;
        public Transform dot;
        public Transform range;
        public bool IsOn = true;

        private Vector3 normal;
        private Vector3 point;
        RaycastHit hit;

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.L)) IsOn = !IsOn;


            if (Physics.Raycast(transform.position, transform.forward, out hit, hitableLayers))
            {
                if (!hit.transform.TryGetComponent(out IgnoreLaserDetection ignore))
                {
                    Enable(hit);
                }
                else
                {
                    Disable();
                }
            }
            else
            {
                Disable();
            }
        }

        private void Enable(RaycastHit hit)
        {
            normal = hit.normal;
            point = hit.point;

            dot.gameObject.SetActive(true);
            dot.position = point;
            dot.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        }

        private void Disable()
        {
            dot.gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point, 0.4f);
        }
    }
}