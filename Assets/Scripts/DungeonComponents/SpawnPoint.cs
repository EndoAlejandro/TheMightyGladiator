using Enemies;
using Pooling;
using UnityEngine;

namespace DungeonComponents
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private PoolAfterSeconds fx;

        public Enemy SpawnEnemy(Enemy enemy)
        {
            fx.Get<PoolAfterSeconds>(transform.position, Quaternion.identity);
            return enemy.Get<Enemy>(transform.position, Quaternion.identity);
        }
    }
}