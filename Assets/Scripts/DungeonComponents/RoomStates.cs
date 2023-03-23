﻿using System.Collections.Generic;
using Enemies;
using ProceduralGeneration;
using StateMachineComponents;
using UnityEngine;

namespace DungeonComponents
{
    public class RoomIdle : IState
    {
        private readonly Door[] _doors;

        public RoomIdle(Door[] doors) => _doors = doors;

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            foreach (var door in _doors) door.gameObject.SetActive(false);
        }

        public void OnExit()
        {
        }
    }

    public class RoomSpawn : IState
    {
        private readonly Room _room;
        private readonly Door[] _doors;
        private float _timer;
        public bool Ended => _timer <= 0f;

        public RoomSpawn(Room room, Door[] doors)
        {
            _room = room;
            _doors = doors;
        }

        public void Tick() => _timer -= Time.deltaTime;

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            _timer = _room.SpawnTime;

            foreach (var door in _doors)
            {
                door.gameObject.SetActive(true);
                door.SetIsOpen(false);
            }

            if (_room.Enemies.Count > 0) return;

            var spawnPoints = _room.VasePattern.SpawnPoints;
            var enemies = new List<Enemy>();
            var isBoss = _room.RoomType == RoomType.Boss;
            foreach (var spawnPoint in spawnPoints)
            {
                var index = Random.Range(0, isBoss
                    ? DungeonManager.Instance.Bosses.Length
                    : DungeonManager.Instance.Enemies.Length
                );
                var enemy = spawnPoint.SpawnEnemy(isBoss
                    ? DungeonManager.Instance.Bosses[index]
                    : DungeonManager.Instance.Enemies[index]);
                enemies.Add(enemy);
            }

            _room.SetEnemies(enemies);
        }

        public void OnExit()
        {
        }
    }

    public class RoomBattle : IState
    {
        private readonly Room _room;
        private readonly Door[] _doors;

        public RoomBattle(Room room, Door[] doors)
        {
            _room = room;
            _doors = doors;
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
            _room.ClearRoom();
            foreach (var door in _doors)
            {
                door.SetIsOpen(true);
            }
        }
    }

    public class RoomCleared : IState
    {
        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}