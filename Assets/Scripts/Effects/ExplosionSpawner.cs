using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_ExplosionPrefab;
    [SerializeField] private float m_LifeTime;

    private Destructible destructible; 

    private void Start()
    {
        if (!TryGetComponent(out destructible)) return;

        destructible.EventOnDeath.AddListener(SpawnExplosion);
    }

    private void OnDestroy()
    {
        if(destructible == null)return;

        destructible.EventOnDeath.RemoveListener(SpawnExplosion);
    }

    public void SpawnExplosion()
    {
        if (m_ExplosionPrefab != null)
        {
            var explosion = Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);

           if(explosion.TryGetComponent(out ImpactEffect impactEffect))
            {
                impactEffect.m_LifeTime = m_LifeTime;
            }
        }
        
    }
}

 