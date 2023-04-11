using CustomUtils;
using UnityEngine;

namespace VfxComponents
{
    public class EnemyFloorMark : MonoBehaviour
    {
        [SerializeField] private GameObject display;
        private bool _isActive;

        private void Start() => ChangeDisplayStatus(true);

        private void Update()
        {
            if (_isActive) transform.position = transform.position.With(y: 0f);
        }

        public void ChangeDisplayStatus(bool activeStatus)
        {
            _isActive = activeStatus;
            display.SetActive(activeStatus);
        }
    }
}