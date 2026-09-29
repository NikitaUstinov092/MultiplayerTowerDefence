using Fusion;

namespace Game.Scripts.GameObjects.Core
{
    public sealed class TeamComponent : NetworkBehaviour, HealthComponent.IDamageCondition
    {
        [Networked, UnitySerializeField]
        public Team Current { get; private set; }

        public void SetTeam(Team team)
        {
            Current = team;
        }

        bool HealthComponent.IDamageCondition.IsMet(NetworkObject attacker)
        {
            if (attacker == null)
                return true;

            TeamComponent attackerTeam = attacker.GetComponentInChildren<TeamComponent>();
            if (attackerTeam == null)
                return true;

            return attackerTeam.Current != Current;
        }
    }
}
