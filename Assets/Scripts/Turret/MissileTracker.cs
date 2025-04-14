
using UnityEngine;

//Изменить класс !!! 
public class MissileTracker : Projectile
{
    [SerializeField] private float m_DetectionRadius;
    [SerializeField] private float m_MaxDistance;

    private Transform m_Target;
    //Изменить класс !!! 
    private void Update()
    {
        float stepLength = m_Velocity * Time.deltaTime;
        Vector2 step = transform.up * stepLength;

        if (m_Target == null)
        {
            
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, m_DetectionRadius, transform.up, m_MaxDistance);

            if (hit.collider != null)
            {
                m_Target = hit.transform;
            }
        }
        
        if(m_Target != null)
        {
            
            Vector2 direction = (m_Target.transform.position - transform.position).normalized;
            transform.up = Vector2.Lerp(transform.up ,direction, Time.deltaTime * m_Velocity);
        }

        RaycastHit2D hitToDamage = Physics2D.Raycast(transform.position, transform.up, stepLength);

        if (hitToDamage)
        {
            if (hitToDamage.collider.transform.root.TryGetComponent<Destructible>(out Destructible destructible))
            {
                if (destructible != m_Parent)
                    destructible.ApplyDamage(m_Damage);

                   AddScoresAndKill(destructible);
            }

            OnProjectileLifeEnd(hitToDamage.collider, hitToDamage.point);
        }

        transform.position += new Vector3(step.x, step.y, 0);

        Debug.DrawRay(transform.position, hitToDamage.point * stepLength, Color.red, 0.1f);


        m_Timer += Time.deltaTime;

        if (m_Timer > m_LifeTime)
            Destroy(gameObject);

    }

   

    private void OnProjectileLifeEnd(Collider2D col, Vector2 pos)
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRadius);
    }

}


