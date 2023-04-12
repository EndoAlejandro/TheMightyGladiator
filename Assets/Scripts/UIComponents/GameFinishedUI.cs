using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class GameFinishedUI : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(GameManager.Instance.LoadMainMenu);
        }
    }
}
