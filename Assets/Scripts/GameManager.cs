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
    public event Action OnGameOver;
    public event Action<bool> OnGamePaused;

    [SerializeField] private Player playerPrefab;

    [SerializeField] private PlayerData defaultPlayerData;

    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private LevelData[] levels;

    public int CurrentLevel { get; private set; }
    public int CurrentSubLevel { get; private set; }

    private LevelData _currentLevelData;
    public PlayerData DefaultPlayerData => defaultPlayerData;
    private bool _isPaused;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => LoadProgress();

    private void SpawnNewLevel()
    {
        _currentLevelData = levels[CurrentLevel];

        BaseRoom instancedBaseRoom = null;
        if (CurrentSubLevel == 0)
            instancedBaseRoom = Instantiate(_currentLevelData.InitialRoom);
        else if (CurrentSubLevel < _currentLevelData.Floors)
        {
            var index = Random.Range(0, _currentLevelData.Rooms.Length);
            instancedBaseRoom = Instantiate(_currentLevelData.Rooms[index]);
        }
        else
            instancedBaseRoom = Instantiate(_currentLevelData.BossRoom);

        instancedBaseRoom.Setup(_currentLevelData, playerPrefab);
    }

    public void NextLevel()
    {
        CurrentSubLevel++;

        if (CurrentSubLevel > _currentLevelData.Floors)
        {
            CurrentSubLevel = 0;
            CurrentLevel++;

            if (CurrentLevel >= levels.Length)
            {
                LoadMainMenu();
                return;
            }
        }

        LoadNextGameScene();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        LoadProgress();
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }

    private void LoadProgress()
    {
        var progress = SaveSystem.GetProgress();
        CurrentLevel = progress.x;
        CurrentSubLevel = progress.y;
    }

    public void FromGameToMenu()
    {
        SaveSystem.SetProgress(new Vector2Int(CurrentLevel, CurrentSubLevel));
        LoadMainMenu();
    }

    public void StartDungeonPortalActivated()
    {
        // Player.Instance.SetPlayerData(DefaultPlayerData);
        LoadNextGameScene();
    }

    public void PortalActivated()
    {
        Player.Instance.SaveData();
        NextLevel();
    }

    private void LoadNextGameScene() => StartCoroutine(LoadSceneAsync("Dungeon", SpawnNewLevel));

    private IEnumerator LoadSceneAsync(string sceneName, Action callback = null)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
        callback?.Invoke();
    }

    public List<Upgrade> GetUpgrades(int amount)
    {
        var upgradesList = upgrades.ToList();
        var result = new List<Upgrade>(upgradesList);
        if (Player.Instance.CanBeHealed)
            foreach (var upgrade in upgradesList.Where(upgrade => upgrade.UpgradeType == UpgradeType.Heal))
            {
                result.Remove(upgrade);
                break;
            }

        result.Shuffle();
        return result.Take(amount).ToList();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        Player.Instance.SetPlayerData(DefaultPlayerData);
        SaveSystem.SetProgress(new Vector2Int());
        Time.timeScale = 0f;
    }

    public void PauseToggle()
    {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
        OnGamePaused?.Invoke(_isPaused);
    }
}