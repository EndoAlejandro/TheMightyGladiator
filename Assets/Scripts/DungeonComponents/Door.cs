using ProceduralGeneration;
using UnityEngine;

namespace DungeonComponents
{
    public class Door : MonoBehaviour
    {
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        [SerializeField] private DoorSide doorSide;
        private Animator _animator;
        private Collider _collider;
        private bool _isOpen = true;
        public DoorSide DoorSide => doorSide;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
        }

        private void Update() => _animator.SetBool(IsOpen, _isOpen);
        public void SetIsOpen(bool isOpen) => _isOpen = isOpen;
    }
}