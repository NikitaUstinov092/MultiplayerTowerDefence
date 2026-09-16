using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace SampleGame
{
    [CreateAssetMenu(
        fileName = "ProjectileCatalog",
        menuName = "SampleGame/Projectiles/New ProjectileCatalog"
    )]
    public sealed class ProjectileCatalog : ScriptableObject, 
        IReadOnlyCollection<KeyValuePair<ProjectileType, ProjectileConfig>>
    {
        [SerializeField]
        private SerializableDictionary<ProjectileType, ProjectileConfig> _configs = new();

        public int Count => _configs.Count;

        public ProjectileConfig GetConfig(ProjectileType type) => _configs[type];

        public IEnumerator<KeyValuePair<ProjectileType, ProjectileConfig>> GetEnumerator() => _configs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}