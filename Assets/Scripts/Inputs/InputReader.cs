using UnityEngine;

namespace Inputs
{
    public class InputReader : MonoBehaviour
    {
        public static InputReader Instance { get; private set; }

        public Vector2 Movement => _input != null ? _input.Main.Movement.ReadValue<Vector2>() : Vector2.zero;

        public Vector2 Aim => _input != null ? Aiming() : Vector2.zero;

        public bool Attack => _input != null && _input.Main.Attack.WasPressedThisFrame();
        public bool Shield => _input != null && _input.Main.Defend.IsPressed();
        public bool Dodge => _input != null && _input.Main.Dodge.WasPressedThisFrame();
        public bool Interact => _input != null && _input.Main.Interact.WasPressedThisFrame();

        private PlayerControls _input;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _input = new PlayerControls();
            _input.Enable();
        }

        private Vector2 Aiming()
        {
            var mouseScreen = _input.Main.Aim.ReadValue<Vector2>();
            return new Vector2(mouseScreen.x / Screen.width, mouseScreen.y / Screen.height); //- Vector2.one / 2;
        }
    }
}