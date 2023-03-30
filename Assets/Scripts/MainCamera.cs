using Cinemachine;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    private CinemachineImpulseSource _impulseSource;
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void Shake(float force = 0.5f) => _impulseSource.GenerateImpulse(force);
    public void SetTarget(Transform target) => _virtualCamera.m_Follow = target;
}