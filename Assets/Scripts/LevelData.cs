using System;
using Enemies;
using Enemies.BigBobComponents;
using Rooms;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct LevelData
{
    [Header("Enemies")]
    [SerializeField] private int enemiesPerWave;

    [SerializeField] private int waves;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private Enemy boss;

    [Header("Floors")]
    [SerializeField] private Biome biome;

    [SerializeField] private int floors;

    [FormerlySerializedAs("initialBaseRoom")] [SerializeField]
    private InitialRoom initialRoom;

    [FormerlySerializedAs("bossBaseRoom")] [SerializeField]
    private BossRoom bossRoom;

    [SerializeField] private Room[] rooms;
    public int EnemiesPerWave => enemiesPerWave;
    public int Waves => waves;
    public int Floors => floors + 1;
    public InitialRoom InitialRoom => initialRoom;
    public BossRoom BossRoom => bossRoom;
    public Room[] Rooms => rooms;
    public Enemy[] Enemies => enemies;
    public Enemy Boss => boss;
}