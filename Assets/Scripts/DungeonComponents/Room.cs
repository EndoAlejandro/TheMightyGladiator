using System;
using PlayerComponents;
using ProceduralGeneration;
using StateMachineComponents;
using UnityEngine;

namespace DungeonComponents
{
    public class Room : FiniteStateBehaviour
    {
        [SerializeField] private GameObject roomBody;
        [SerializeField] private GameObject cover;
        [SerializeField] private float spawnTime;

        private Door[] _doors;
        
        private RoomData _roomData;
        private RoomSpawn _spawn;
        private RoomCleared _cleared;

        private bool _isCleared;

        public int ID { get; private set; }
        public float SpawnTime => spawnTime;
        public TileType TileType { get; private set; }

        public void Setup(RoomData roomData)
        {
            ID = gameObject.GetInstanceID();

            _roomData = roomData;
            TileType = _roomData.TileType;

            _doors = GetComponentsInChildren<Door>();
            
            var idle = new RoomIdle(roomBody);
            _spawn = new RoomSpawn(this, _doors);
            _cleared = new RoomCleared();
            var battle = new RoomBattle(_doors);

            stateMachine.SetState(idle);

            stateMachine.AddTransition(_spawn, battle, () => _spawn.Ended);
        }

        private bool IsThisRoomActive() => DungeonManager.Instance.CurrentRoom.ID == ID;

        public void SetRoomVisibility(bool isVisible)
        {
            roomBody.SetActive(isVisible);
            cover.SetActive(!isVisible);

            if (!_isCleared) stateMachine.SetState(_spawn);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            if (IsThisRoomActive()) return;
            DungeonManager.Instance.SetCurrentRoom(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            if (!IsThisRoomActive()) return;
            stateMachine.SetState(_spawn);
        }

    }
}