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

        private RoomData _roomData;
        private RoomSpawn _spawn;
        private RoomCleared _cleared;

        private bool _isCleared;

        public int ID { get; private set; }
        public TileType TileType { get; private set; }

        public void Setup(RoomData roomData)
        {
            _roomData = roomData;
            TileType = _roomData.TileType;

            var idle = new RoomInvisible(roomBody);
            _spawn = new RoomSpawn(this);
            _cleared = new RoomCleared();
            var battle = new RoomBattle();

            stateMachine.SetState(idle);
            ID = gameObject.GetInstanceID();

            // stateMachine.AddTransition(idle,spawn, ());
        }

        private bool IsThisRoomActive() => DungeonManager.Instance.CurrentRoom.ID == ID;

        public void SetRoomVisibility(bool isVisible)
        {
            roomBody.SetActive(isVisible);
            cover.SetActive(!isVisible);
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