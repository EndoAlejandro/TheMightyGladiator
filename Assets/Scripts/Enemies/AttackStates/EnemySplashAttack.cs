using PlayerComponents;
using StateMachineComponents;
using UnityEngine;
using VfxComponents;

namespace Enemies
{
    public class EnemySplashAttack : IState
    {
        private readonly Enemy _enemy;
        private readonly Collider[] _results;
        public EnemySplashAttack(Enemy enemy)
        {
            _enemy = enemy;
            _results = new Collider[20];
        }

        public bool Ended { get; private set; }

        public void Tick() => Ended = true;

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            Ended = false;
            FxManager.Instance.PlayFx(Vfx.Splash, _enemy.transform.position);
            var size =Physics.OverlapSphereNonAlloc(_enemy.transform.position, _enemy.AoeRadius, _results);

            for (int i = 0; i < size; i++)
            {
                if(!_results[i].TryGetComponent(out Player player)) continue;
                player.TryToGetDamageFromEnemy(_enemy, true);
                break;
            }
        }

        public void OnExit() => Ended = false;
    }
}