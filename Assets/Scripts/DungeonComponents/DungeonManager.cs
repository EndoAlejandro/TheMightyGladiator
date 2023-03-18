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

        private DungeonGenerator _dungeonGenerator;
        public Room CurrentRoom { get; private set; }

        public Room[,] Matrix { get; private set; }

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

                if (room.TileType == TileType.Origin)
                {
                    SetCurrentRoom(room);
                    Instantiate(playerPrefab, room.transform.position, quaternion.identity);
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
        }
    }
}