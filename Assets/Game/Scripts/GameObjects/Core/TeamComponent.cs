using Fusion;

namespace SampleGame
{
    public sealed class TeamComponent : NetworkBehaviour, HealthComponent.IDamageCondition
    {
        [Networked, UnitySerializeField]
        public Team Current { get; private set; }

        public void SetTeam(Team team)
        {
            this.Current = team;
        }

        bool HealthComponent.IDamageCondition.IsMet(PlayerRef attacker)
        {
            NetworkObject attackerObject = this.Runner.GetPlayerObject(attacker);
            if (attackerObject == null)
                return true;

            TeamComponent attackerTeam = attackerObject.GetComponentInChildren<TeamComponent>();
            if (attackerTeam == null)
                return true;

            return attackerTeam.Current != this.Current;
        }
    }
}
