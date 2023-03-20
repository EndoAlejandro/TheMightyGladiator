using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralGeneration
{
    public class Walker
    {
        public Vector2Int Position { get; private set; }
        private readonly RoomType _roomType;
        private int _remainingSteps;

        public Walker(Vector2Int position, RoomType roomType, int steps)
        {
            Position = position;
            _roomType = roomType;
            _remainingSteps = steps;
        }

        public RoomData[,] Walk(RoomData[,] matrix)
        {
            var doors = Enum.GetValues(typeof(DoorSide)).Cast<DoorSide>().ToList();

            while (_remainingSteps > 0)
            {
                var direction = doors[Random.Range(0, doors.Count)];
                var stepValue = GetPositionBySide(direction);
                var nextPosition = stepValue + Position;

                if (IsOutOfBounds(nextPosition, matrix) ||
                    matrix[nextPosition.x, nextPosition.y]?.RoomType == RoomType.Boss)
                    continue;

                matrix[Position.x, Position.y].SetDoorValue(direction, DoorType.Normal);

                RoomData nextRoom = null;
                if (matrix[nextPosition.x, nextPosition.y] != null)
                    nextRoom = matrix[nextPosition.x, nextPosition.y];
                else
                {
                    var nextTile = _remainingSteps > 0 ? RoomType.Room : _roomType;
                    nextRoom = new RoomData(nextPosition, nextTile);
                    _remainingSteps--;
                }

                /*if (_remainingSteps <= 0)
                    matrix[Position.x, Position.y].SetDoorValue(direction, DoorType.Normal);*/

                nextRoom.SetDoorValue(GetOppositeDirection(direction), DoorType.Normal);
                Position = nextPosition;
                matrix[Position.x, Position.y] = nextRoom;
            }

            return matrix;
        }

        private Vector2Int GetPositionBySide(DoorSide doorSide)
        {
            Vector2Int position = doorSide switch
            {
                DoorSide.North => new Vector2Int(0, 1),
                DoorSide.East => new Vector2Int(1, 0),
                DoorSide.South => new Vector2Int(0, -1),
                DoorSide.West => new Vector2Int(-1, 0),
                _ => Vector2Int.zero
            };

            return position;
        }

        private DoorSide GetOppositeDirection(DoorSide door)
        {
            return door switch
            {
                DoorSide.North => DoorSide.South,
                DoorSide.East => DoorSide.West,
                DoorSide.South => DoorSide.North,
                DoorSide.West => DoorSide.East,
                _ => door
            };
        }

        private bool IsOutOfBounds(Vector2Int position, RoomData[,] matrix) =>
            position.x < 0 || position.x >= matrix.GetLength(0) ||
            position.y < 0 || position.y >= matrix.GetLength(1);
    }
}