using System.Collections.Generic;
using Enemies;
using PlayerComponents;
using ProceduralGeneration;
using StateMachineComponents;
using Unity.AI.Navigation;
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
        private NavMeshSurface _navMeshSurface;

        public bool IsCleared { get; private set; }
        private RoomIdle _idle;
        private int _enemyCount;

        public int ID { get; private set; }
        public float SpawnTime => spawnTime;
        public RoomType RoomType { get; private set; }
        public VasePattern VasePattern { get; private set; }
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

        public void Setup(RoomData roomData, VasePattern vasePattern)
        {
            ID = gameObject.GetInstanceID();

            _roomData = roomData;
            RoomType = _roomData.RoomType;

            switch (RoomType)
            {
                case RoomType.Origin:
                    IsCleared = true;
                    break;
                case RoomType.Room:
                    VasePattern = Instantiate(vasePattern, transform.position, Quaternion.identity, roomBody.transform);
                    break;
                case RoomType.Boss:
                    VasePattern = Instantiate(vasePattern, transform.position, Quaternion.identity, roomBody.transform);
                    break;
            }

            _doors = GetComponentsInChildren<Door>();
            _navMeshSurface = GetComponent<NavMeshSurface>();

            StateManagement();
        }

        private void StateManagement()
        {
            _idle = new RoomIdle(_doors);
            _spawn = new RoomSpawn(this, _doors);
            _cleared = new RoomCleared();
            var battle = new RoomBattle(this, _doors);

            stateMachine.SetState(_idle);

            stateMachine.AddTransition(_idle, _spawn, () => IsThisRoomActive() && !IsCleared);
            stateMachine.AddTransition(_spawn, battle, () => _spawn.Ended);
            stateMachine.AddTransition(battle, _cleared, () => Enemies.Count <= 0);

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
                _navMeshSurface.BuildNavMesh();
            else
                stateMachine.SetState(_idle);
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

        public void SetEnemies(List<Enemy> enemies)
        {
            Enemies = enemies;
            _enemyCount = enemies.Count;
            foreach (var enemy in Enemies)
                enemy.OnDead += EnemyOnDead;
        }

        private void EnemyOnDead(Enemy enemy)
        {
            _enemyCount--;
            Enemies.Remove(enemy);
            enemy.OnDead -= EnemyOnDead;
        }
    }
}