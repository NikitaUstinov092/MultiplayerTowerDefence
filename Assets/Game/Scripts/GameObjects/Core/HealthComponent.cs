using System;
using Fusion;
using UnityEngine;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class HealthComponent : NetworkBehaviour
    {
        public interface IDamageCondition
        {
            bool IsMet(NetworkObject attacker);
        }

        public delegate void HealthChangedHandler(int previous, int current);

        public event HealthChangedHandler OnHealthChanged;
        public event Action OnDamageTaken;
        public event Action OnDied;

        private static PropertyReader<int> s_healthReader =
            GetPropertyReader<int>(typeof(HealthComponent), nameof(Current));

        [Networked, OnChangedRender(nameof(InvokeHealthChanged))] // Client Side Prediction (Input Authority)
        public int Current { get; set; } = 5; // NetworkBuffer<int>

        [field: SerializeField]
        public int Max { get; set; } = 10; // Const

        public bool IsDead => Current <= 0;

        public bool IsAlive => Current > 0;

        public bool IsNotFull => Current < Max;
        public float Progress => (float) Current / Max;

        [Networked]
        private ushort _takeDamageEvents { get; set; } // 65000

        private ushort _localTakeDamageEvents;

        private IDamageCondition _condition;

        public void SetDamageCondition(IDamageCondition condition)
        {
            _condition = condition;
        }

        public bool CanBeDamagedBy(NetworkObject attacker)
        {
            // Без TeamComponent объект не может быть целью ни для поиска, ни для урона (например, Portal)
            if (!Object.TryGetBehaviour(out TeamComponent _))
                return false;

            var result = _condition == null || _condition.IsMet(attacker);
            return result;
        }

        public override void Spawned()
        {
            _localTakeDamageEvents = _takeDamageEvents;
        }

        public override void Render()
        {
            while (_localTakeDamageEvents < _takeDamageEvents)
            {
                OnDamageTaken?.Invoke();
                _localTakeDamageEvents++;
            }
        }

        public void Restore(int heal)
        {
            if (heal <= 0)
                return;

            Current = Math.Min(Max, Current + heal);
        }

        // Смерть без урона: не увеличивает _takeDamageEvents, чтобы не проигрывалась анимация попадания.
        public void Kill()
        {
            if (!HasStateAuthority || IsDead)
                return;

            Current = 0;
            OnDied?.Invoke();
        }

        public void TakeDamage(int damage)
        {
            // Урон считает только сервер: снаряды симулируются и на клиенте-владельце (предикт),
            // а запись в HP чужих прокси-объектов на клиенте даёт мерцание до прихода снапшота.
            if (!HasStateAuthority || damage <= 0 || IsDead)
                return;

            Current = Math.Max(0, Current - damage);
            _takeDamageEvents++;

            if (IsDead)
                OnDied?.Invoke();
        }

        public void TakeDamage(int damage, NetworkObject attacker)
        {
            if (!CanBeDamagedBy(attacker))
                return;

            TakeDamage(damage);
        }

        // Render()
        private void InvokeHealthChanged(NetworkBehaviourBuffer previousSnapshot)
        {
            int previousHealth = s_healthReader.Read(previousSnapshot);
            OnHealthChanged?.Invoke(previousHealth, Current);
        }
    }
}