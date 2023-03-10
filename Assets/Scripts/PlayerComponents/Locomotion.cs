using System;
using Inputs;
using UnityEngine;

namespace PlayerComponents
{
    public class Locomotion : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float rotationSpeed = 100f;

        private Vector3 _moveDirection;

        private Rigidbody _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void Update()
        {
            PlayerRotation();
            _moveDirection = new Vector3(InputReader.Instance.Movement.x, 0f, InputReader.Instance.Movement.y).normalized;
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(_moveDirection * (speed * acceleration), ForceMode.Force);
            SpeedControl();
        }

        private void PlayerRotation()
        {
            var aimDirection = new Vector3(InputReader.Instance.Aim.x, 0f, InputReader.Instance.Aim.y);
            transform.forward =
                Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
        }

        private void SpeedControl()
        {
            if (_rigidbody.velocity.magnitude > speed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * speed;
        }
    }
}