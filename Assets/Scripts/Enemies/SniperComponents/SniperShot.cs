using StateMachineComponents;
using UnityEngine;

namespace Enemies.SniperComponents
{
    public class SniperShot : IState
    {
        private readonly Enemy _enemy;
        private readonly Rigidbody _rigidbody;

        public SniperShot(Enemy enemy, Rigidbody rigidbody)
        {
            _enemy = enemy;
            _rigidbody = rigidbody;
        }

        public bool Ended { get; private set; }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            Ended = true;
            var bullet = _enemy.BulletPrefab.Get<Bullet>(_enemy.transform.position + Vector3.up, Quaternion.identity);
            bullet.Setup(_enemy.transform.forward, _enemy.BulletSpeed, 1);
            _rigidbody.AddForce(-_enemy.transform.forward, ForceMode.VelocityChange);
        }

        public void OnExit()
        {
        }
    }
}