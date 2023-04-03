using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button titleButton;

        private void Start()
        {
            panel.SetActive(false);
            GameManager.Instance.OnGameOver += GameManagerOnGameOver;
            tryAgainButton.onClick.AddListener(TryAgainButtonPressed);
            titleButton.onClick.AddListener(TitleButtonPressed);
        }

        private void GameManagerOnGameOver() => panel.SetActive(true);
        private void TryAgainButtonPressed() => GameManager.Instance.LoadLobby();
        private void TitleButtonPressed() => GameManager.Instance.LoadMainMenu();

        private void OnDestroy()
        {
            if(GameManager.Instance != null)
                GameManager.Instance.OnGameOver -= GameManagerOnGameOver;
        }
    }
}