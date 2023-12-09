using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

namespace FPSFramework
{
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        public HealthType type = HealthType.Other;
        public float health = 100;
        public float destroyDelay;
        [Range(0, 1)] public float damageCameraShake = 0.3f;

        [Space]
        public bool destoryOnDeath;
        public bool destroyRoot;
        public bool ragdolls;
        public GameObject deathEffect;

        [Space]
        public UnityEvent OnDeath;

        public Actor Actor { get; set; }
        public Ragdoll ragdoll { get; set; }
        private Actor killer;
        private DeathCamera deathCamera;
        public Vector3 deathForce { get; set; }
        public float MaxHealth { get; set; }
        public IDamageableGroup[] groups { get; set; }
        private bool died;
        public bool deadConfirmed { get; set; }

        private void Start()
        {
            Actor = GetComponent<Actor>();
            ragdoll = GetComponent<Ragdoll>();

            OnDeath.AddListener(Die);
            if (FindObjectOfType<GameManager>()) deathCamera = FindObjectOfType<GameManager>().DeathCamera;

            MaxHealth = health;

            if (type == HealthType.Humanoid)
            {
                if (deathCamera && Actor.Controller != null) deathCamera.Disable();

                groups = GetComponentsInChildren<IDamageableGroup>();

                if (Actor.Controller != null)
                {
                    if (UIManager.Instance && UIManager.Instance.HealthDisplay)
                    {
                        UIManager.Instance.HealthDisplay.UpdateHealth(health);
                        UIManager.Instance.HealthDisplay.actorNameText.text = Actor.Name;
                    }
                }
            }

            if (type == HealthType.Other)
            {
                if (ragdoll || Actor) Debug.LogWarning($"{this} has humanoid components and it's type is Other please change type to Humanoid to avoid errors.");
            }
        }

        private void UpdateSystem()
        {
            if (!died && health <= 0)
            {
                OnDeath?.Invoke();
            }

            UpdateUI(1);
        }

        public void Heal(float heal)
        {
            health += heal;
        }

        private void DoDamage(float damage, Actor killer)
        {
            health -= damage;
            this.killer = killer;

            if (type == HealthType.Humanoid && Actor.Controller != null)
            {
                Actor.Controller.GetCameraManager().ShakeCameras(damageCameraShake);
            }

            UpdateSystem();
        }

        private void UpdateUI(float alpha)
        {
            if (type == HealthType.Humanoid && Actor.Controller != null)
            {
                UIManager.Instance.DamageIndicator.Show(alpha);
                UIManager.Instance.HealthDisplay.UpdateHealth(health);
            }
        }

        private void Die()
        {
            if (type == HealthType.Humanoid)
            {
                if (Actor.ActorManager && Actor.ActorManager.respawnable) Actor.ConfirmDeath();
            }

            if (destoryOnDeath && !destroyRoot) Destroy(gameObject, destroyDelay);
            if (destoryOnDeath && destroyRoot) Destroy(gameObject.transform.parent.gameObject, destroyDelay);
            if (ragdoll) ragdoll.Enable(deathForce);

            if (!died) Respwan();

            if (deathEffect)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
                effect.SetActive(true);
            }

            died = true;
        }

        private void Respwan()
        {
            if (type != HealthType.Humanoid && !Actor) return;

            if (Actor.ActorManager && Actor.ActorManager.respawnable)
                Actor.ActorManager.Respwan(Actor.ActorManager.SpwanManager.respawnDelay);

            //if player enable death cam
            if (Actor.Controller != null) deathCamera.Enable(killer);
        }

        public float GetHealth()
        {
            return health;
        }

        public void Damage(float amount, Actor damageSource)
        {
            DoDamage(amount, damageSource);
        }

        public bool IsDead()
        {
            return health <= 0 ? true : false;
        }

        public Actor GetActor()
        {
            return Actor;
        }

        public int GetGroupsCount()
        {
            if (groups != null) return groups.Length;

            return 0;
        }

        public Ragdoll GetRagdoll()
        {
            return ragdoll;
        }
    }

    public enum HealthType
    {
        Humanoid = 0,
        Other = 1
    }
}