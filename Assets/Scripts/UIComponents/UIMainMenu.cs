using System;
using FxComponents;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponents
{
    public class UIMainMenu : MonoBehaviour
    {
        private static readonly int ChangeMenu = Animator.StringToHash("ChangeMenu");

        [Header("Main Menu")]
        [SerializeField] private GameObject mainMenuContainer;

        [SerializeField] private Button startGameButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitGameButton;

        [Header("Options")]
        [SerializeField] private GameObject optionsMenuContainer;

        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider fxSlider;
        [SerializeField] private Button optionsReturnButton;

        [Header("Credits")]
        [SerializeField] private GameObject creditsMenuContainer;

        [SerializeField] private Button creditsReturnButton;


        private Animator _animator;

        private GameObject _activeMenu;
        private GameObject _nextMenu;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            mainMenuContainer.SetActive(false);
            optionsMenuContainer.SetActive(false);
            creditsMenuContainer.SetActive(false);

            // Main Menu
            startGameButton.onClick.AddListener(OnStartGameButtonPressed);
            optionsButton.onClick.AddListener(() => TriggerMenuChange(optionsMenuContainer));
            creditsButton.onClick.AddListener(() => TriggerMenuChange(creditsMenuContainer));
            exitGameButton.onClick.AddListener(Application.Quit);

            // Options
            optionsReturnButton.onClick.AddListener(() => TriggerMenuChange(mainMenuContainer));
            masterSlider.onValueChanged.AddListener(
                (value) => SfxManager.Instance.SetMixerGroupVolume(SaveSystem.PrefsField.Master, value));
            musicSlider.onValueChanged.AddListener(
                (value) => SfxManager.Instance.SetMixerGroupVolume(SaveSystem.PrefsField.Music, value));
            fxSlider.onValueChanged.AddListener(
                (value) => SfxManager.Instance.SetMixerGroupVolume(SaveSystem.PrefsField.Fx, value));

            // Credits
            creditsReturnButton.onClick.AddListener(() => TriggerMenuChange(mainMenuContainer));

            TriggerMenuChange(mainMenuContainer);
        }

        private void Start()
        {
            masterSlider.value = SaveSystem.GetVolume(SaveSystem.PrefsField.Master);
            musicSlider.value = SaveSystem.GetVolume(SaveSystem.PrefsField.Music);
            fxSlider.value = SaveSystem.GetVolume(SaveSystem.PrefsField.Fx);
        }

        private void OnStartGameButtonPressed() => GameManager.Instance.LoadLobby();

        private void TriggerMenuChange(GameObject next)
        {
            _animator.SetTrigger(ChangeMenu);
            _nextMenu = next;
        }

        private void SwapMenu()
        {
            if (_activeMenu != null) _activeMenu.SetActive(false);
            _nextMenu.SetActive(true);
            _activeMenu = _nextMenu;
        }
    }
}