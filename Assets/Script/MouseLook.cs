using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerCamera;

    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("Clamp")]
    public float xRotation = 0f;
    public float minX = -80f;
    public float maxX = 80f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minX, maxX);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * mouseX);
    }
}