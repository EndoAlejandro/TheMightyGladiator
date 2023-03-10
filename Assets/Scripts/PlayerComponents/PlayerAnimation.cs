using UnityEngine;

namespace PlayerComponents
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Right = Animator.StringToHash("Right");
        
        private Animator _animator;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void Update()
        {
            var forward = Vector3.Dot(transform.forward, _rigidbody.velocity);
            var right = Vector3.Dot(transform.right, _rigidbody.velocity);
            
            _animator.SetFloat(Forward, forward);
            _animator.SetFloat(Right, right);
        }
    }
}
