using StateMachineComponents;
using UnityEngine;

namespace DungeonComponents
{
    public class RoomIdle : IState
    {
        private readonly GameObject _roomBody;

        public RoomIdle(GameObject roomBody) => _roomBody = roomBody;

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

            foreach (var door in _doors) door.SetIsOpen(false);
        }

        public void OnExit()
        {
        }
    }

    public class RoomBattle : IState
    {
        private readonly Door[] _doors;

        public RoomBattle(Door[] doors)
        {
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