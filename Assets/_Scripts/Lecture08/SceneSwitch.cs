using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Lecture08
{
    public class SceneSwitch : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene("Scene2");
            }
        }
    }
}
