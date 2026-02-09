using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySecondScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Invoke(nameof(HitNotification), 3);
            StartCoroutine(HitNotificationRoutine());
        }
    }

    private IEnumerator HitNotificationRoutine()
    {
        Debug.Log("Player hit!");
        yield return new WaitForSeconds(3);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    private void HitNotification()
    {
        Debug.Log("Player hit!");
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
