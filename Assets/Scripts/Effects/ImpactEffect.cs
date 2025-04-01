using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public float m_LifeTime;

    private void Start()
    {
        Destroy(gameObject, m_LifeTime);
      
    }
}
