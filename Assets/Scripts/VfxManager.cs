using System.Collections.Generic;
using Pooling;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private PoolAfterSeconds swordHit;
    [SerializeField] private PoolAfterSeconds swordCriticalHit;
    [SerializeField] private PoolAfterSeconds playerGetHit;

    private static Dictionary<Vfx, PoolAfterSeconds> _listedVfx;

    private void Awake()
    {
        _listedVfx = new Dictionary<Vfx, PoolAfterSeconds>
        {
            { Vfx.Sword, swordHit },
            { Vfx.SwordCritical, swordCriticalHit },
            { Vfx.PlayerHit, playerGetHit },
        };
    }

    public static void PlayFxWithRotation(Vfx fx, Vector3 position, Quaternion rotation) => _listedVfx[fx].Get<PoolAfterSeconds>(position, rotation);
    public static void PlayFx(Vfx fx, Vector3 position) => PlayFxWithRotation(Vfx.Sword, position, Quaternion.identity);
}