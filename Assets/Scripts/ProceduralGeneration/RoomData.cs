using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralGeneration
{
    public class RoomData
    {
        public Vector2Int Positions { get; private set; }
        public TileType TileType { get; private set; }
        public Dictionary<DoorSide, DoorType> Doors { get; private set; }

        public RoomData(Vector2Int position, TileType tileType)
        {
            Positions = position;
            TileType = tileType;
            InitializeDoors();
        }

        private void InitializeDoors()
        {
            Doors = new Dictionary<DoorSide, DoorType>
            {
                { DoorSide.North, DoorType.Wall },
                { DoorSide.East, DoorType.Wall },
                { DoorSide.South, DoorType.Wall },
                { DoorSide.West, DoorType.Wall }
            };
        }

        public void SetDoorValue(DoorSide door, DoorType type) => Doors[door] = type;

        public List<DoorSide> GetAvailableDoors() =>
            (from doorPair in Doors where doorPair.Value != DoorType.Wall select doorPair.Key).ToList();
    }
}