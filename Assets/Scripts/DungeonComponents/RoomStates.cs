using StateMachineComponents;
using UnityEngine;

namespace DungeonComponents
{
    public class RoomInvisible : IState
    {
        private readonly GameObject _roomBody;

        public RoomInvisible(GameObject roomBody) => _roomBody = roomBody;

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            // _roomBody.SetActive(false);
        }

        public void OnExit()
        {
            // _roomBody.SetActive(true);
        }
    }

    public class RoomSpawn : IState
    {
        private readonly Room _room;

        public RoomSpawn(Room room) => _room = room;

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }

        public void OnEnter()
        {
            // Useless change.
        }

        public void OnExit()
        {
        }
    }

    public class RoomBattle : IState
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