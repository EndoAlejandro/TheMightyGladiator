namespace ProceduralGeneration
{
    public enum TileType
    {
        Empty,
        Room,
        Origin,
        Map,
        SmallKey,
        BossKey,
        Boss,
        BigBoss
    }
    
    public enum DoorType
    {
        Wall,
        Normal,
        Locked,
        BossLocked,
    }
    
    public enum DoorSide
    {
        North,
        East,
        South,
        West,
        None,
    }
}