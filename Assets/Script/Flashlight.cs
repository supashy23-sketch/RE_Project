using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Settings")]
    public Light flashlight;     
    public KeyCode toggleKey = KeyCode.T;

    void Start()
    {
        if (flashlight == null)
            flashlight = GetComponent<Light>();

        if (flashlight == null)
            Debug.LogError("Flashlight: No Light component assigned or found!");
        else
            flashlight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            flashlight.enabled = !flashlight.enabled;
    }
}