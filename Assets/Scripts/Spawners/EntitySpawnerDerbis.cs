using System;
using UnityEngine;
using UnityEngine.Rendering;


public class EntitySpawnerDerbies : MonoBehaviour
{
    [SerializeField] private Destructible[] m_DestructiblePrefabs;

    [SerializeField] private CircleArea m_CircleAreas;

    [SerializeField] private int m_NumDerbies;

    [SerializeField] private float m_MinRandomSpeed;
    [SerializeField] private float m_MaxRandomSpeed;


    private void Start()
    {
        for(int i = 0; i < m_NumDerbies; i++)
        {
            SpawnDerbies();
        }
    }

    private void SpawnDerbies()
    {

        int index = UnityEngine.Random.Range(0, m_DestructiblePrefabs.Length);

        float currentSpeed = UnityEngine.Random.Range(m_MinRandomSpeed,m_MaxRandomSpeed);
        
        GameObject deb = Instantiate(m_DestructiblePrefabs[index]).gameObject;

        deb.transform.position = m_CircleAreas.GetRandomInsideZone;

        if(deb.TryGetComponent<Destructible>(out Destructible destructible))
        {
            destructible.EventOnDeath.AddListener(OnDerbiesDeath);
        }

        if(deb.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            if(m_MinRandomSpeed > 0)
            {
                rb.linearVelocity = UnityEngine.Random.insideUnitSphere * currentSpeed;
            }
        }
            

    }

    private void OnDerbiesDeath()
    {
        SpawnDerbies();
    }
}
