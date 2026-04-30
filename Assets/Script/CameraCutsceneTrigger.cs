using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float triggerRadius = 3f;

    [Header("Camera")]
    public Transform[] cameraPoints;
    public float[] waitTimes;
    public Camera mainCamera;
    public Transform playerCameraHolder;

    [Header("Player")]
    public MonoBehaviour playerMovement;

    private Transform player;
    private bool playerInRange = false;
    private bool hasPlayed = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || hasPlayed) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= triggerRadius;

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            StartCoroutine(PlayCutscene());
    }

    void OnGUI()
    {
        if (playerInRange && !hasPlayed)
            GUI.Label(new Rect(Screen.width / 2 - 120, Screen.height - 100, 240, 30), "Press F to start cutscene");
    }

    IEnumerator PlayCutscene()
    {
        hasPlayed = true;

        // 🔒 ล็อก player
        playerMovement.enabled = false;

        // แยกกล้องออก
        mainCamera.transform.parent = null;

        for (int i = 0; i < cameraPoints.Length; i++)
        {
            Transform point = cameraPoints[i];

            mainCamera.transform.position = point.position;
            mainCamera.transform.rotation = point.rotation;

            float wait = (i < waitTimes.Length) ? waitTimes[i] : 3f;
            yield return new WaitForSeconds(wait);
        }

        // 🔙 กลับกล้อง
        mainCamera.transform.SetParent(playerCameraHolder);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;

        // 🔓 ปลดล็อก player
        playerMovement.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}