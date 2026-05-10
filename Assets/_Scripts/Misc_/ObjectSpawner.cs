
using _Scripts.Lecture09;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{ 
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool usingPool;

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            if (usingPool)
            {
                var spawnedBall = ObjectPooler.GetFromPool("Ball", transform.position, transform.rotation);
                ObjectPooler.ReleaseToPool(spawnedBall, 3);
            }
            else
            {
                Instantiate(prefab, gameObject.transform);
            }
        }
    }
}
