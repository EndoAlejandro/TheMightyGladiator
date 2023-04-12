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
                LoadGameFinished();
                return;
            }
        }

        LoadNextGameScene();
    }

    private void LoadGameFinished()
    {
        SaveSystem.SetProgress(new Vector2Int());
        StartCoroutine(LoadSceneAsync("GameOver"));
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

    public void StartDungeonPortalActivated() => LoadNextGameScene();

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

    private void OnApplicationQuit() => SaveSystem.SetProgress(new Vector2Int(CurrentLevel, CurrentSubLevel));
}