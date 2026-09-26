using Fusion;
using SampleGame;
using UnityEngine;

public class Enemy : NetworkBehaviour,
    MoveComponent.ICondition,
    CapsuleCollisionComponent.IHandler
{
    [SerializeField]
    private HealthComponent _healthComponent;

    [SerializeField]
    private MoveComponent _moveComponent;

    [SerializeField]
    private TeamComponent _teamComponent;

    [SerializeField]
    private CapsuleCollisionComponent _collisionComponent;

    [SerializeField]
    private ContactDamageComponent _contactDamage;

    public override void Spawned()
    {
        _moveComponent.SetCondition(this);

        if (_teamComponent != null)
            _healthComponent.SetDamageCondition(_teamComponent);

        if (_collisionComponent != null)
            _collisionComponent.SetHandler(this);
    }

    bool MoveComponent.ICondition.IsMet()
    {
        if(_healthComponent == null)
            return true;
        return _healthComponent.IsAlive;
    }

    void CapsuleCollisionComponent.IHandler.OnCollision(NetworkObject other)
    {
        if (_contactDamage == null || !_healthComponent.IsAlive)
            return;

        _contactDamage.TryDamage(other);
    }
}
