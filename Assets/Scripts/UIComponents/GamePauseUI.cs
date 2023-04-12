using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class GamePauseUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            panel.SetActive(false);
            resumeButton.onClick.AddListener(GameManager.Instance.PauseToggle);
            mainMenuButton.onClick.AddListener(GameManager.Instance.FromGameToMenu);
            GameManager.Instance.OnGamePaused += GameManagerOnGamePaused;
        }

        private void GameManagerOnGamePaused(bool isPaused) => panel.SetActive(isPaused);

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGamePaused -= GameManagerOnGamePaused;
        }
    }
}