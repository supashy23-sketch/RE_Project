using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    public Transform target;

    [Header("Distance")]
    public float DetectDistance = 10f;
    public float AggroDistance = 20f; // 🔥 ระยะใหม่หลังเจอเป้าหมาย
    public float AttackDistance = 2f;

    [Header("Attack")]
    public float AttackCooldown = 1.5f;

    private float lastAttackTime;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;

    private bool hasDetectedTarget = false; // 🔥 จำว่าเคยเจอแล้ว

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        m_Distance = Vector3.Distance(transform.position, target.position);

        // 🔥 ถ้าเคยเจอแล้ว ใช้ระยะ Aggro แทน
        float currentDetectDistance = hasDetectedTarget ? AggroDistance : DetectDistance;

        // 🔥 เช็คว่าพึ่งเจอเป้าหมายครั้งแรก
        if (!hasDetectedTarget && m_Distance <= DetectDistance)
        {
            hasDetectedTarget = true;
        }

        // 🟥 โจมตี
        if (m_Distance <= AttackDistance)
        {
            m_Agent.isStopped = true;

            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            if (Time.time >= lastAttackTime + AttackCooldown)
            {
                m_Animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }

            m_Animator.SetFloat("Speed", 0);
        }
        // 🟨 เดินเข้าไปหา
        else if (m_Distance <= currentDetectDistance)
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
        // 🟩 ยืนเฉย
        else
        {
            m_Agent.isStopped = true;
            m_Animator.SetFloat("Speed", 0);

            // (เลือกได้) รีเซ็ต aggro ถ้าอยากให้ลืมเป้าหมาย
            // hasDetectedTarget = false;
        }
    }
}