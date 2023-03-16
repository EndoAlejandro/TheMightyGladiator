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

            // if (_timer <= 0f) Attack();
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
            _directions = _blob.GetFanPatternDirections();

            foreach (var direction in _directions)
            {
                var bullet = _blob.Bullet.Get<Bullet>(_blob.transform.position + Vector3.up * 0.5f,
                    Quaternion.LookRotation(direction));
                // bullet.GetComponent<Rigidbody>().velocity = direction;
                bullet.Setup(direction, _blob.BulletSpeed);
            }
        }

        public void OnExit()
        {
        }
    }
}