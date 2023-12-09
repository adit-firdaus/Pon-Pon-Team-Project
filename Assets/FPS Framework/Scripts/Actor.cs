using UnityEngine;
using System;

namespace FPSFramework
{
    [DisallowMultipleComponent()]
    public class Actor : MonoBehaviour
    {
        public ActorType type;
        public string Name;

        public ActorManager ActorManager { get; set; }
        public ICharacterController Controller { get; set; }
        public IDamageable HealthSystem { get; set; }
        public Action<Actor, IDamageableGroup> OnKill;

        public Inventory Inventory { get; private set; }

        private void Awake()
        {
            ActorManager = GetComponentInParent<ActorManager>();
            Inventory = transform.SearchFor<Inventory>();
            Controller = GetComponent<ICharacterController>();
            HealthSystem = GetComponent<IDamageable>();
        }

        private void Start()
        {
            OnKill = ConfirmKill;
            if (ActorManager) ActorManager.actor = this;

            if (UIManager.Instance && Controller != null)
            {
                UIManager.Instance.HealthDisplay.UpdateHealthNoLerp(HealthSystem.GetHealth());
                UIManager.Instance.HealthDisplay.slider.maxValue = HealthSystem.GetHealth();
                UIManager.Instance.HealthDisplay.backgroundSlider.maxValue = HealthSystem.GetHealth();
                UIManager.Instance.HealthDisplay.actorNameText.text = Name;
            }
        }

        private void Update()
        {
            if (UIManager.Instance && Controller != null)
                UIManager.Instance.HealthDisplay.UpdateHealth(HealthSystem.GetHealth());
        }

        public void ConfirmKill(Actor victim, IDamageableGroup group)
        {
            if(ActorManager)
            ActorManager.kills++;

            if (UIManager.Instance)
            {
                UIManager.Instance.KillFeed.Show(this, victim.Name, group.GetBone() == HumanBodyBones.Head ? true : false);
                UIManager.Instance.Hitmarker.Enable(true, true, UIManager.Instance.Hitmarker.maxSize);
            }
        }

        public void ConfirmDeath()
        {
            if(ActorManager)
            ActorManager.deaths++;
        }
    }

    public enum ActorType
    {
        Human,
        Bot
    }
}