using UnityEngine;

namespace _Scripts.Misc_
{
    public class DestroyMe : MonoBehaviour
    {
        public bool usingPool;
        
        private void Start()
        {
            if (usingPool)
            {
                return;
            }
            
            Destroy(gameObject, 3);
        }
    }
}
