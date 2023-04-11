using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button titleButton;

        private void Start()
        {
            panel.SetActive(false);
            GameManager.Instance.OnGameOver += GameManagerOnGameOver;
            titleButton.onClick.AddListener(TitleButtonPressed);
        }

        private void GameManagerOnGameOver() => panel.SetActive(true);

        private void TitleButtonPressed() => GameManager.Instance.LoadMainMenu();

        private void OnDestroy()
        {
            if(GameManager.Instance != null)
                GameManager.Instance.OnGameOver -= GameManagerOnGameOver;
        }
    }
}