using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;

        private void Awake()
        {
            startGameButton.onClick.AddListener(OnStartGameButtonPressed);
        }

        private void OnStartGameButtonPressed()
        {
            GameManager.Instance.StartGame();
        }
    }
}