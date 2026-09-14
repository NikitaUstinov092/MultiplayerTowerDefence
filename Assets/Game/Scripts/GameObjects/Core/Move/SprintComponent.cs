using System;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    public sealed class SprintComponent : NetworkBehaviour
    {
        public event Action OnStateChanged;
        
        [Min(1f), SerializeField]
        private float _multiplier = 1.5f;
        
        [Networked, OnChangedRender(nameof(InvokeStateChanged))]
        public NetworkBool IsSprint { get; set; }

        public float Multiplier => this.IsSprint ? _multiplier : 1;
        
        private void InvokeStateChanged() => this.OnStateChanged?.Invoke();
    }
}