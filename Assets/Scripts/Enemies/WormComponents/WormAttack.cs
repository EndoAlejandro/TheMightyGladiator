using PlayerComponents;
using UnityEngine;

namespace Enemies.WormComponents
{
    public class WormAttack : EnemyAttack
    {
        private readonly Worm _worm;
        private readonly Vector3 _offset;
        private readonly Collider[] _results;

        private float _timer;

        public bool Ended => _timer <= 0f;

        public WormAttack(Worm worm)
        {
            enemy = worm;
            _worm = worm;
            _results = new Collider[5];

            _offset = Vector3.up * _worm.Height;
        }

        public override void Tick()
        {
            _timer -= Time.deltaTime;

            var size = Physics.OverlapSphereNonAlloc(enemy.transform.position + _offset, _worm.AttackRadius, _results);
            for (int i = 0; i < size; i++)
            {
                var result = _results[i];
                if (!result.TryGetComponent(out Player player)) continue;
                if (!player.TryToDealDamage(_worm.transform.position)) continue;
                player.TakeDamage(_worm);
            }
        }

        public override void FixedTick()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _worm.SetColliderState(true);
            _timer = _worm.IdleTime;
        }
    }
}