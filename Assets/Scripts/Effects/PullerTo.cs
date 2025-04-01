
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PullerTo : MonoBehaviour
{
    [SerializeField] private float m_Force;
    [SerializeField] private float m_Radius;

    private Rigidbody2D m_Rigidbody;

    [HideInInspector] public bool m_isActive = false;


    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Rigidbody.gravityScale = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_isActive)
        {
            if (collision.attachedRigidbody == null) return;

            Vector2 dir = collision.transform.position - transform.position;

            float dist = dir.magnitude;

            float strength = Mathf.Clamp01(1 - (dist / m_Radius));

            if(dist < 0.3f)
            {
                m_Rigidbody.linearVelocity = Vector2.zero;
            }
            else 
            {
                Vector2 force = dir.normalized * m_Force * strength * strength;

                m_Rigidbody.AddForce(force, ForceMode2D.Force);
            }
     
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GetComponent<CircleCollider2D>().radius = m_Radius;
    }
#endif
}

