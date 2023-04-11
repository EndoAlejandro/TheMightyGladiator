using CustomUtils;
using FxComponents;
using PlayerComponents;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.JarBomberComponents
{
    public class JarBomberAttack : EnemyAttack
    {
        private readonly Enemy _enemy;
        private NavMeshHit _navMeshHit;
        public bool Ended { get; private set; }

        public JarBomberAttack(Enemy enemy) : base(enemy) => _enemy = enemy;

        public override void FixedTick() => Ended = true;

        public override void OnEnter()
        {
            Ended = false;
            base.OnEnter();

            for (int i = 0; i < _enemy.RoundsAmount; i++)
            {
                var mortarBullet =
                    _enemy.MortarPrefab.Get<MortarBomb>(
                        _enemy.transform.position.With(y: 1f),
                        Quaternion.identity);
                var target = Player.Instance.transform.position +
                             Random.insideUnitSphere.With(y: 0f).normalized * Random.Range(1f, _enemy.Accuracy);

                if (NavMesh.SamplePosition(target, out _navMeshHit, 2f, NavMesh.AllAreas))
                    target = _navMeshHit.position;

                mortarBullet.Setup(target, 70f);
            }
            SfxManager.Instance.PlayFx(Sfx.MortarShot, _enemy.transform.position);
        }
    }
}