using System;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class HealthComponent : NetworkBehaviour
    {
        public interface IDamageCondition
        {
            bool IsMet(PlayerRef attacker);
        }

        public delegate void HealthChangedHandler(int previous, int current);

        public event HealthChangedHandler OnHealthChanged;
        public event Action OnDamageTaken;

        private static PropertyReader<int> s_healthReader =
            GetPropertyReader<int>(typeof(HealthComponent), nameof(Current));

        [Networked, OnChangedRender(nameof(InvokeHealthChanged))] // Client Side Prediction (Input Authority)
        public int Current { get; set; } = 5; // NetworkBuffer<int>

        [field: SerializeField]
        public int Max { get; set; } = 10; // Const

        public bool IsDead => this.Current <= 0;

        public bool IsAlive => this.Current > 0;

        public bool IsNotFull => this.Current < this.Max;
        public float Progress => (float) this.Current / this.Max;

        [Networked]
        private ushort _takeDamageEvents { get; set; } // 65000

        private ushort _localTakeDamageEvents;

        private IDamageCondition _condition;

        public void SetDamageCondition(IDamageCondition condition)
        {
            _condition = condition;
        }

        public bool CanBeDamagedBy(PlayerRef attacker) => _condition == null || _condition.IsMet(attacker);

        public override void Spawned()
        {
            _localTakeDamageEvents = _takeDamageEvents;
        }

        public override void Render()
        {
            while (_localTakeDamageEvents < _takeDamageEvents)
            {
                this.OnDamageTaken?.Invoke();
                _localTakeDamageEvents++;
            }
        }

        public void Restore(int heal)
        {
            if (heal <= 0)
                return;

            this.Current = Math.Min(this.Max, this.Current + heal);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || this.IsDead)
                return;

            this.Current = Math.Max(0, this.Current - damage);
            _takeDamageEvents++;
        }

        public void TakeDamage(int damage, PlayerRef attacker)
        {
            if (!this.CanBeDamagedBy(attacker))
                return;

            this.TakeDamage(damage);
        }

        // Render()
        private void InvokeHealthChanged(NetworkBehaviourBuffer previousSnapshot)
        {
            int previousHealth = s_healthReader.Read(previousSnapshot);
            this.OnHealthChanged?.Invoke(previousHealth, this.Current);
        }
    }
}