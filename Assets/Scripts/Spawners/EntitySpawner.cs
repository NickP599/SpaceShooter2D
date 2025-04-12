using System;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public enum SpawnMode
    {
        Start,
        Loop
    }

    [SerializeField] private Entity[] m_EntityPrefabs;

    [SerializeField] private CircleArea m_CircleArea;

    [SerializeField] private SpawnMode m_SpawnMode;

    [SerializeField] private int m_NumSpawns;

    [SerializeField] private int m_TeamId;

    [SerializeField] private float m_RespawnTime;

    private float m_Timer;

    private void Start()
    {
        if(m_SpawnMode == SpawnMode.Start)
        {
            SpawnEntities();
        }

        m_Timer = m_RespawnTime;
    }

    private void Update()
    {
        if(m_Timer > 0)
          m_Timer -= Time.deltaTime;

        if(m_SpawnMode == SpawnMode.Loop)
        {
            SpawnEntities();
            m_Timer = m_RespawnTime;
        }

    }

    private void SpawnEntities()
    {
        
        for(int i = 0; i < m_NumSpawns; i++)
        {
            int index = UnityEngine.Random.Range(0, m_EntityPrefabs.Length);

            GameObject entity = Instantiate(m_EntityPrefabs[index]).gameObject;
            entity.transform.position = m_CircleArea.GetRandomInsideZone;

            Destructible des = entity.GetComponent<Destructible>();
            des.TeamId = m_TeamId;


        }
    }
}
