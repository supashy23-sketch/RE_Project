using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [Header("Settings")]
    public string keyID = "key_01"; 
    public float pickupRadius = 3f;

    private Transform player;
    private PlayerKeyHolder keyHolder;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            keyHolder = playerObj.GetComponent<PlayerKeyHolder>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRadius;

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            PickUp();
    }

    void OnGUI()
    {
        if (playerInRange)
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 30), $"Press F to pick up [{keyID}]");
    }

    void PickUp()
    {
        if (keyHolder == null) return;

        keyHolder.AddKey(keyID);
        Debug.Log($"Picked up key: {keyID}");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}