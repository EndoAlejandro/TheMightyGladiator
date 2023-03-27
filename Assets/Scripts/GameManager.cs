using System;
using System.Collections;
using BigRoom;
using CustomUtils;
using Enemies;
using PlayerComponents;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player player;
    [SerializeField] private LevelData[] levels;

    private int _currentBiome;
    private int _currentFloor;

    private LevelData _currentLevelData;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void SpawnNewLevel()
    {
        _currentLevelData = levels[_currentBiome];

        HadesRoom instancedRoom = null;
        if (_currentFloor == 0)
            instancedRoom = Instantiate(_currentLevelData.InitialRoom);
        else if (_currentFloor < _currentLevelData.Rooms.Length)
        {
            var index = Random.Range(0, _currentLevelData.Rooms.Length);
            instancedRoom = Instantiate(_currentLevelData.Rooms[index]);
        }
        else
            instancedRoom = Instantiate(_currentLevelData.BossRoom);

        instancedRoom.Setup(player);
    }

    public void NextLevel()
    {
        _currentFloor++;

        if (_currentFloor >= _currentLevelData.Floors)
        {
            _currentFloor = 0;
            _currentBiome++;

            if (_currentBiome >= levels.Length)
            {
                //TODO: EndGame.
                Debug.LogError("No more levels to display");
            }
        }

        StartCoroutine(ReloadGameScene());
    }

    public void PortalActivated(PortalType portalType)
    {
        if(portalType == PortalType.Normal) NextLevel();
        else if (portalType == PortalType.Starting) StartGame();
    }

    public void StartGame() => StartCoroutine(ReloadGameScene());

    public void LoadLobby() => StartCoroutine(LoadLobbyScene());

    private IEnumerator LoadLobbyScene()
    {
        yield return SceneManager.LoadSceneAsync("Lobby");
    }

    private IEnumerator ReloadGameScene()
    {
        yield return SceneManager.LoadSceneAsync("HadesLike");
        SpawnNewLevel();
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
    public int Floors => floors;
    public InitialRoomController InitialRoom => initialRoom;
    public BossRoomController BossRoom => bossRoom;
    public BigRoomController[] Rooms => rooms;
}