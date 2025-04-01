using UnityEngine;
[RequireComponent (typeof(Rigidbody2D))]
public class DistanceBombProjectile : Projectile 
{
    [SerializeField] private float m_MoveTime;
    [SerializeField] private float m_Distance;
    
    private Rigidbody2D m_Rigidbody;
    private PullerTo pullerTo;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        pullerTo = GetComponent<PullerTo>();
      
        pullerTo.m_isActive = false;

        m_Rigidbody.gravityScale = 0;       
        m_Timer = m_MoveTime;

    }

    private void FixedUpdate()
    {
        m_Timer -= Time.fixedDeltaTime;

        if (m_Timer < 0)
        {
           m_Timer = 0;
           m_Rigidbody.linearVelocity = Vector2.zero;
           pullerTo.m_isActive = true;

        }

        float stepLength = m_Velocity * Time.fixedDeltaTime;

        Vector2 origin = transform.position;   
        Vector2 direction = transform.up;

        float radius = 0.5f;
        float distance = stepLength * m_Distance;

        RaycastHit2D hit = Physics2D.CircleCast(origin,radius,direction,distance);
        
        if (hit.transform != null)
        {
            GameObject r  = hit.collider.gameObject;

            if (hit.collider.transform.root.TryGetComponent<Destructible>(out Destructible destructible))
            {
                destructible.ApplyDamage(m_Damage);

                OnProjectileLifeEnd(hit.collider, hit.point);
            }
            Debug.DrawRay(transform.position, direction * distance, Color.red, 0.1f);
            Debug.Log("hit = " + r.name);
        }


        Vector2 step = transform.up * stepLength;
        UpdateRigidBody(step);

        

        
    }

 

    private void UpdateRigidBody(Vector2 step)
    {
        m_Rigidbody.AddForce(step, ForceMode2D.Impulse);
             
    }


}
