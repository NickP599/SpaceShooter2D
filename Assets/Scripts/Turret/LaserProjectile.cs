using UnityEngine;

public class LaserProjectile : Projectile
{
    [SerializeField] private float m_defDistanceRat = 10f;

    private LineRenderer m_LineRenderer;
    private Transform m_Target;

    private Vector2 m_EndPos;
    private void Awake()
    {
        m_LineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        Destroy(gameObject, m_LifeTime);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, m_defDistanceRat);

        m_EndPos = (Vector2)transform.position + (Vector2)transform.up * m_defDistanceRat;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.transform != m_Parent.transform)
            {
                m_EndPos = hit.point;
                m_Target = hit.transform;

                foreach (Destructible destructible in hit.transform.root.GetComponents<Destructible>())
                {
                    destructible.ApplyDamage(m_Damage);
                    AddScoresAndKill(destructible);            
                }
            }
        }

        Draw2DRay(transform.position, m_EndPos);
        Debug.DrawRay(transform.position, transform.up * m_defDistanceRat, Color.red, 0.1f);
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_LineRenderer.SetPosition(0, startPos);
        m_LineRenderer.SetPosition(1, endPos);
    }
}