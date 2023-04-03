using System.Collections;
using Enemies;
using Rooms;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public void Setup(Room room, Enemy enemyToSpawn) => StartCoroutine(SpawnEnemyAfterSeconds(room, enemyToSpawn));
    private IEnumerator SpawnEnemyAfterSeconds(Room room, Enemy enemyToSpawn)
    {
        yield return new WaitForSeconds(0.25f);
        enemyToSpawn.Get<Enemy>();
        enemyToSpawn.Setup(room);
    }
}