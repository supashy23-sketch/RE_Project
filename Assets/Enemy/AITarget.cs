using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    public Transform target;

    [Header("Distance")]
    public float DetectDistance = 10f;
    public float AggroDistance = 20f;
    public float AttackDistance = 2f;

    [Header("Attack")]
    public float AttackCooldown = 1.5f;

    private PlayerHealth playerHealth;

    private float lastAttackTime;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;

    private bool hasDetectedTarget = false;

    // 🔥 State system
    private enum AIState
    {
        Idle,
        ChasePlayer,
        ForcedMove
    }

    private AIState currentState = AIState.Idle;

    // 🔥 จุดหมายที่ถูกบังคับให้ไป
    private Transform forcedTargetPoint;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        playerHealth = target.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // 🔥 ถ้ามี ForcedMove ให้ทำก่อนทุกอย่าง
        if (currentState == AIState.ForcedMove && forcedTargetPoint != null)
        {
            HandleForcedMove();
            return; // ❗ ไม่ให้ไปไล่ผู้เล่น
        }

        m_Distance = Vector3.Distance(transform.position, target.position);

        float currentDetectDistance = hasDetectedTarget ? AggroDistance : DetectDistance;

        if (!hasDetectedTarget && m_Distance <= DetectDistance)
        {
            hasDetectedTarget = true;
        }

        // 🟥 โจมตี
        if (m_Distance <= AttackDistance)
        {
            HandleAttack();
        }
        // 🟨 ไล่ผู้เล่น
        else if (m_Distance <= currentDetectDistance)
        {
            HandleChase();
        }
        // 🟩 ยืนเฉย
        else
        {
            HandleIdle();
        }
    }

    // =========================
    // 🔥 PUBLIC FUNCTION (เรียกจากปุ่ม)
    // =========================
    public void ForceMoveToPoint(Transform point)
    {
        forcedTargetPoint = point;
        currentState = AIState.ForcedMove;

        m_Agent.isStopped = false;
        m_Agent.SetDestination(point.position);
    }

    // =========================
    // 🔥 FORCED MOVE
    // =========================
    void HandleForcedMove()
    {
        float dist = Vector3.Distance(transform.position, forcedTargetPoint.position);

        if (dist > 0.5f)
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(forcedTargetPoint.position);

            float speed = m_Agent.velocity.magnitude;
            m_Animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

            if (m_Agent.velocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(m_Agent.velocity.normalized);
            }
        }
        else
        {
            m_Agent.isStopped = true;
            m_Animator.SetFloat("Speed", 0);

            // 🔥 ถึงจุดแล้ว → กลับไป idle หรือจะให้ไล่ต่อก็ได้
            currentState = AIState.Idle;

            // ถ้าอยากให้กลับไปไล่ผู้เล่นแทน ใช้แบบนี้:
            // hasDetectedTarget = true;
            // currentState = AIState.ChasePlayer;
        }
    }

    // =========================
    // 🟥 ATTACK
    // =========================
    void HandleAttack()
    {
        m_Agent.isStopped = true;

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        if (Time.time >= lastAttackTime + AttackCooldown)
        {
            m_Animator.SetTrigger("Attack");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // 🔥 ลดเลือดตรงนี้
            }

            lastAttackTime = Time.time;
        }

        m_Animator.SetFloat("Speed", 0);
    }

    // =========================
    // 🟨 CHASE
    // =========================
    void HandleChase()
    {
        m_Agent.isStopped = false;
        m_Agent.SetDestination(target.position);

        float speed = m_Agent.velocity.magnitude;
        m_Animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        if (m_Agent.velocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(m_Agent.velocity.normalized);
        }
    }

    // =========================
    // 🟩 IDLE
    // =========================
    void HandleIdle()
    {
        m_Agent.isStopped = true;
        m_Animator.SetFloat("Speed", 0);

        // เลือกได้
        // hasDetectedTarget = false;
    }
}