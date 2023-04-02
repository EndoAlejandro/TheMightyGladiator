using System.Collections.Generic;
using CustomUtils;
using Enemies.BatComponents;
using StateMachineComponents;
using UnityEngine;

namespace Enemies.SpawnerComponents
{
    public class BatSpawnerSpawn : StateTimer, IState
    {
        private readonly BatSpawner _batSpawner;
        private readonly List<Bat> _spawnedBats;
        private Vector3[] _directions;

        public BatSpawnerSpawn(BatSpawner batSpawner)
        {
            _batSpawner = batSpawner;
            _spawnedBats = new List<Bat>();
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            timer = 0.5f;
            _directions = Utils.GetFanPatternDirections(_batSpawner.transform, _batSpawner.SpawnAmount, 360f);

            foreach (var direction in _directions)
            {
                var bat = _batSpawner.BatPrefab.Get<Bat>(
                    _batSpawner.transform.position + direction * 1.5f, Quaternion.identity);
                _spawnedBats.Add(bat);
            }

            _batSpawner.RegisterSpawnedBats(_spawnedBats);
        }

        public override void OnExit()
        {
            base.OnExit();
            _spawnedBats.Clear();
        }
    }

    public class BatSpawnedDeath : EnemyDeath
    {
        private readonly BatSpawner _batSpawner;
        private readonly Collider _collider;

        public BatSpawnedDeath(BatSpawner batSpawner, Collider collider) : base(batSpawner)
        {
            _batSpawner = batSpawner;
            _collider = collider;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _collider.enabled = false;
        }

        public override void OnExit()
        {
            _collider.enabled = true;
            base.OnExit();
        }
    }
}