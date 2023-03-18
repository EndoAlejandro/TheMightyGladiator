using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetTarget(Transform target) => _virtualCamera.m_Follow = target;
}