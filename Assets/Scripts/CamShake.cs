using Cinemachine;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance { get; private set; }

    private CinemachineImpulseSource _impulseSource;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float force = 1f)
    {
        _impulseSource.GenerateImpulse(force);
    }
}
