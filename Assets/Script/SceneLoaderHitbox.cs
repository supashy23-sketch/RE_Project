using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderHitbox : MonoBehaviour
{
    [Header("Scene to Load")]
    public string sceneName;

    [Header("Settings")]
    public string triggerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoaderHitbox: No scene name set in Inspector!");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Collider col = GetComponent<Collider>();
        if (col is BoxCollider box)
            Gizmos.DrawCube(transform.position, box.size);
        else
            Gizmos.DrawSphere(transform.position, 1f);
    }
}