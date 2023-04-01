using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomUtils;
using PlayerComponents;
using Rooms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Upgrades;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player player;
    [SerializeField] private Upgrade[] upgrades;
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

        BaseRoom instancedBaseRoom = null;
        if (_currentFloor == 0)
            instancedBaseRoom = Instantiate(_currentLevelData.InitialRoom);
        else if (_currentFloor < _currentLevelData.Floors)
        {
            var index = Random.Range(0, _currentLevelData.Rooms.Length);
            instancedBaseRoom = Instantiate(_currentLevelData.Rooms[index]);
        }
        else
            instancedBaseRoom = Instantiate(_currentLevelData.BossRoom);

        instancedBaseRoom.Setup(_currentLevelData, player);
    }

    public void NextLevel()
    {
        _currentFloor++;

        if (_currentFloor > _currentLevelData.Floors)
        {
            _currentFloor = 0;
            _currentBiome++;

            if (_currentBiome >= levels.Length)
            {
                LoadMainMenu();
                return;
            }
        }

        LoadNextGameScene();
    }

    private void LoadMainMenu()
    {
        _currentBiome = 0;
        _currentFloor = 0;
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }

    public void PortalActivated(PortalType portalType)
    {
        if (portalType == PortalType.Normal) NextLevel();
        else if (portalType == PortalType.Starting) StartGame();
    }

    private void LoadNextGameScene() => StartCoroutine(LoadSceneAsync("Dungeon", SpawnNewLevel));
    public void StartGame() => LoadNextGameScene();
    public void LoadLobby() => StartCoroutine(LoadSceneAsync("Lobby"));

    private IEnumerator LoadSceneAsync(string sceneName, Action callback = null)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
        callback?.Invoke();
    }

    public List<Upgrade> GetUpgrades(int amount)
    {
        var upgradesList = upgrades.ToList();
        if (!player.CanBeHealed)
            foreach (var upgrade in upgradesList.Where(upgrade => upgrade.UpgradeType == UpgradeType.Heal))
                upgradesList.Remove(upgrade);

        upgradesList.Shuffle();
        return upgradesList.Take(amount).ToList();
    }
}