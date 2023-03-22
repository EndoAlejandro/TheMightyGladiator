using DungeonComponents;
using UnityEngine;

namespace ProceduralGeneration
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int dungeonSize = new Vector2Int(3, 3);
        [SerializeField] private Vector2 roomSize = Vector2.one;
        [SerializeField] private int maxSteps = 4;

        [SerializeField] private Room initialRoomPrefab;
        [SerializeField] private Room bossRoomPrefab;
        [SerializeField] private Room[] normalRoomsPrefabs;
        [SerializeField] private VasePattern[] vasePatterns;
        [SerializeField] private VasePattern[] bossVasePatterns;

        private RoomData[,] _matrix;
        private Room[,] _roomMatrix;
        private Vector2Int _origin;

        private void Setup()
        {
            if (dungeonSize.x % 2 == 0) dungeonSize.x++;
            if (dungeonSize.y % 2 == 0) dungeonSize.y++;

            _matrix = new RoomData[dungeonSize.x, dungeonSize.y];
            _roomMatrix = new Room[dungeonSize.x, dungeonSize.y];
            _origin = new Vector2Int((dungeonSize.x - 1) / 2, (dungeonSize.y - 1) / 2);
        }

        public Room[,] GenerateDungeon()
        {
            Setup();

            _matrix[_origin.x, _origin.y] = new RoomData(_origin, RoomType.Origin);

            var bossWalker = new Walker(_origin, RoomType.Boss, maxSteps);
            _matrix = bossWalker.Walk(_matrix);
            var walker = new Walker(_origin, RoomType.Room, maxSteps);
            _matrix = walker.Walk(_matrix);

            CreateRooms();
            return _roomMatrix;
        }

        private void CreateRooms()
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    if (_matrix[i, j] == null) continue;

                    var spawnPosition = CalculateTileSpawnPosition(i, j);
                    var roomType = _matrix[i, j].RoomType;
                    Room roomToInstantiate = roomType switch
                    {
                        RoomType.Origin => initialRoomPrefab,
                        RoomType.Boss => bossRoomPrefab,
                        _ => normalRoomsPrefabs[Random.Range(0, normalRoomsPrefabs.Length)]
                    };

                    var instancedRoom = Instantiate(roomToInstantiate, spawnPosition, Quaternion.identity);
                    instancedRoom.transform.parent = transform;
                    var vasePattern = roomType == RoomType.Boss
                        ? bossVasePatterns[Random.Range(0, bossVasePatterns.Length)]
                        : vasePatterns[Random.Range(0, vasePatterns.Length)];
                    instancedRoom.Setup(_matrix[i, j], vasePattern);
                    _roomMatrix[i, j] = instancedRoom;
                }
            }
        }

        private Vector3 CalculateTileSpawnPosition(int i, int j) => new Vector3(
            (i - _matrix.GetLength(0) / 2) * (roomSize.x),
            0f, (j - _matrix.GetLength(1) / 2) * (roomSize.y));
    }
}