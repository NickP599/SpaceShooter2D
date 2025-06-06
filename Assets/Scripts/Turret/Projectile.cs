using UnityEngine;

public class Projectile : Entity
{
    [SerializeField] protected float m_Velocity;
    [SerializeField] protected float m_LifeTime;
    [SerializeField] protected int m_Damage;
    public int Damage => m_Damage;
    public float Velocity => m_Velocity;    

    [SerializeField] protected ImpactEffect m_EffectPrefab;

    protected float m_Timer;

    private void  Update()
    {
        float stepLength = m_Velocity * Time.fixedDeltaTime;

        Vector2 step = transform.up * stepLength;

        transform.position += new Vector3(step.x, step.y ,0);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLength);

        if (hit)
        {
            if(hit.collider.transform.root.TryGetComponent<Destructible>(out Destructible destructible))
            {  
                if(destructible != m_Parent)             
                   destructible.ApplyDamage(m_Damage);

                AddScoresAndKill(destructible);
            }

            OnProjectileLifeEnd(hit.collider, hit.point); 
        }

        m_Timer += Time.deltaTime;

        if(m_Timer > m_LifeTime)
            Destroy(gameObject);
    }

    protected void OnProjectileLifeEnd(Collider2D col,Vector2 pos)
    {
        Destroy(gameObject);
    }

    protected Destructible m_Parent;

    protected  void AddScoresAndKill(Destructible destructible)
    {
        if (m_Parent == Player.Instance.ActiveShip)
        {
            Player.Instance.AddScores(destructible.ScoreValue);

            if (destructible is SpaceShip)
            {
                if (destructible.HitPoints <= 0)
                {
                    Player.Instance.AddKill();
                }
            }
        }
    }

    public void SetParentDestructible(Destructible parent)
    {
        m_Parent = parent;
    }
}
