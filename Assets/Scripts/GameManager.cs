using System;
using BigRoom;
using CustomUtils;
using Enemies;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private LevelData[] levels;

    protected override void Awake()
    {
        base.Awake();
    }
}

public enum Biome
{
    First,
    Second,
    Third,
}

[Serializable]
public struct LevelData
{
    [SerializeField] private Biome biome;
    [SerializeField] private int floors;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private InitialRoomController initialRoom;
    [SerializeField] private BossRoomController bossRoom;
    [SerializeField] private BigRoomController[] rooms;
}