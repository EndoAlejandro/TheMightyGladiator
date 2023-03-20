using ProceduralGeneration;
using UnityEngine;

namespace DungeonComponents
{
    public class Door : MonoBehaviour
    {
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");
        [SerializeField] private DoorSide doorSide;
        private Animator _animator;
        public DoorSide DoorSide => doorSide;
        private void Awake() => _animator = GetComponent<Animator>();
        private void Start() => SetIsOpen(true);
        public void SetIsOpen(bool isOpen) => _animator.SetBool(IsOpen, isOpen);
    }
}