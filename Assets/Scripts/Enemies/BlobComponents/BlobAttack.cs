using StateMachineComponents;
using UnityEngine;

namespace Enemies.BlobComponents
{
    public class BlobAttack : IState
    {
        private readonly Blob _blob;

        private Vector3[] _directions;

        public BlobAttack(Blob blob) => _blob = blob;
        public bool Ended { get; private set; }

        private float _timer;

        public void Tick()
        {
            _timer -= Time.deltaTime;
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _timer = _blob.MoveRate;
            Ended = false;
            Attack();
        }

        private void Attack()
        {
            Ended = true;

            var bullet = _blob.BulletPrefab.Get<Bullet>(_blob.transform.position + Vector3.up * 0.5f,
                Quaternion.LookRotation(_blob.transform.forward));
            bullet.Setup(_blob.transform.forward, _blob.BulletSpeed, _blob.Damage, true, 10f);

        }

        public void OnExit()
        {
        }
    }
}