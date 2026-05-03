using System.Collections;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float triggerRadius = 3f;

    [Header("Camera")]
    public Transform[] cameraPoints;
    public float[] waitTimes;

    public Camera mainCamera;                 // Main Camera
    public Transform playerCameraHolder;      // จุดที่กล้องอยู่ตอนปกติ
    public GameObject cinemachineCamera;      // ตัว Cinemachine Virtual Camera

    [Header("Player")]
    public MonoBehaviour playerMovement;      // FirstPersonController

    private Transform player;
    private bool playerInRange = false;
    private bool hasPlayed = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || hasPlayed) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= triggerRadius;

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PlayCutscene());
        }
    }

    void OnGUI()
    {
        if (playerInRange && !hasPlayed)
        {
            GUI.Label(new Rect(Screen.width / 2 - 120, Screen.height - 100, 240, 30),
                "Press F to start cutscene");
        }
    }

    IEnumerator PlayCutscene()
    {
        hasPlayed = true;

        Debug.Log("Cutscene Start");

        // 🔒 ล็อก player
        if (playerMovement != null)
            playerMovement.enabled = false;

        // ❗ ปิด Cinemachine (สำคัญมาก)
        if (cinemachineCamera != null)
            cinemachineCamera.SetActive(false);

        // แยกกล้องออกจาก player
        mainCamera.transform.SetParent(null);

        // 🎬 เล่นคัทซีนตามจุด
        for (int i = 0; i < cameraPoints.Length; i++)
        {
            Transform point = cameraPoints[i];

            Debug.Log("Move to: " + point.name);

            mainCamera.transform.position = point.position;
            mainCamera.transform.rotation = point.rotation;

            float wait = (i < waitTimes.Length) ? waitTimes[i] : 3f;
            yield return new WaitForSeconds(wait);
        }

        // 🔙 กลับกล้องไป player
        mainCamera.transform.SetParent(playerCameraHolder);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;

        // ❗ เปิด Cinemachine กลับ
        if (cinemachineCamera != null)
            cinemachineCamera.SetActive(true);

        // 🔓 ปลดล็อก player
        if (playerMovement != null)
            playerMovement.enabled = true;

        Debug.Log("Cutscene End");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}