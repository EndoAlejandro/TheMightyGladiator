using PlayerComponents;
using ProceduralGeneration;
using StateMachineComponents;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

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
        private NavMeshSurface _navMeshSurface;

        public bool IsCleared { get; private set; }
        private RoomIdle _idle;

        public int ID { get; private set; }
        public float SpawnTime => spawnTime;
        public RoomType RoomType { get; private set; }

        public void Setup(RoomData roomData)
        {
            ID = gameObject.GetInstanceID();

            _roomData = roomData;
            RoomType = _roomData.RoomType;

            if (RoomType == RoomType.Origin) IsCleared = true;

            _doors = GetComponentsInChildren<Door>();
            _navMeshSurface = GetComponent<NavMeshSurface>();

            _idle = new RoomIdle(_doors);
            _spawn = new RoomSpawn(this, _doors);
            _cleared = new RoomCleared();
            var battle = new RoomBattle(this, _doors);

            stateMachine.SetState(_idle);

            stateMachine.AddTransition(_idle, _spawn, () => IsThisRoomActive() && !IsCleared);
            stateMachine.AddTransition(_spawn, battle, () => _spawn.Ended);
            stateMachine.AddTransition(battle, _cleared, () => battle.Ended);

            stateMachine.AddTransition(_idle, _cleared, () => IsThisRoomActive() && IsCleared);
            stateMachine.AddTransition(_cleared, _idle, () => !IsThisRoomActive());
        }

        private bool IsThisRoomActive() => DungeonManager.Instance.CurrentRoom.ID == ID;

        public void ClearRoom() => IsCleared = true;

        public void SetRoomVisibility(bool isVisible)
        {
            roomBody.SetActive(isVisible);
            cover.SetActive(!isVisible);
            if (isVisible)
            {
                _navMeshSurface.BuildNavMesh();
            }
            else
            {
                stateMachine.SetState(_idle);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            if (IsThisRoomActive()) return;
            DungeonManager.Instance.SetCurrentRoom(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (RoomType == RoomType.Origin) return;
            if (!other.TryGetComponent(out Player player)) return;
            if (!IsThisRoomActive()) return;
            if (!IsCleared) stateMachine.SetState(_spawn);
        }
    }
}