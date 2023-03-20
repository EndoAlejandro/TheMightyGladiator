using Enemies;
using PlayerComponents;
using ProceduralGeneration;
using Unity.Mathematics;
using UnityEngine;

namespace DungeonComponents
{
    [RequireComponent(typeof(DungeonGenerator))]
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        [SerializeField] private Player playerPrefab;

        [SerializeField] private Enemy[] enemies;
        public Enemy[] Enemies => enemies;
        
        private DungeonGenerator _dungeonGenerator;
        public Room CurrentRoom { get; private set; }
        public Room[,] Matrix { get; private set; }

        private Player _player;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _dungeonGenerator = GetComponent<DungeonGenerator>();
        }

        private void Start()
        {
            Matrix = _dungeonGenerator.GenerateDungeon();

            foreach (var room in Matrix)
            {
                if (room == null) continue;
                room.SetRoomVisibility(false);

                if (room.RoomType == RoomType.Origin)
                {
                    _player = Instantiate(playerPrefab, room.transform.position, quaternion.identity);
                    SetCurrentRoom(room);
                }
            }
        }

        public void SetCurrentRoom(Room room)
        {
            if (CurrentRoom != null)
                CurrentRoom.SetRoomVisibility(false);

            CurrentRoom = room;
            CameraManager.Instance.SetTarget(CurrentRoom.transform);
            CurrentRoom.SetRoomVisibility(true);
            if (!CurrentRoom.IsCleared) _player.Sleep(1.5f);
        }
    }
}