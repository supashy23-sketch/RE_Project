using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHit = 3;
    private int currentHit = 0;

    [Header("UI Effect")]
    public Image damageOverlay; // ใส่ Image สีแดงใน Canvas
    public float fadeSpeed = 5f;

    [Header("Death")]
    public string sceneToLoad;

    private float targetAlpha = 0f;

    void Update()
    {
        // ค่อยๆจาง
        if (damageOverlay != null)
        {
            Color color = damageOverlay.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            damageOverlay.color = color;

            // ค่อยๆลดความแดงกลับ
            targetAlpha = Mathf.Lerp(targetAlpha, 0f, Time.deltaTime * 2f);
        }
    }

    public void TakeDamage()
    {
        currentHit++;

        // 🔴 ทำให้จอแดง
        targetAlpha += 0.4f;
        targetAlpha = Mathf.Clamp01(targetAlpha);

        Debug.Log("Player Hit: " + currentHit);

        if (currentHit >= maxHit)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Dead");

        // 🔓 ปลดล็อกเมาส์
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🔥 เปลี่ยนซีน
        SceneManager.LoadScene(sceneToLoad);
    }
}