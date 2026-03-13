using UnityEngine;

namespace _Scripts.Lecture08
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int health;
        public float moveSpeed;
        public float edgeDetectionDistance;
        public float wallDetectionDistance;
        public LayerMask groundLayer;
    }
}
