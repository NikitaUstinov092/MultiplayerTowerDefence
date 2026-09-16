using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class SprintAnimComponent : NetworkBehaviour
    {
        private static readonly int SprintMultiplier = Animator.StringToHash(nameof(SprintMultiplier));

        [SerializeField]
        private SprintComponent _sprintComponent;
        [SerializeField]
        private Animator _animator;

        public override void Spawned()
        {
            _sprintComponent.OnStateChanged += this.OnStateChanged;
            this.OnStateChanged();
        }

        public override void Despawned(NetworkRunner runner, bool hasState) =>
            _sprintComponent.OnStateChanged -= this.OnStateChanged;

        private void OnStateChanged() =>
            _animator.SetFloat(SprintMultiplier, _sprintComponent.Multiplier);
    }
}