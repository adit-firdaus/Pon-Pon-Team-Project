using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    [AddComponentMenu("Weapons/Projectile")]
    public class Projectile : MonoBehaviour
    {
        [Header("Base Settings")]
        public LayerMask hittableLayers;
        public Vector3Direction decalDirection = Vector3Direction.forward;
        public float penetrationStrenght = 100;
        public float speed = 50;
        public float gravity = 1;
        public float force = 10;
        public int lifeTime = 5;
        public GameObject defaultDecal;

        [Header("Additional Settings")]
        public bool destroyOnImpact = false;
        public bool useSourceVelocity = true;
        public bool useAutoScaling = true;
        public float scaleMultipler = 45;

        [Header("Range Control")]
        public float range = 300;
        public AnimationCurve damageRangeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });


        public Firearm source { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 shooterVelocity { get; set; }
        private float damage;
        private float damageRangeFactor;
        private float maxVelocity;
        private Vector3 velocity;
        private TrailRenderer trail;
        private Camera mainCamera;

        private Rigidbody rb;

        private Vector3 previousPosition;

        private Transform Effects;

        private Vector3 startPosition;

        private void Awake()
        {
            Setup();
        }

        public virtual void Setup()
        {
            previousPosition = transform.position;
            startPosition = transform.position;
            transform.localScale = Vector3.zero;

            FindComponents();

            if (trail && useAutoScaling) trail.widthMultiplier = 0;
        }

        public virtual void FindComponents()
        {
            trail = GetComponentInChildren<TrailRenderer>();
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Vector3 sorceVelocity = useSourceVelocity ? shooterVelocity : Vector3.zero;

            velocity = (transform.forward + direction) * (speed / 2) + sorceVelocity;

            rb.AddForce(velocity, ForceMode.VelocityChange);

            if (source && source.Inventory && source.Inventory.Controller != null && source.Inventory.Controller.GetCameraManager().mainCamera)
                mainCamera = source.Inventory.Controller.GetCameraManager().mainCamera;
            else mainCamera = Camera.main;

            maxVelocity = source.preset.muzzleVelocity;

            if (transform.Find("Effects"))
            {
                Effects = transform.Find("Effects");
                Effects.parent = null;
                Destroy(gameObject, lifeTime + 1);
            }

            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            float distanceFromStartPosition = Vector3.Distance(startPosition, transform.position);
            distanceFromStartPosition = Mathf.Clamp(distanceFromStartPosition, 0, range);

            damageRangeFactor = (rb.velocity.magnitude / maxVelocity) * (damageRangeCurve.Evaluate(distanceFromStartPosition / range));
            damage = (!source.preset.alwaysApplyFire ? source.preset.damage / source.preset.shotCount : source.preset.damage) * damageRangeFactor;


            RaycastHit[] hits = Physics.RaycastAll(previousPosition, -(previousPosition - transform.position), Vector3.Distance(transform.position, previousPosition));
            if (penetrationStrenght <= 0) Destroy(gameObject);

            for (int i = 0; i < hits.Length; i++)
            {
                if (penetrationStrenght > 0)
                {
                    RaycastHit hit = hits[i];
                    UpdateHits(hit);
                }
            }

            if (useAutoScaling && mainCamera)
            {
                float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
                float scale = (distance / scaleMultipler) * (mainCamera.fieldOfView / 360);

                transform.localScale = Vector3.one * scale;
                if (trail) trail.widthMultiplier = scale;
            }

            if (!useAutoScaling)
            {
                transform.localScale = Vector3.one * scaleMultipler;
            }

            if (Effects)
            {
                Effects.position = transform.position;
            }
        }

        private void FixedUpdate()
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        }

        private void LateUpdate()
        {
            previousPosition = transform.position;
        }

        private void UpdateHits(RaycastHit hit)
        {
            //stop if object has ignore component
            if (hit.transform.TryGetComponent(out IgnoreHitDetection ignore)) return;
            OnHit(hit);

            //setup hit info for hit detection without directly editing on the code but using IHitable interface
            HitInfo hitInfo = new HitInfo(this, hit);
            GameObject currentDecal = defaultDecal;

            if (source && source.Controller != null && source.Controller.transform == hit.transform) return;

            //calls OnHit() for any object with IHitable interface
            IOnHit onHit = hit.transform.GetComponent<IOnHit>();
            IOnHitInChildren onHitInChildren = hit.transform.GetComponentInChildren<IOnHitInChildren>();
            IOnHitInParent onHitInParent = hit.transform.GetComponentInParent<IOnHitInParent>();

            if (onHit != null) onHit.OnHit(hitInfo);
            if (onHitInChildren != null && onHit == null) onHitInChildren.IOnHitInChildren(hitInfo);
            if (onHitInParent != null && onHit == null) onHitInParent.OnHitInParent(hitInfo);

            if (hit.transform.TryGetComponent(out Explosive _explosive))
            {
                _explosive.source = source.Actor;
            }

            #region Decal And Projectile Health
            if (hit.transform.TryGetComponent(out CustomDecal customDecal))
            {
                penetrationStrenght -= customDecal.materialStrenght;
                currentDecal = customDecal.decalVFX;
            }
            else
            {
                penetrationStrenght -= 10;
            }
            #endregion

            HandleDamage(hit, hitInfo);
            AddDecal(hit, currentDecal, customDecal);
            ApplyImpactForce(hit);


            if (destroyOnImpact) Destroy(gameObject);
        }

        public virtual void OnHit(RaycastHit hit)
        {

        }

        public virtual void HandleDamage(RaycastHit hit, HitInfo hitInfo)
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                float damageResult = damage * damageRangeFactor * source.attachmentsManager.damage;
                if (hitInfo != null) hitInfo.damageReceived = damageResult;

                Actor sourceActor = null;

                if (source.Actor) sourceActor = source.Actor;
                damageable.Damage(damageResult, sourceActor);

                bool highDamage = damageable.GetHealth() <= damageable.MaxHealth * 0.3f ? true : false;

                if (UIManager.Instance && UIManager.Instance.Hitmarker)
                    UIManager.Instance.Hitmarker.Enable(highDamage, true);
            }

            if (hit.transform.TryGetComponent(out IDamageableGroup damageableGroup))
            {
                bool isAlive = damageableGroup.GetDamageable() != null ? !damageableGroup.GetDamageable().IsDead() : false;
                if (isAlive)
                {
                    Transform controllerTransform = null;
                    if (source && source.Controller != null) controllerTransform = source.Controller.transform;
                    if (hit.transform != controllerTransform)
                    {
                        float damageResult = damage * damageableGroup.GetDamageMultipler() * source.attachmentsManager.damage;

                        if (hitInfo != null) hitInfo.damageReceived = damageResult;

                        damageableGroup.GetDamageable()?.Damage(damage * damageableGroup.GetDamageMultipler(), source.Actor);

                        float force = source.preset.alwaysApplyFire ?
                            source.preset.impactForce / source.preset.shotCount : source.preset.impactForce;
                        damageableGroup.GetDamageable().deathForce = transform.forward * force;

                        UIManager.Instance.Hitmarker.Enable();
                    }
                }
            }

            #region Verify Kill
            if (damageableGroup != null && damageable == null)
            {
                if (damageableGroup != null && damageableGroup.GetDamageable() != null)
                {
                    if (!damageableGroup.GetDamageable().deadConfirmed && damageableGroup.GetDamageable().IsDead())
                    {
                        if (source.Actor == null) source.Actor = new Actor();

                        if (source)
                            source.Actor.OnKill?.Invoke(damageableGroup.GetDamageable().GetActor(), damageableGroup);

                        damageableGroup.GetDamageable().deadConfirmed = true;
                    }
                }
            }
            #endregion
        }

        public virtual void ApplyImpactForce(RaycastHit hit)
        {
            if (hit.rigidbody)
            {
                float force = source.preset.shotDelay <= 0 ? source.preset.impactForce / source.preset.shotCount : source.preset.impactForce;
                hit.rigidbody.AddForceAtPosition(transform.forward * force, transform.position, ForceMode.Impulse);
            }
        }

        public virtual void AddDecal(RaycastHit hit, GameObject currentDecal, CustomDecal customDecal)
        {
            if (source && source.Inventory && source.Inventory.Controller != null
                && hit.transform != source.Inventory.Controller.transform
                && currentDecal)
            {

                Vector3 position = hit.point;
                Quaternion rotation = MathUtilities.GetFromToRotation(hit, decalDirection);

                GameObject impact = Instantiate(currentDecal, position, rotation);
                impact.transform.localScale *= source.preset.decalSize;
                impact.transform.SetParent(hit.transform);

                float lifeTime = customDecal ? customDecal.lifeTime : 60;
                Destroy(impact, lifeTime);
            }
        }

        private void OnDestroy()
        {
            source?.Projectiles?.Remove(this);
        }
    }
}