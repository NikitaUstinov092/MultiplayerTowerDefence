using Fusion;

namespace Game.Scripts.GameSystems.Player
{
    public sealed class LoseComponent : NetworkBehaviour
    {
        public interface ICondition
        {
            bool IsMet();
        }

        // Условие проверяется каждый тик - флаг нужен, чтобы уведомление о поражении ушло один раз.
        [Networked]
        private NetworkBool _isLost { get; set; }

        private ICondition _condition;

        public void SetCondition(ICondition condition)
        {
            _condition = condition;
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || _isLost || _condition == null || !_condition.IsMet())
                return;

            _isLost = true;
            Runner.GetBehaviour<LoseNotificator>().NotifyAboutLose();
        }
    }
}
