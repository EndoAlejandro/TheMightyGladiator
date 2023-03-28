using System;
using BigRoom;
using Enemies;
using Enemies.BigBobComponents;
using UnityEngine;

[Serializable]
public struct LevelData
{
    [Header("Enemies")]
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private BigBob boss;

    [Header("Floors")]
    [SerializeField] private Biome biome;

    [SerializeField] private int floors;
    [SerializeField] private InitialRoomController initialRoom;
    [SerializeField] private BossRoomController bossRoom;
    [SerializeField] private BigRoomController[] rooms;
    public int Floors => floors + 1;
    public InitialRoomController InitialRoom => initialRoom;
    public BossRoomController BossRoom => bossRoom;
    public BigRoomController[] Rooms => rooms;
    public Enemy[] Enemies => enemies;
    public BigBob Boss => boss;
}