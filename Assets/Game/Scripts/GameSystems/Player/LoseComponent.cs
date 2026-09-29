using Fusion;
using Game.Scripts.Common;

namespace Game.Scripts.GameSystems.Player
{
    public sealed class LoseComponent : NetworkBehaviour
    {
        public interface ICondition
        {
            bool IsMet();
        }

        private ICondition _condition;
        private GameState _gameState;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        public override void Spawned()
        {
            ServiceLocator.TryGet(out _gameState);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || _gameState == null || _gameState.IsGameOver ||
                _condition == null || !_condition.IsMet())
                return;

            _gameState.SetGameOver();
        }
    }
}
