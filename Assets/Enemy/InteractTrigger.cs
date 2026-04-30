using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float interactRadius = 3f;

    [Header("UI")]
    public string message = "Press F to activate";

    [Header("Action")]
    public Transform moveTarget;
    public List<AITarget> zombieList = new List<AITarget>();

    private Transform player;
    private bool playerInRange = false;

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
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactRadius;

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Activate();
        }
    }

    void Activate()
    {
        foreach (AITarget zombie in zombieList)
        {
            if (zombie != null && moveTarget != null)
            {
                zombie.ForceMoveToPoint(moveTarget);
            }
        }

        Debug.Log("Activated trigger!");
    }

    void OnGUI()
    {
        if (playerInRange)
        {
            GUI.Label(
                new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 30),
                message
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}