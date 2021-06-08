using System;
using UnityEngine;
using UnityEngine.Events;
using GameFramework.UnityEngine.EventsExtension;

namespace GameFramework.Components.ForUI
{
    [Serializable]
    public class DamageEvent : UnityEvent<int, IAttackable> { }

    public class Health : MonoBehaviour, IHealth
    {
        #region UnityEvents

        public DamageEvent OnTakenDamage;
        public UnityEvent OnDie;
        public UnityEventInt OnHealthChanged;

        #endregion

        #region Propertyes

        public int CurrentHealth
        {
            get => currentHealth;
            private set
            {
                if (Mathf.Abs(currentHealth - value) > 0)
                    currentHealth = value;
                if (OnHealthChanged != null)
                    OnHealthChanged.Invoke(currentHealth);
            }
        }

        public int MaxHealth => maxHealth;
        public float PercentageOfHealth => (currentHealth / maxHealth);
        public bool IsAlive => currentHealth > 0;
        public bool IsImmortal => isImmortal;

        #endregion

        #region SerializePrivateFields

        [SerializeField] private bool isImmortal = false;
        [SerializeField] private int currentHealth = 100;
        [SerializeField] private int maxHealth = 100;

        #endregion

        #region PublicInterfaceInteraction

        public void SetupComponent(int currentHealth, int maxHealth)
        {
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
        }

        public void AddHealthPoint(int healthPoint)
            => CurrentHealth += healthPoint;

        public void TakeDamage(int damage, IAttackable attackable)
        {
            if (isImmortal || currentHealth <= 0)
                return;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            if (OnTakenDamage != null)
                OnTakenDamage.Invoke(damage, attackable);

            if (CurrentHealth <= 0 && OnDie != null)
                OnDie.Invoke();
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void OnDestroy()
        {
            RemoveAllEventsListeners();
        }

        #endregion

        #region SetupEvents

        public void RemoveAllEventsListeners()
        {
            OnDie.RemoveAllListeners();
            OnHealthChanged.RemoveAllListeners();
            OnTakenDamage.RemoveAllListeners();
        }

        #endregion
    }
}

