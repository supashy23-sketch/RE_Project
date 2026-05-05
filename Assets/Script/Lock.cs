using UnityEngine;
using UnityEngine.InputSystem;

public class Lock : MonoBehaviour
{
    [Header("Settings")]
    public string keyID = "key_01";      
    public float unlockRadius = 3f;

    [Header("Chain Key (Optional)")]
    public bool grantsNewKey = false;    
    public string grantedKeyID = "";   

    private Transform player;
    private PlayerKeyHolder keyHolder;
    private bool playerInRange = false;

    void Update()
    {
        if (player == null || keyHolder == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                keyHolder = playerObj.GetComponent<PlayerKeyHolder>();

                if (keyHolder == null)
                    Debug.LogError("PlayerKeyHolder component not found");
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        bool inRadius = distance <= unlockRadius;
        bool hasMatchingKey = keyHolder.HasKey(keyID);

        playerInRange = inRadius && hasMatchingKey;

        if (inRadius && !hasMatchingKey)
            Debug.Log($"Lock [{keyID}]: In range but player does not have this key. Keys held: {keyHolder.GetKeyList()}");

        if (playerInRange && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Unlock();
    }

    void OnGUI()
    {
        if (playerInRange)
        {
            string label = grantsNewKey && grantedKeyID != ""
                ? $"Press F to unlock [{keyID}] → grants [{grantedKeyID}]"
                : $"Press F to unlock [{keyID}]";

            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 30), label);
        }
    }

    void Unlock()
    {
        keyHolder.RemoveKey(keyID);

        if (grantsNewKey && grantedKeyID != "")
        {
            keyHolder.AddKey(grantedKeyID);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = grantsNewKey ? Color.green : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, unlockRadius);
    }
}