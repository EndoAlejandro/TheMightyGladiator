using System;
using UnityEngine;

public class RandomFloorTile : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;

    private void Awake()
    {
        var noise = Mathf.PerlinNoise(transform.position.x, transform.position.z);
        var index = Mathf.RoundToInt(Mathf.Lerp(0, tiles.Length - 1, noise));

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].SetActive(index == i);
        }
    }
}